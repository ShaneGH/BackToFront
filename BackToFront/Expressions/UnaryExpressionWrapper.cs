using System.Collections.Generic;
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

        public override bool IsSameExpression(Expression expression)
        {
            if (!base.IsSameExpression(expression))
                return false;

            var ex = expression as UnaryExpression;
            if (ex == null)
                return false;

            return ex.Method == Expression.Method &&
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

        private ExpressionElementMeta _Meta;
        public override ExpressionElementMeta Meta
        {
            get
            {
                return _Meta ?? (_Meta = new ExpressionElementMeta(Expression.NodeType.ToString(), new ExpressionElementMeta[0], ExpressionWrapperType.Unary, Expression.Type, Operand.Meta));
            }
        }
    }
}
