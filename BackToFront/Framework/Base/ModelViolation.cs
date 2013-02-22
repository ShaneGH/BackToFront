using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using BackToFront.Framework.Base;
using BackToFront.Logic;
using BackToFront.Logic.Base;

namespace BackToFront.Framework.Base
{
    internal abstract class ModelViolation<TEntity> : PathElement<TEntity>, IModelViolation1<TEntity>, IModelViolation2<TEntity>
    {
        public ModelViolation(Func<TEntity, object> descriptor, Rule<TEntity> rule)
            : base(descriptor, rule)
        {
        }

        protected ThrowViolation<TEntity> Violation;
        private IRule<TEntity> AddViolation(IViolation violation)
        {
            Do(() => { Violation = new ThrowViolation<TEntity>(violation, ParentRule); });
            return ParentRule;
        }

        public IRule<TEntity> ModelViolationIs(IViolation violation)
        {
            return AddViolation(violation);
        }

        public IRule<TEntity> OrModelViolationIs(IViolation violation)
        {
            return AddViolation(violation);
        }
    }
}