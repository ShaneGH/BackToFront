
using BackToFront.Enum;
using BackToFront.Expressions;
using System;
using System.Linq;
using System.Collections.Generic;
using E = System.Linq.Expressions;

using BackToFront.Extensions.IEnumerable;

namespace BackToFront.Utilities
{
    public class Mocks : IEnumerable<Mock>
    {
        public readonly E.ParameterExpression Parameter;
        private readonly Dictionary<Mock, E.UnaryExpression> Params = new Dictionary<Mock, E.UnaryExpression>();

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
}