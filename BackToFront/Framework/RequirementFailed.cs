﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using BackToFront.Enum;
using BackToFront.Expressions;
using BackToFront.Extensions.IEnumerable;
using BackToFront.Framework.Base;
using BackToFront.Framework.Meta;
using BackToFront.Logic;
using BackToFront.Logic.Compilations;
using BackToFront.Utilities;

namespace BackToFront.Framework
{
    public class RequirementFailed<TEntity> : ExpressionElement<TEntity, bool>, IModelViolation<TEntity>
    {
        public override IEnumerable<PathElement<TEntity>> NextPathElements(TEntity subject, ValidationContext context)
        {
            return NextPathElements();
        }

        public IEnumerable<PathElement<TEntity>> NextPathElements()
        {
            yield return Violation;
        }

        public RequirementFailed(Expression<Func<TEntity, bool>> property, Rule<TEntity> rule)
            : base(property, rule)
        {
        }

        protected ThrowViolation<TEntity> Violation;
        public IAdditionalRuleCondition<TEntity> WithModelViolation(Func<IViolation> violation)
        {
            return WithModelViolation(a => violation());
        }

        public IAdditionalRuleCondition<TEntity> WithModelViolation(string violation)
        {
            return WithModelViolation(a => new SimpleViolation(violation));
        }

        public IAdditionalRuleCondition<TEntity> WithModelViolation(Func<TEntity, IViolation> violation)
        {
            Do(() => { Violation = new ThrowViolation<TEntity>(violation, ParentRule); });
            return ParentRule;
        }

        public override IViolation ValidateEntity(TEntity subject, ValidationContext context)
        {
            if (!Compile(context.Mocks).Invoke(subject, context.Mocks.AsValueArray))
            {
                context.ViolatedMembers.AddRange(AffectedMembers.Select(am => am.Member));
                return base.ValidateEntity(subject, context);
            }
            else
                return null;
        }

        public override void FullyValidateEntity(TEntity subject, IList<IViolation> violationList, ValidationContext context)
        {
            if (!Compile(context.Mocks).Invoke(subject, context.Mocks.AsValueArray))
            {
                context.ViolatedMembers.AddRange(AffectedMembers.Select(am => am.Member));
                base.FullyValidateEntity(subject, violationList, context);
            }
        }

        public override bool PropertyRequirement
        {
            get { return false; }
        }

        private MetaData _Meta;
        public override IMetaElement Meta
        {
            get { return _Meta ?? (_Meta = new MetaData(this)); }
        }

        private class MetaData : IMetaElement
        {
            private readonly RequirementFailed<TEntity> _Owner;

            public MetaData(RequirementFailed<TEntity> owner)
            {
                _Owner = owner;
            }

            public IEnumerable<IMetaElement> Children
            {
                get
                {
                    return _Owner.NextPathElements().Where(a => a != null).Select(a => a.Meta);
                }
            }

            public ExpressionWrapperBase Code
            {
                get { return _Owner.Descriptor; }
            }

            public PathElementType Type
            {
                get { return PathElementType.RequirementFailed; }
            }
        }
    }
}
