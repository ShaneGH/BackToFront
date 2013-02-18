using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using BackToFront.Framework.Base;
using BackToFront.Logic;

namespace BackToFront.Framework
{
    internal class Operator<TEntity, TViolation> : OperatorBase<TEntity, TViolation>, IOperator<TEntity, TViolation>
        where TViolation : IViolation
    {
        private readonly If<TEntity, TViolation> ParentIf;

        protected override IEnumerable<IValidatablePathElement<TEntity>> NextPathElement
        {
            get
            {
                yield return _ModelViolationIs;
                //yield return _Then;
                //yield return _RequireThat;
            }
        }

        public Operator(Func<TEntity, object> property, Rule<TEntity, TViolation> rule, If<TEntity, TViolation> condition)
            : base(property, rule)
        {
            ParentIf = condition;
        }

        public IOperators<TEntity, TViolation> And(Func<TEntity, object> value)
        {
            return ParentIf.AddIf(value);
        }

        public IOperators<TEntity, TViolation> Or(Func<TEntity, object> value)
        {
            return ParentIf.OrIf(value);
        }

        private ThrowViolation<TEntity> _ModelViolationIs;
        public IRule<TEntity, TViolation> ModelViolationIs(TViolation violation)
        {
            Do(() => { _ModelViolationIs = new ThrowViolation<TEntity>(violation); });
            return ParentRule;
        }

        private IRule<TEntity, TViolation> _Then = null;
        public IRule<TEntity, TViolation> Then(Action<ISubRule<TEntity, TViolation>> action)
        {
            throw new NotImplementedException();
        }

        private IRequirement<TEntity, TViolation> _RequireThat = null;
        public IRequirement<TEntity, TViolation> RequireThat(Func<TEntity, object> property)
        {
            throw new NotImplementedException();
        }
    }
}
