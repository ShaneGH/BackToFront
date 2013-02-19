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
    internal class ModelViolation1<TEntity> : PathElement<TEntity>, IModelViolation1<TEntity>
    {
        private readonly Operators<TEntity> ParentIf;

        protected override IEnumerable<IPathElement<TEntity>> NextPathElements
        {
            get
            {
                yield return _ModelViolationIs;
                //yield return _Then;
                //yield return _RequireThat;
            }
        }

        public ModelViolation1(Func<TEntity, object> property, Rule<TEntity> rule, Operators<TEntity> condition)
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

        private IRequireOperators<TEntity> _RequireThat = null;
        public IRequireOperators<TEntity> RequireThat(Func<TEntity, object> property)
        {
            throw new NotImplementedException();
        }

        public override IViolation ValidateEntity(TEntity subject)
        {
            return ValidateNext(subject);
        }

        public override void FullyValidateEntity(TEntity subject, IList<IViolation> violationList)
        {
            ValidateAllNext(subject, violationList);
        }
    }
}
