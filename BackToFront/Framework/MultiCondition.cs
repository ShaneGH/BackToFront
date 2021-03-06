﻿using BackToFront.Extensions.IEnumerable;
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
        private readonly IList<Condition> _If = new List<Condition>();
        public IEnumerable<Condition> If
        {
            get
            {
                return _If.ToArray();
            }
        }

        public MultiCondition(Rule<TEntity> rule)
            : base(rule) { }

        public override IEnumerable<PathElement<TEntity>> AllPossiblePaths
        {
            get { return _If.Select(a => a.Action).ToArray(); }
        }

        public RequireOperator<TEntity> Add(Expression<Func<TEntity, bool>> condition)
        {
            var _if = new Condition(condition, ParentRule);
            _If.Add(_if);
            return _if.Action;
        }

        protected override Expression _Compile(ExpressionMocker visitor)
        {
            Expression final = null;
            var possibilities = _If.Select(a => 
            {
                using (visitor.WithEntityParameter(a.EntityParameter))
                    return new Tuple<Expression, Expression>(visitor.Mock(a.Descriptor.WrappedExpression), a.Action.Compile(visitor));
            }).ToArray();

            if (possibilities.Any())
            {
                final = Expression.IfThen(possibilities.Last().Item1, possibilities.Last().Item2);
                possibilities.Reverse().Skip(1).Each(a => final = Expression.IfThenElse(a.Item1, a.Item2, final));
            }

            return final ?? Expression.Empty();
        }
    }
}