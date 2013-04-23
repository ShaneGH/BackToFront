﻿using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Linq.Expressions;
using BackToFront.Enum;
using BackToFront.Extensions.Reflection;
using BackToFront.Meta;
using BackToFront.Utilities;
using System.Runtime.Serialization;

namespace BackToFront.Expressions
{
    public class ParameterExpressionWrapper : ExpressionWrapperBase<ParameterExpression>, IPropertyChainGetter
    {
        public ParameterExpressionWrapper(ParameterExpression expression)
            : base(expression)
        {
        }

        public override bool IsSameExpression(ExpressionWrapperBase expression)
        {
            if (!base.IsSameExpression(expression))
                return false;

            // TODO: is this correct?

            var ex = expression as ParameterExpressionWrapper;
            if (ex == null)
                return false;

            return ex.Expression.Type.Is(Expression.Type);
        }

        protected override Expression CompileInnerExpression(Mocks mocks)
        {
            return Expression;
        }

        public object Get(object root)
        {
            if (!root.GetType().Is(Expression.Type))
            {
                throw new InvalidOperationException("##");
            }

            return root;
        }

        public ExpressionWrapperBase Root
        {
            get { return null; }
        }

        public ExpressionWrapperBase WithAlternateRoot<TEntity, TChild>(Expression root, Expression<Func<TEntity, TChild>> child)
        {
            return new ParameterExpressionWrapper(Expression);
        }

        protected override IEnumerable<MemberChainItem> _GetMembersForParameter(ParameterExpression parameter)
        {
            if (parameter == Expression)
                yield return new MemberChainItem(parameter.Type);
        }

        protected override IEnumerable<ParameterExpression> _UnorderedParameters
        {
            get { yield return Expression; }
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
            private readonly ParameterExpressionWrapper _Owner;

            public MetaData(ParameterExpressionWrapper owner)
            {
                _Owner = owner;
            }

            public override object Descriptor
            {
                get { return _Owner.Expression.Name; }
            }

            public override IEnumerable<ExpressionElementMeta> Elements
            {
                get { yield break; }
            }

            public override ExpressionWrapperType ExpressionType
            {
                get { return ExpressionWrapperType.Parameter; }
            }

            public override Type Type
            {
                get { return _Owner.Expression.Type; }
            }

            public override ExpressionElementMeta Base
            {
                get { return null; }
            }
        }
    }
}
