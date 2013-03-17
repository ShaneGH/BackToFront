using BackToFront.Enum;
using BackToFront.Extensions.IEnumerable;
using BackToFront.Framework;
using BackToFront.Utils;
using BackToFront.Utils.Expressions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using BackToFront.Extensions.Reflection;

namespace BackToFront.Logic
{
    public class ValidateResult<TEntity> : IValidateResult<TEntity>
    {
        private readonly TEntity Entity;
        private readonly IEnumerable<object> Helpers;
        private readonly List<Mock> Mocks = new List<Mock>();

        public ValidateResult(TEntity entity, params object[] helperClasses)
        {
            Helpers = helperClasses == null ? Enumerable.Empty<object>() : helperClasses.ToArray();
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
            Mocks.Add(new Mock(ExpressionWrapperBase.ToWrapper(property), value, behavior));
            return this;
        }

        public IValidateResult<TEntity> WithMockedParameter<TParameter>(Expression<Func<TEntity, TParameter>> property, TParameter value)
        {
            return WithMockedParameter(property, value, MockBehavior.MockOnly);
        }

        /// <summary>
        /// Orders rules, mocks and helpers and delivers them to a function (for vaslidation)
        /// </summary>
        /// <param name="action">Validation function. Returns </param>
        private void RunValidation(Func<Rule<TEntity>, IEnumerable<Mock>, bool> action)
        {
            // segregate from global object
            IEnumerable<Mock> mocks = Mocks.ToArray();

            // mocks which require their value to be persisted
            var setters = mocks.Where(a => a.Behavior == MockBehavior.MockAndSet || a.Behavior == MockBehavior.SetOnly).ToArray();
            
            if (setters.Any(a => !a.CanSet))
                throw new InvalidOperationException("##");

            // resut of all violations. If true, mocks will be persisted
            bool success = true;
            foreach (var rule in Rules<TEntity>.Repository.Registered)
            {
                // invalid helpers delivered to validate function
                if (rule.HelperPointers.Count != Helpers.Count())
                    throw new InvalidOperationException("##");

                var newMocks = mocks.Where(a => a.Behavior == MockBehavior.MockOnly || a.Behavior == MockBehavior.MockAndSet).ToList();

                // add each helper class as a MockOnly mock. When defined 
                Helpers.Each((a, i) => 
                {
                    if (!a.GetType().Is(rule.HelperPointers[i].Item1))
                        throw new InvalidOperationException("##");
                    else
                        newMocks.Add(new Mock(new ConstantExpressionWrapper(Expression.Constant(rule.HelperPointers[i]), null), a, MockBehavior.MockOnly));
                });

                success &= action(rule, newMocks);
            }

            // success, persist mock values
            if(success)
                setters.Each(a => a.SetValue(Entity));
        }

        public void ResetResult()
        {
            _FirstViolation = null;
            _AllViolations = null;
        }
    }
}
