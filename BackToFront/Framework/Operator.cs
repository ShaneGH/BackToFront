using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using BackToFront.Framework.Base;
using BackToFront.Logic;
using BackToFront.Logic.Base;

namespace BackToFront.Framework
{
    internal class Operator<TEntity> : OperatorBase<TEntity>, IOperator<TEntity>
    {
        private readonly If<TEntity> ParentIf;

        protected override IEnumerable<IPathElement> NextPathElement
        {
            get
            {
                yield return _ModelViolationIs;
                //yield return _Then;
                //yield return _RequireThat;
            }
        }

        public Operator(Func<TEntity, object> property, Rule<TEntity> rule, If<TEntity> condition)
            : base(property, rule)
        {
            ParentIf = condition;
        }

        public IOperators<TEntity> And(Func<TEntity, object> value)
        {
            return ParentIf.AddIf(value);
        }

        public IOperators<TEntity> Or(Func<TEntity, object> value)
        {
            return ParentIf.OrIf(value);
        }

        private ThrowViolation<TEntity> _ModelViolationIs;
        public IRule<TEntity> ModelViolationIs(IViolation violation)
        {
            Do(() => { _ModelViolationIs = new ThrowViolation<TEntity>(violation); });
            return ParentRule;
        }

        private IRule<TEntity> _Then = null;
        public IRule<TEntity> Then(Action<ISubRule<TEntity>> action)
        {
            throw new NotImplementedException();
        }

        private IRequirement<TEntity> _RequireThat = null;
        public IRequirement<TEntity> RequireThat(Func<TEntity, object> property)
        {
            throw new NotImplementedException();
        }
    }
}
