using System.Collections.Generic;
using System.Linq;
using BackToFront.Enum;
using BackToFront.Framework.Base;
using BackToFront.Framework.Meta;
using BackToFront.Utilities;

namespace BackToFront.Framework
{
    internal class DeadEnd<TEntity> : PathElement<TEntity>
    {
        public DeadEnd()
            : base(null) { }

        public override IEnumerable<PathElement<TEntity>> NextPathElements(TEntity subject, ValidationContext context)
        {
            yield break;
        }

        public override IViolation ValidateEntity(TEntity subject, ValidationContext context)
        {
            return null;
        }

        public override void FullyValidateEntity(TEntity subject, IList<IViolation> violationList, ValidationContext context)
        {
            return;
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
            get { return _Meta ?? (_Meta = new MetaData()); }
        }

        private class MetaData : IMetaElement
        {
            public IEnumerable<IMetaElement> Children
            {
                get { return Enumerable.Empty<IMetaElement>(); }
            }

            public Expressions.ExpressionWrapperBase Code
            {
                get { return null; }
            }

            public PathElementType Type
            {
                get { return PathElementType.DeadEnd; }
            }
        }
    }
}
