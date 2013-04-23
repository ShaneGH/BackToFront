using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using BackToFront.Enum;
using BackToFront.Expressions;
using BackToFront.Framework.Base;
using BackToFront.Framework.Meta;
using BackToFront.Utilities;

namespace BackToFront.Framework
{
    /// <summary>
    /// End of a pathway, Throw violation
    /// </summary>
    /// <typeparam name="TEntity"></typeparam>
    public class ThrowViolation<TEntity> : PathElement<TEntity>
    {
        private readonly Func<TEntity, IViolation> _violation;
        public ThrowViolation(Func<TEntity, IViolation> violation, Rule<TEntity> parentRule)
            : base(parentRule)
        {
            if(violation == null)
                throw new ArgumentNullException("##6");

            _violation = violation;
        }

        public IEnumerable<PathElement<TEntity>> NextPathElements()
        {
            yield break;
        }

        public override IEnumerable<PathElement<TEntity>> NextPathElements(TEntity subject, ValidationContext context)
        {
            return NextPathElements();
        }

        public override IViolation ValidateEntity(TEntity subject, ValidationContext context)
        {
            var violation = _violation(subject);
            violation.ViolatedEntity = subject;
            return violation;
        }

        public override void FullyValidateEntity(TEntity subject, IList<IViolation> violationList, ValidationContext context)
        {
            var violation = ValidateEntity(subject, context);
            if (violation != null)
                violationList.Add(violation);
        }

        public override IEnumerable<AffectedMembers> AffectedMembers
        {
            get
            {
                yield break;
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
            private readonly ThrowViolation<TEntity> _Owner;

            public MetaData(ThrowViolation<TEntity> owner)
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
                get { return null; }
            }

            public PathElementType Type
            {
                get { return PathElementType.ThrowViolation; }
            }
        }
    }
}
