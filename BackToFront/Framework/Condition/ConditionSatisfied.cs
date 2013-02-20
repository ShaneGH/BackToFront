﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using BackToFront.Framework.Base;
using BackToFront.Framework.Requirement;
using BackToFront.Logic;
using BackToFront.Logic.Base;

using BackToFront.Logic.Compilations;

namespace BackToFront.Framework.Condition
{
    internal class ConditionSatisfied<TEntity> : ModelViolation<TEntity>, IConditionSatisfied<TEntity>
    {
        private readonly Operators<TEntity> ParentIf;

        protected override IEnumerable<IPathElement<TEntity>> NextPathElements
        {
            get
            {
                yield return Violation;
                //yield return _Then;
                yield return _RequireThat;
            }
        }

        public ConditionSatisfied(Func<TEntity, object> property, Rule<TEntity> rule, Operators<TEntity> operators)
            : base(property, rule)
        {
            ParentIf = operators;
        }

        public IOperators<TEntity> And(Func<TEntity, object> value)
        {
            return ParentIf.AddIf(value);
        }

        public IOperators<TEntity> Or(Func<TEntity, object> value)
        {
            return ParentIf.OrIf(value);
        }

        private RequireOperators<TEntity> _RequireThat = null;
        public IRequireOperators<TEntity> RequireThat(Func<TEntity, object> property)
        {
            return Do(() => _RequireThat = new RequireOperators<TEntity>(property, ParentRule));
        }

        private IRule<TEntity> _Then = null;
        public IRule<TEntity> Then(Action<ISubRule<TEntity>> action)
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