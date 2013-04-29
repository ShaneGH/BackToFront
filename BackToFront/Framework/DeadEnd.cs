using System.Collections.Generic;
using System.Linq;
using BackToFront.Enum;
using BackToFront.Framework.Base;
using BackToFront.Meta;
using BackToFront.Utilities;
using System.Runtime.Serialization;
using System;
using System.Linq.Expressions;

namespace BackToFront.Framework
{
    internal class DeadEnd<TEntity> : PathElement<TEntity>
    {
        public DeadEnd()
            : base(null) { }

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

        protected override Expression _NewCompile(Expressions.Visitors.SwapPropVisitor visitor)
        {
            return Expression.Empty();
        }
    }
}
