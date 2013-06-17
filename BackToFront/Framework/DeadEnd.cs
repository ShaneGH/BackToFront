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

        public override IEnumerable<PathElement<TEntity>> AllPossiblePaths
        {
            get { yield break; }
        }

        protected override Expression _Compile(Expressions.Visitors.SwapPropVisitor visitor)
        {
            return Expression.Empty();
        }
    }
}
