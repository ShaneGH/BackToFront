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

namespace BackToFront.Logic
{
    public class ValidateResult<TEntity> : IValidateResult<TEntity>
    {
        private readonly TEntity Entity;
        private readonly IEnumerable<RuleDependency> Dependencies;
        private readonly List<Mock> Mocks = new List<Mock>();
        private readonly List<Func<IValidateResult>> ValidateChildMembers = new List<Func<IValidateResult>>();
        private readonly ValidateOptions Options;

        public ValidateResult(TEntity entity)
            : this(entity, new ValidateOptions(), null)
        {
        }

        public ValidateResult(TEntity entity, ValidateOptions options, object dependencyClasses)
        {
            Dependencies = dependencyClasses == null ? Enumerable.Empty<RuleDependency>() : ToDependencies(dependencyClasses);
            Entity = entity;
            Options = options ?? new ValidateOptions();
        }

        /// <summary>
        /// Create a carbon copy which shares readonly ref data 
        /// </summary>
        /// <param name="entity"></param>
        /// <param name="options"></param>
        /// <param name="dependencyClasses"></param>
        /// <param name="mocks"></param>
        private ValidateResult(TEntity entity, ValidateOptions options, IEnumerable<RuleDependency> dependencyClasses, List<Mock> mocks)
        {
            Entity = entity;
            Options = options;
            Dependencies = dependencyClasses;
            Mocks = mocks;
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

                    foreach (var child in ValidateChildMembers)
                    {
                        if (_FirstViolation != null)
                            break;

                        _FirstViolation = child().FirstViolation;
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
                    var violations = new List<IViolation>();
                    RunValidation((rule, mocks) => 
                    {
                        var allViolations = new List<IViolation>();
                        rule.FullyValidateEntity(Entity, allViolations, mocks);

                        violations.AddRange(allViolations);
                        return !allViolations.Any();
                    });

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
        private void RunValidation(Func<IRuleValidation<TEntity>, ValidationContext, bool> action)
        {
            // segregate from global object
            var mocks = Mocks.ToArray();

            // mocks which require their value to be persisted
            var setters = mocks.Where(a => a.Behavior == MockBehavior.MockAndSet || a.Behavior == MockBehavior.SetOnly).ToArray();
            
            if (setters.Any(a => !a.CanSet))
                throw new InvalidOperationException("##");

            // result of all violations. If true, mocks will be persisted
            bool success = true;
            var dependencies = (IEnumerable<RuleDependency>)Dependencies.ToArray();

            // add parent class rules
            IEnumerable<IEnumerable<IRuleValidation<TEntity>>> rulesRepository = new[] { Rules<TEntity>.Repository.Registered };
            if (Options.ValidateAgainstParentClassRules)
                rulesRepository = rulesRepository.Concat(Rules<TEntity>.ParentClassRepositories);

            rulesRepository.Aggregate().Each(rule =>
            {
                ValidateDependencies(rule.Dependencies, ref dependencies);
                var newMocks = mocks.Where(a => a.Behavior == MockBehavior.MockOnly || a.Behavior == MockBehavior.MockAndSet).ToList();
                newMocks.AddRange(dependencies.Select(d => d.ToMock()));

                success &= action(rule, new ValidationContext { Mocks = new Mocks(newMocks) });
            });

            // success, persist mock values
            if(success)
                setters.Each(a => a.SetValue(Entity));
        }

        internal static void ValidateDependencies(IEnumerable<DependencyWrapper> required, ref IEnumerable<RuleDependency> delivered)
        {
            List<RuleDependency> requiredByRule = new List<RuleDependency>();
            foreach (var r in required)
            {
                var match = delivered.FirstOrDefault(a => a.Name == r.DependencyName);
                if (match != null && match.Value != null)
                {
                    if (!match.Value.GetType().Is(r.DependencyType))
                        throw new InvalidOperationException("##");

                    requiredByRule.Add(match);
                }
            }

            delivered = requiredByRule.ToArray();
        }

        internal static IEnumerable<RuleDependency> ToDependencies(object input)
        {
            var dependencies = input.GetType().GetProperties(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance);
            return dependencies.Select(d => new RuleDependency(d.Name, d.GetValue(input)));
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

            IEnumerable<RuleDependency> dependencies = dependencyClasses == null ? null : ToDependencies(dependencyClasses);

            MemberExpression tester = member.Body as MemberExpression;
            while (tester != null)
            {
                if (tester.Expression is ParameterExpression)
                {
                    var compiled = member.Compile();
                    ValidateChildMembers.Add(() => new ValidateResult<TParameter>(compiled(Entity), Options, dependencies ?? Dependencies, Mocks.Select(m => 
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
