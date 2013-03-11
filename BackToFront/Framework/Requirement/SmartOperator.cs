﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

using BackToFront.Framework.Base;
using BackToFront.Framework.Requirement;
using BackToFront.Logic;
using BackToFront.Logic.Compilations;

namespace BackToFront.Framework.Requirement
{
    internal class SmartRequireOperator<TEntity> : ModelViolation<TEntity>, CONDITION_IS_TRUE<TEntity>, ISmartConditionSatisfied<TEntity>
    {
        readonly Expression<Func<TEntity, bool>> IfCodition;

        public SmartRequireOperator(Expression<Func<TEntity, bool>> descriptor, Rule<TEntity> rule)
            : base(a => (object)descriptor.Compile()(a), rule)
        {
            IfCodition = descriptor;
        }

        protected override IEnumerable<PathElement<TEntity>> NextPathElements(TEntity subject)
        {
            yield return Violation;
            yield return _Then;
            yield return _SmartRequireThat;
        }

        public override void ValidateEntity(TEntity subject, out IViolation violation)
        {
            violation = ValidateNext(subject);
        }

        public override void FullyValidateEntity(TEntity subject, IList<IViolation> violationList)
        {
            ValidateAllNext(subject, violationList);
        }

        public bool ConditionIsTrue(TEntity subject)
        {
            return IfCodition.Compile()(subject);
        }

        private RequirementFailed<TEntity> _SmartRequireThat = null;
        public IModelViolation2<TEntity> SmartRequireThat(Expression<Func<TEntity, bool>> condition)
        {
            return Do(() => _SmartRequireThat = new RequirementFailed<TEntity>(a => condition.Compile()(a), ParentRule));
        }

        private SubRuleCollection<TEntity> _Then = null;
        public IAdditionalRuleCondition<TEntity> Then(Action<IRule<TEntity>> action)
        {
            Do(() => _Then = new SubRuleCollection<TEntity>(ParentRule));

            // run action on new sub rule
            action(_Then);

            // present rule to begin process again
            return ParentRule;
        }
    }
}
