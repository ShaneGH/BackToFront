using BackToFront.Extensions.IEnumerable;
using BackToFront.Framework.Base;
using BackToFront.Utilities;
using System.Collections.Generic;
using System.Linq;
using BackToFront.Meta;
using BackToFront.Expressions;
using BackToFront.Enum;
using System.Runtime.Serialization;
using System;
using BackToFront.Expressions.Visitors;

namespace BackToFront.Framework
{
    /// <summary>
    /// Describes if, else if, else logic
    /// </summary>
    /// <typeparam name="TEntity"></typeparam>
    public class MultiCondition<TEntity> : PathElement<TEntity>
    {
        public readonly IList<Operator<TEntity>> If = new List<Operator<TEntity>>();

        public MultiCondition(Rule<TEntity> rule)
            : base(rule) { }

        public override IEnumerable<PathElement<TEntity>> AllPossiblePaths
        {
            get { return If.ToArray(); }
        }

        public override IEnumerable<AffectedMembers> AffectedMembers
        {
            get
            {
                return If.Select(i => i.AffectedMembers).Aggregate();
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
                return _Meta ?? (_Meta = new PathElementMeta(If.Select(i => i.Meta), null, PathElementType.MultiCondition));
            }
        }

        protected override Action<TEntity, ValidationContextX> _NewCompile(SwapPropVisitor visitor)
        {
            var ifs = If.Select(a => new Tuple<CompiledMockedExpression<TEntity, bool>, Action<TEntity, ValidationContextX>>(a.Compile(visitor), a.NewCompile(visitor))).ToArray();

            return (a, b) =>
            {
                foreach (var i in ifs)
                {
                    if (i.Item1.Invoke(a, b.Mocks, b.Dependencies))
                    {
                        i.Item2(a, b);
                        break;
                    }
                }
            };
        }
    }
}