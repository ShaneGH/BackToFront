using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

using BackToFront.Framework.Base;
using BackToFront.Logic;

namespace BackToFront.Framework.Base
{
    internal abstract class ModelViolation<TEntity> : ExpressionElement<TEntity>, IModelViolation1<TEntity>, IModelViolation2<TEntity>
    {
        public ModelViolation(Expression<Func<TEntity, object>> descriptor, Rule<TEntity> rule)
            : base(descriptor, rule)
        {
        }

        protected ThrowViolation<TEntity> Violation;
        private IAdditionalRuleCondition<TEntity> AddViolation(IViolation violation)
        {
            Do(() => { Violation = new ThrowViolation<TEntity>(violation, ParentRule); });
            return ParentRule;
        }

        public IAdditionalRuleCondition<TEntity> ModelViolationIs(IViolation violation)
        {
            return AddViolation(violation);
        }

        public IAdditionalRuleCondition<TEntity> OrModelViolationIs(IViolation violation)
        {
            return AddViolation(violation);
        }
    }
}