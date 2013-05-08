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
        private ExpressionWrapperBase Left
        {
            get
            {
                return _Left ?? (_Left = CreateChildWrapper(Expression.Left));
            }
        }

        private ExpressionWrapperBase _Right;
        private ExpressionWrapperBase Right
        {
            get
            {
                return _Right ?? (_Right = CreateChildWrapper(Expression.Right));
            }
        }

        public BinaryExpressionWrapper(BinaryExpression expression)
            : base(expression)
        {
        }

        public override bool IsSameExpression(BinaryExpression ex)
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

        private ExpressionElementMeta _Meta;
        public override ExpressionElementMeta Meta
        {
            get
            {
                return _Meta ?? (_Meta = new ExpressionElementMeta(Expression.NodeType.ToString(), new[] { Left.Meta, Right.Meta }, ExpressionWrapperType.Binary, Expression.Type, null));
            }
        }
    }
}
