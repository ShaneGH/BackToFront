using System.Collections.Generic;
using System.Linq;
using BackToFront.Enum;
using BackToFront.Framework.Base;
using BackToFront.Meta;
using BackToFront.Utilities;
using System.Runtime.Serialization;

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

        private PathElementMeta _Meta;
        public override PathElementMeta Meta
        {
            get
            {
                return _Meta ?? (_Meta = new PathElementMeta(Enumerable.Empty<PathElementMeta>(), null, PathElementType.DeadEnd));
            }
        }

        public override IEnumerable<PathElement<TEntity>> AllPossiblePaths
        {
            get { yield break; }
        }

        protected override System.Action<TEntity, ValidationContextX> _NewCompile(Expressions.Visitors.SwapPropVisitor visitor)
        {
            return DoNothing;
        }
    }
}
