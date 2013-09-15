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

            public override IEnumerable<PathElement<TEntity>> AllPossiblePaths
            {
                get { yield return Action; }
            }

            /// <summary>
            /// Is not meant to be compiled
            /// </summary>
            /// <param name="visitor"></param>
            /// <returns>Expression.Empty()</returns>
            protected override Expression _Compile(ExpressionMocker visitor)
            {
                return Expression.Empty();
            }

            //TODO: cache???
            //TODO: Test
            public override IEnumerable<MemberChainItem> RequiredForValidation
            {
                get
                {
                    return base.RequiredForValidation.Union(Descriptor.GetMembersForParameter(EntityParameter));
                }
            }
        }
    }
}