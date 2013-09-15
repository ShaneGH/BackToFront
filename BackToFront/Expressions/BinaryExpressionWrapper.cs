using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Linq.Expressions;
using E = System.Linq.Expressions;
using BackToFront.Utilities;
using System.Reflection;
using BackToFront.Meta;
using BackToFront.Enum;
using System.Runtime.Serialization;
using BackToFront.Expressions.Visitors;


namespace BackToFront.Expressions
{
    public class BinaryExpressionWrapper : ExpressionWrapperBase<BinaryExpression>
    {
        private ExpressionWrapperBase _Left;
        public ExpressionWrapperBase Left
        {
            get
            {
                return _Left ?? (_Left = CreateExpressionWrapper(Expression.Left));
            }
        }

        private ExpressionWrapperBase _Right;
        public ExpressionWrapperBase Right
        {
            get
            {
                return _Right ?? (_Right = CreateExpressionWrapper(Expression.Right));
            }
        }

        public BinaryExpressionWrapper(BinaryExpression expression)
            : base(expression)
        {
        }

        protected override bool _IsSameExpression(BinaryExpression ex)
        {
            return Expression.NodeType == ex.NodeType &&
                Expression.Method == ex.Method &&
                Left.IsSameExpression(ex.Left) &&
                Right.IsSameExpression(ex.Right);                
        }

        protected override IEnumerable<MemberChainItem> _GetMembersForParameter(ParameterExpression parameter)
        {
            return Left.GetMembersForParameter(parameter).Union(Right.GetMembersForParameter(parameter));
        }

        protected override IEnumerable<ParameterExpression> _UnorderedParameters
        {
            get 
            {
                return Left.UnorderedParameters.Union(Right.UnorderedParameters);
            }
        }
    }
}
