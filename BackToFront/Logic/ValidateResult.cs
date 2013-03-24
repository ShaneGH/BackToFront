using BackToFront.Enum;
using BackToFront.Extensions.IEnumerable;
using BackToFront.Framework;
using BackToFront.Utils;
using BackToFront.Expressions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using BackToFront.Extensions.Reflection;

namespace BackToFront.Logic
{
    internal class Dependency
    {
        public string Name { get; set; }
        public object Value { get; set; }
    }

    public class ValidateResult<TEntity> : IValidateResult<TEntity>
    {
        private readonly TEntity Entity;
        private readonly IEnumerable<Dependency> Dependencies;
        private readonly List<Mock> Mocks = new List<Mock>();

        public ValidateResult(TEntity entity, object dependencyClasses)
        {
            if(dependencyClasses == null)
            {
                Dependencies = Enumerable.Empty<Dependency>();
            }
            else
            {
                var dependencies = dependencyClasses.GetType().GetProperties(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance);
                Dependencies = dependencies.Select(h => new Dependency { Name = h.Name, Value = h.GetValue(dependencyClasses) });
            }

            Entity = entity;
        }

        public IViolation _FirstViolation;
        public IViolation FirstViolation
        {
            get
            {
                if (_FirstViolation == null)
                {
                    var skip = false;
                    RunValidation((rule, mocks) =>
                    {
                        // don't run all rules if a violation is found
                        if (skip)
                            return false;

                        _FirstViolation = rule.ValidateEntity(Entity, mocks);
                        if (_FirstViolation != null)
                        {
                            skip = true;
                            return false;
                        }

                        return true;
                    });
                }

                return _FirstViolation;
            }
        }

        public IEnumerable<IViolation> _AllViolations;
        public IEnumerable<IViolation> AllViolations
        {
            get
            {
                if (_AllViolations == null)
                {
                    _AllViolations = Enumerable.Empty<IViolation>();
                    RunValidation((rule, mocks) => 
                    {
                        var allViolations = new List<IViolation>();
                        rule.FullyValidateEntity(Entity, allViolations, mocks);

                        _AllViolations = _AllViolations.Concat(allViolations);
                        return !allViolations.Any(); 
                    });
                }

                return _AllViolations;
            }
        }

        public IValidateResult<TEntity> WithMockedParameter<TParameter>(Expression<Func<TEntity, TParameter>> property, TParameter value, MockBehavior behavior)
        {
            // invalidate previous result
            ResetResult();
            Mocks.Add(new Mock(ExpressionWrapperBase.ToWrapper(property), value, typeof(TParameter), behavior));
            return this;
        }

        public IValidateResult<TEntity> WithMockedParameter<TParameter>(Expression<Func<TEntity, TParameter>> property, TParameter value)
        {
            return WithMockedParameter(property, value, MockBehavior.MockOnly);
        }

        /// <summary>
        /// Orders rules, mocks and dependencies and delivers them to a function (for vaslidation)
        /// </summary>
        /// <param name="action">Validation function. Returns </param>
        private void RunValidation(Func<Rule<TEntity>, Mocks, bool> action)
        {
            // segregate from global object
            var mocks = Mocks.ToArray();

            // mocks which require their value to be persisted
            var setters = mocks.Where(a => a.Behavior == MockBehavior.MockAndSet || a.Behavior == MockBehavior.SetOnly).ToArray();
            
            if (setters.Any(a => !a.CanSet))
                throw new InvalidOperationException("##");

            // resut of all violations. If true, mocks will be persisted
            bool success = true;
            foreach (var rule in Rules<TEntity>.Repository.Registered)
            {
                var dependencies = (IEnumerable<Dependency>)Dependencies.ToArray();
                ValidateDependencies(rule.Dependencies, ref dependencies);

                var newMocks = mocks.Where(a => a.Behavior == MockBehavior.MockOnly || a.Behavior == MockBehavior.MockAndSet).ToList();

                // add each dependency class as a MockOnly mock.
                dependencies.Each(a => 
                {
                    // TODO: Null reference in GetType
                    newMocks.Add(new Mock(new ConstantExpressionWrapper(Expression.Constant(a)), a.Value, a.Value.GetType(), MockBehavior.MockOnly));
                });

                success &= action(rule, new Mocks(newMocks));
            }

            // success, persist mock values
            if(success)
                setters.Each(a => a.SetValue(Entity));
        }

        private static void ValidateDependencies(IEnumerable<DependencyWrapper> required, ref IEnumerable<Dependency> delivered)
        {
            List<Dependency> requiredByRule = new List<Dependency>();
            foreach (var r in required)
            {
                var match = delivered.FirstOrDefault(a => a.Name == r.DependencyName);
                if (match == null)
                    throw new InvalidOperationException("##");

                if (match.Value == null || !match.Value.GetType().Is(r.DependencyType))
                    throw new InvalidOperationException("##");

                requiredByRule.Add(match);
            }

            delivered = requiredByRule.ToArray();
        }

        public void ResetResult()
        {
            _FirstViolation = null;
            _AllViolations = null;
        }
    }
}
