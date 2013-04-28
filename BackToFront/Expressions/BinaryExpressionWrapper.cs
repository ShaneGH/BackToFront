﻿using System;
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

        public override bool IsSameExpression(Expression expression)
        {
            if (!base.IsSameExpression(expression))
                return false;

            var ex = expression as BinaryExpression;
            if (ex == null)
                return false;

            return Expression.NodeType == ex.NodeType &&
                Expression.Method == ex.Method &&
                Left.IsSameExpression(ex.Left) &&
                Right.IsSameExpression(ex.Right);                
        }

        protected override Expression CompileInnerExpression(ISwapPropVisitor mocks)
        {
            Expression lhs = Left.Compile(mocks);
            Expression rhs = Right.Compile(mocks);

            if (lhs == Left.WrappedExpression && rhs == Right.WrappedExpression)
                return Expression;

            return E.Expression.MakeBinary(Expression.NodeType, lhs, rhs, Expression.IsLiftedToNull, Expression.Method, Expression.Conversion);
        }

        protected override IEnumerable<MemberChainItem> _GetMembersForParameter(ParameterExpression parameter)
        {
            foreach (var item in Left.GetMembersForParameter(parameter))
                yield return item;

            foreach (var item in Right.GetMembersForParameter(parameter))
                yield return item;
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
