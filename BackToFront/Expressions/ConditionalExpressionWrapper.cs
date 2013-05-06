﻿using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq.Expressions;
using BackToFront.Enum;
using BackToFront.Meta;
using BackToFront.Utilities;
using System.Runtime.Serialization;
using BackToFront.Dependency;
using BackToFront.Expressions.Visitors;
using System.Linq;

using BackToFront.Extensions.IEnumerable;

namespace BackToFront.Expressions
{
    public class ConditionalExpressionWrapper : ExpressionWrapperBase<ConditionalExpression>
    {
        private ExpressionWrapperBase _Test;
        private ExpressionWrapperBase Test
        {
            get
            {
                return _Test ?? (_Test = ExpressionWrapperBase.CreateChildWrapper(Expression.Test));
            }
        }

        private ExpressionWrapperBase _IfTrue;
        private ExpressionWrapperBase IfTrue
        {
            get
            {
                return _IfTrue ?? (_IfTrue = ExpressionWrapperBase.CreateChildWrapper(Expression.IfTrue));
            }
        }

        private ExpressionWrapperBase _IfFalse;
        private ExpressionWrapperBase IfFalse
        {
            get
            {
                return _IfFalse ?? (_IfFalse = ExpressionWrapperBase.CreateChildWrapper(Expression.IfFalse));
            }
        }

        public ConditionalExpressionWrapper(ConditionalExpression expression)
            : base(expression)
        {
        }

        protected override IEnumerable<MemberChainItem> _GetMembersForParameter(ParameterExpression parameter)
        {
            return Test.GetMembersForParameter(parameter).Union(IfTrue.GetMembersForParameter(parameter)).Union(IfFalse.GetMembersForParameter(parameter));
        }

        public override ExpressionElementMeta Meta
        {
            get { throw new NotImplementedException(); }
        }

        protected override IEnumerable<ParameterExpression> _UnorderedParameters
        {
            get 
            {
                return Test.UnorderedParameters.Union(IfTrue.UnorderedParameters).Union(IfFalse.UnorderedParameters);
            }
        }

        public override bool IsSameExpression(Expression expression)
        {
            if (!base.IsSameExpression(expression))
                return false;

            var ex = expression as ConditionalExpression;
            if (ex == null)
                return false;

            return Test.IsSameExpression(ex.Test) &&
                IfTrue.IsSameExpression(ex.IfTrue) && 
                IfFalse.IsSameExpression(ex.IfFalse);
        }
    }
}