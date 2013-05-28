using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq.Expressions;
using E = System.Linq.Expressions;
using BackToFront.Utilities;
using System;
using BackToFront.Meta;
using BackToFront.Enum;
using System.Runtime.Serialization;
using BackToFront.Expressions.Visitors;

namespace BackToFront.Expressions
{
    public class UnaryExpressionWrapper : ExpressionWrapperBase<UnaryExpression>, ILinearExpression
    {
        private ExpressionWrapperBase _Operand;
        public ExpressionWrapperBase Operand
        {
            get
            {
                return _Operand ?? (_Operand = CreateChildWrapper(Expression.Operand));
            }
        }

        public UnaryExpressionWrapper(UnaryExpression expression)
            : base(expression)
        {
        }

        protected override bool IsSameExpression(UnaryExpression expression)
        {
            return expression.Method == Expression.Method &&
                Operand.IsSameExpression(expression.Operand);
        }

        protected override IEnumerable<MemberChainItem> _GetMembersForParameter(ParameterExpression parameter)
        {
            return Operand.GetMembersForParameter(parameter);
        }

        protected override IEnumerable<ParameterExpression> _UnorderedParameters
        {
            get 
            {
                return Operand.UnorderedParameters;
            }
        }

        public ExpressionWrapperBase Root
        {
            get { return Operand; }
        }

        public ExpressionWrapperBase WithAlternateRoot<TEntity, TChild>(Expression root, Expression<Func<TEntity, TChild>> child)
        {
            return new UnaryExpressionWrapper(E.Expression.MakeUnary(Expression.NodeType, root, Expression.Type, Expression.Method));
        }
    }
}
