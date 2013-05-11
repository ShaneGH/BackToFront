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
        public ExpressionWrapperBase Test
        {
            get
            {
                return _Test ?? (_Test = ExpressionWrapperBase.CreateChildWrapper(Expression.Test));
            }
        }

        private ExpressionWrapperBase _IfTrue;
        public ExpressionWrapperBase IfTrue
        {
            get
            {
                return _IfTrue ?? (_IfTrue = ExpressionWrapperBase.CreateChildWrapper(Expression.IfTrue));
            }
        }

        private ExpressionWrapperBase _IfFalse;
        public ExpressionWrapperBase IfFalse
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

        private ConditionalExpressionMeta _Meta;
        public override ExpressionMeta Meta
        {
            get
            {
                return _Meta ?? (_Meta = new ConditionalExpressionMeta(this));
            }
        }

        protected override IEnumerable<ParameterExpression> _UnorderedParameters
        {
            get 
            {
                return Test.UnorderedParameters.Union(IfTrue.UnorderedParameters).Union(IfFalse.UnorderedParameters);
            }
        }

        public override bool IsSameExpression(ConditionalExpression expression)
        {
            return Test.IsSameExpression(expression.Test) &&
                IfTrue.IsSameExpression(expression.IfTrue) && 
                IfFalse.IsSameExpression(expression.IfFalse);
        }
    }
}
