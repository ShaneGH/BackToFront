using BackToFront.Dependency;
using BackToFront.Enum;
using BackToFront.Expressions;
using BackToFront.Extensions.IEnumerable;
using BackToFront.Extensions.Reflection;
using BackToFront.Framework;
using BackToFront.Utilities;
using BackToFront.Validate;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using BackToFront.Validation;
using BackToFront.Expressions.Visitors;

namespace BackToFront.Logic
{
    public class ValidateResult<TEntity> : IValidateResult<TEntity>
    {
        private readonly Repository _Repository;
        private readonly TEntity Entity;
        private readonly IDictionary<string, object> Dependencies;
        private readonly List<Mock> Mocks = new List<Mock>();
        private readonly List<Func<IValidateResult>> ValidateChildMembers = new List<Func<IValidateResult>>();
        private readonly ValidateOptions Options;

        public ValidateResult(TEntity entity, Repository repository)
            : this(entity, repository, new ValidateOptions(), null)
        {
        }

        public ValidateResult(TEntity entity, Repository repository, ValidateOptions options, object dependencyClasses)
            : this(entity, repository, options, dependencyClasses == null ? Enumerable.Empty<KeyValuePair<string, object>>() : ToDependencies(dependencyClasses), Enumerable.Empty<Mock>())
        {
        }

        /// <summary>
        /// Create a carbon copy which shares readonly ref data 
        /// </summary>
        /// <param name="entity"></param>
        /// <param name="options"></param>
        /// <param name="dependencyClasses"></param>
        /// <param name="mocks"></param>
        private ValidateResult(TEntity entity, Repository repository, ValidateOptions options, IEnumerable<KeyValuePair<string, object>> dependencyClasses, IEnumerable<Mock> mocks)
        {
            if (entity == null || repository == null)
                throw new InvalidOperationException("##");

            Entity = entity;
            _Repository = repository;
            Options = options ?? new ValidateOptions();
            Dependencies = dependencyClasses.ToDictionary(a => a.Key, a => a.Value);

            Mocks.AddRange(mocks);
        }

        public IViolation _FirstViolation;
        public IViolation FirstViolation
        {
            get
            {
                if (_FirstViolation == null)
                {
                    if (_AllViolations != null)
                    {
                        _FirstViolation = _AllViolations.FirstOrDefault();
                    }
                    else
                    {
                        _FirstViolation = RunValidation(true).FirstOrDefault();

                        if (_FirstViolation == null)
                        {
                            foreach (var child in ValidateChildMembers)
                            {
                                if (_FirstViolation != null)
                                    break;

                                _FirstViolation = child().FirstViolation;
                            }
                        }
                    }
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
                    var violations = RunValidation(false);
                    ValidateChildMembers.Each(child => violations.AddRange(child().AllViolations));
                    _AllViolations = violations.ToArray();
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
        private IList<IViolation> RunValidation(bool breakOnFirstError)
        {
            // segregate from global object
            var mocks = Mocks.ToArray();

            // mocks which require their value to be persisted
            var setters = mocks.Where(a => a.Behavior == MockBehavior.MockAndSet || a.Behavior == MockBehavior.SetOnly).ToArray();
            
            if (setters.Any(a => !a.CanSet))
                throw new InvalidOperationException("##");

            // result of all violations. If true, mocks will be persisted
            bool success = true;

            var violations = new List<IViolation>();
            var current = typeof(TEntity);
            while (current != null)
            {
                var visitor = new SwapPropVisitor(mocks.Where(a => a.Behavior == MockBehavior.MockOnly || a.Behavior == MockBehavior.MockAndSet), Dependencies, current);
                var context = new ValidationContextX(breakOnFirstError, visitor.MockValues, visitor.DependencyValues);

                // compile and run
                _Repository.Rules(current).Each(rule => rule.NewCompile(visitor)(Entity, context));
                success &= !context.IsViolated;
                violations.AddRange(context.Violations);

                if (!success && breakOnFirstError)
                    break;

                current = current.BaseType;
            }

            // success, persist mock values
            if(success)
                setters.Each(a => a.SetValue(Entity));

            return violations;
        }

        internal static void ValidateDependencies(IEnumerable<DependencyWrapper> required, ref IEnumerable<KeyValuePair<string, object>> delivered)
        {
            List<KeyValuePair<string, object>> requiredByRule = new List<KeyValuePair<string, object>>();
            foreach (var r in required)
            {
                var match = delivered.FirstOrDefault(a => a.Key == r.DependencyName);
                if (!string.IsNullOrEmpty(match.Key) && match.Value != null)
                {
                    if (!match.Value.GetType().Is(r.DependencyType))
                        throw new InvalidOperationException("##");

                    requiredByRule.Add(match);
                }
            }

            delivered = requiredByRule.ToArray();
        }

        internal static IEnumerable<KeyValuePair<string, object>> ToDependencies(object input)
        {
            var dependencies = input.GetType().GetProperties(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance);
            return dependencies.Select(d => new KeyValuePair<string, object>(d.Name, d.GetValue(input)));
        }

        public void ResetResult()
        {
            _FirstViolation = null;
            _AllViolations = null;
        }

        public IValidateResult<TEntity> ValidateMember<TParameter>(Expression<Func<TEntity, TParameter>> member)
        {
            return ValidateMember(member, null);
        }

        public IValidateResult<TEntity> ValidateMember<TParameter>(Expression<Func<TEntity, TParameter>> member, object dependencyClasses)
        {
            if (member.Body is ParameterExpression)
                throw new InvalidOperationException("##");

            IEnumerable<KeyValuePair<string, object>> dependencies = dependencyClasses == null ? null : ToDependencies(dependencyClasses);

            MemberExpression tester = member.Body as MemberExpression;
            while (tester != null)
            {
                if (tester.Expression is ParameterExpression)
                {
                    var compiled = member.Compile();
                    ValidateChildMembers.Add(() => new ValidateResult<TParameter>(compiled(Entity), _Repository, Options, dependencies ?? Dependencies, Mocks.Select(m => 
                        {
                            Mock output;
                            if (m.TryForChild(member, Expression.Parameter(typeof(TParameter)), out output) && !(output.Expression is ParameterExpressionWrapper))
                                return output;

                            return null;
                        }).Where(m => m!= null).ToList()));

                    return this;
                }

                // ensure it is a full member chain before validating (no complex expressions)
                tester = tester.Expression as MemberExpression;
            }

            throw new InvalidOperationException("##");
        }
    }
}
