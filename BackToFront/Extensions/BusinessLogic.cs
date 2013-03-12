using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

using BackToFront.Utils;
using BackToFront.Utils.Expressions;
using BackToFront.Extensions.Expressions;

namespace BackToFront.Validate
{
    internal class ValidateResult<TEntity> : IValidateResult<TEntity>
    {
        private readonly TEntity Entity;

        private readonly List<Tuple<ExpressionWrapperBase, dynamic>> Mocks = new List<Tuple<ExpressionWrapperBase, dynamic>>();

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
                    var rule = Rules.Repository.Registered[typeof(TEntity)];
                    _FirstViolation = rule.ValidateEntity(Entity);
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
                    var rule = Rules.Repository.Registered[typeof(TEntity)];
                    _AllViolations = rule.FullyValidateEntity(Entity);
                }

                return _AllViolations;
            }
        }

        public IValidateResult<TEntity> WithMockedParameter<TParameter>(Expression<Func<TEntity, TParameter>> property, TParameter value)
        {
            // invalidate previous result
            ResetResult();
            Mocks.Add(new Tuple<ExpressionWrapperBase,dynamic>(new FuncExpressionWrapper<TEntity, TParameter>(property), value));
            return this;
        }

        public void SetAllMocks()
        {
            throw new NotImplementedException();
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