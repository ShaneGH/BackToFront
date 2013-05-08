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
using System.Linq.Expressions;
using System.Collections.ObjectModel;

namespace BackToFront.Framework
{
    /// <summary>
    /// Describes if, else if, else logic
    /// </summary>
    /// <typeparam name="TEntity"></typeparam>
    public partial class MultiCondition<TEntity> : PathElement<TEntity>
    {
        public class Condition : ExpressionElement<TEntity, bool>
        {
            public readonly RequireOperator<TEntity> Action;
            public Condition(Expression<Func<TEntity, bool>> expression, Rule<TEntity> rule)
                : base(expression, rule) 
            {
                Action = new RequireOperator<TEntity>(ParentRule);

                // lock element
                Do(() => { });
            }

            public override bool PropertyRequirement
            {
                get { return false; }
            }

            public override IEnumerable<PathElement<TEntity>> AllPossiblePaths
            {
                get { yield return Action; }
            }

            public override PathElementMeta Meta
            {
                get { throw new NotImplementedException(); }
            }

            /// <summary>
            /// Is not meant to be compiled
            /// </summary>
            /// <param name="visitor"></param>
            /// <returns>Expression.Empty()</returns>
            protected override Expression _Compile(SwapPropVisitor visitor)
            {
                return Expression.Empty();
            }
        }
    }
}