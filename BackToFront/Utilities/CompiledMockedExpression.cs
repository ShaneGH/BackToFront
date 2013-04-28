using System;
using System.Collections.Generic;
using System.Linq;

namespace BackToFront.Utilities
{
    public class CompiledMockedExpression<TEntity, TMember>
    {
        private readonly Utilities.Mocks _Mocks;
        private readonly Func<TEntity, object[], IDictionary<string, object>, TMember> _Function;

        public CompiledMockedExpression(Func<TEntity, object[], IDictionary<string, object>, TMember> function, Utilities.Mocks mocks)
        {
            _Function = function;
            _Mocks = mocks;
        }

        public TMember Invoke(TEntity entity, object[] mockedValues, IDictionary<string, object> dependencies)
        {
            if (mockedValues == null)
                mockedValues = new object[0];

            // TODO: check each type and compile some very descriptive exception details
            if (_Mocks.Count() != mockedValues.Length)
                throw new InvalidOperationException("##");

            return _Function(entity, mockedValues ?? new object[0], dependencies ?? new Dictionary<string, object>());
        }
    }
}
