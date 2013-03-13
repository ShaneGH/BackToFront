using BackToFront.Utils;
using BackToFront.Utils.Expressions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using BackToFront.Extensions.IEnumerable;

namespace BackToFront.Validate
{
    internal class ValidateResult<TEntity> : IValidateResult<TEntity>
    {
        private readonly TEntity Entity;

        private readonly List<Mock> Mocks = new List<Mock>();

        public ValidateResult(TEntity entity)
        {
            Entity = entity;
        }

        public IViolation _FirstViolation;
        public IViolation FirstViolation
        {
            get
            {
                if (_FirstViolation == null)
                {
                    RunValidation((a, b) => (_FirstViolation = a.ValidateEntity(Entity, b)) == null);
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
                    RunValidation((a, b) => (_AllViolations = a.FullyValidateEntity(Entity, b)) == null || !_AllViolations.Any());
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

        private void RunValidation(Func<IValidate, IEnumerable<Mock>, bool> action)
        {
            var mocks = Mocks.ToArray();
            var setters = mocks.Where(a => a.Behavior == MockBehavior.MockAndSet || a.Behavior == MockBehavior.SetOnly);
            var invalidSetters = setters.Where(a => !a.Expression.CanSet);
            if (invalidSetters.Any())
                throw new InvalidOperationException("##");

            if (action(Rules.Repository.Registered[typeof(TEntity)], mocks.Where(a => (a.Behavior == MockBehavior.MockAndSet || a.Behavior == MockBehavior.MockOnly))))
                setters.Each(a => a.SetValue(Entity));
        }

        public void ResetResult()
        {
            _FirstViolation = null;
            _AllViolations = null;
        }
    }

    public static class BusinessLogic
    {
        /// <summary>
        /// Validate this object
        /// </summary>
        /// <typeparam name="T">The object type</typeparam>
        /// <param name="test">The object</param>
        /// <returns>The first business rule violation encountered</returns>
        public static IValidateResult<TEntity> Validate<TEntity>(this TEntity test)
        {
            return new ValidateResult<TEntity>(test);
        }
    }
}