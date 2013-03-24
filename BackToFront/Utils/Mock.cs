
using BackToFront.Enum;
using BackToFront.Expressions;
using System;
using System.Linq;
using System.Collections.Generic;
using E = System.Linq.Expressions;

using BackToFront.Extensions.IEnumerable;

namespace BackToFront.Utils
{
    public class Mocks : IEnumerable<Mock>
    {
        public readonly E.ParameterExpression Parameter;
        readonly Dictionary<Mock, E.UnaryExpression> Params = new Dictionary<Mock, E.UnaryExpression>();

        private readonly Mock[] _Mocks;

        public Mocks()
            : this(Enumerable.Empty<Mock>()) { }

        public Mocks(IEnumerable<Mock> mocks)
        {
            _Mocks = mocks.ToArray();
            Parameter = E.Expression.Parameter(typeof(object[]));
        }

        public IEnumerator<Mock> GetEnumerator()
        {
            return ((IEnumerable<Mock>)_Mocks).GetEnumerator();
        }

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return _Mocks.GetEnumerator();
        }

        public E.UnaryExpression ParameterForMock(Mock mock)
        {
            if (!Params.ContainsKey(mock))
            {
                var i = _Mocks.IndexOf(mock);
                if (i == -1)
                    throw new InvalidOperationException("##");

                Params.Add(mock, E.Expression.Convert(E.Expression.ArrayIndex(Parameter, E.Expression.Constant(i)), mock.ValueType));
            }

            return Params[mock];
        }

        public object[] AsValueArray
        {
            get
            {
                return _Mocks.Select(m => m.Value).ToArray();
            }
        }
    }

    public class Mock
    {
        public readonly MockBehavior Behavior;
        public readonly ExpressionWrapperBase Expression;
        //public readonly E.ParameterExpression Parameter;
        public readonly Type ValueType;
        public readonly object Value;

        public Mock(E.Expression expression, object value, Type valueType, MockBehavior behavior)
            : this(ExpressionWrapperBase.CreateChildWrapper(expression), value, valueType, behavior)
        {
        }

        public Mock(E.Expression expression, object value, Type valueType)
            : this(ExpressionWrapperBase.CreateChildWrapper(expression), value, valueType)
        {
        }

        public Mock(ExpressionWrapperBase wrapperExpression, object value, Type valueType, MockBehavior behavior)
        {
            Expression = wrapperExpression;
            Behavior = behavior;
            Value = value;
            ValueType = valueType;
            //Parameter = E.Expression.Parameter(valueType);
        }

        public Mock(ExpressionWrapperBase wrapperExpression, object value, Type valueType)
            : this(wrapperExpression, value, valueType, MockBehavior.MockAndSet)
        {
        }

        public static Mock Create<TEntity, TReturnVal>(E.Expression<Func<TEntity, TReturnVal>> expression, object value, Type valueType)
        {
            return new Mock(ExpressionWrapperBase.ToWrapper(expression), value, valueType);
        }

        public bool CanSet
        {
            get
            {
                return Expression is IPropertyChain && (Expression as IPropertyChain).CanSet;
            }
        }

        internal void SetValue(object root)
        {
            if (!CanSet)
                throw new InvalidOperationException("##");

            (Expression as IPropertyChain).Set(root, Value);
        }
    }
}
