﻿using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq.Expressions;
using E = System.Linq.Expressions;
using BackToFront.Utilities;
using System;
using BackToFront.Meta;
using BackToFront.Enum;
using System.Runtime.Serialization;

namespace BackToFront.Expressions
{
    // TODO: test
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

        public override bool IsSameExpression(ExpressionWrapperBase expression)
        {
            if (!base.IsSameExpression(expression))
                return false;

            var ex = expression as UnaryExpressionWrapper;
            if (ex == null)
                return false;

            return ex.Expression.Method == Expression.Method &&
                Operand.IsSameExpression(ex.Operand);
        }

        protected override Expression CompileInnerExpression(Mocks mocks)
        {
            var result = Operand.Compile(mocks);

            return result == Operand.WrappedExpression ? Expression :
                E.Expression.MakeUnary(Expression.NodeType, result, Expression.Type, Expression.Method);
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

        private MetaData _Meta;
        public override ExpressionElementMeta Meta 
        {
            get 
            {
                return _Meta ?? (_Meta = new MetaData(this));
            }
        }

        [DataContract]
        private class MetaData : ExpressionElementMeta
        {
            private readonly UnaryExpressionWrapper _Owner;

            public MetaData(UnaryExpressionWrapper owner)
            {
                _Owner = owner;
            }

            public override object Descriptor
            {
                get { return _Owner.Expression.NodeType.ToString(); }
            }

            public override IEnumerable<ExpressionElementMeta> Elements
            {
                get { yield break; }
            }

            public override ExpressionWrapperType ExpressionType
            {
                get { return ExpressionWrapperType.Unary; }
            }

            public override Type Type
            {
                get { return _Owner.Expression.Type; }
            }

            public override ExpressionElementMeta Base
            {
                get { return _Owner.Operand.Meta; }
            }
        }
    }
}
