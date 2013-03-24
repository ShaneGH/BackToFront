using System;
using System.Linq;

namespace BackToFront.Utils
{
    public class CompiledMockedExpression<TEntity, TMember>
    {
        private readonly Utils.Mocks _Mocks;
        private readonly Func<TEntity, object[], TMember> _Function;

        public CompiledMockedExpression(Func<TEntity, object[], TMember> function, Utils.Mocks mocks)
        {
            _Function = function;
            _Mocks = mocks;
        }

        public TMember Invoke(TEntity entity, params object[] mockedValues)
        {
            if (mockedValues == null)
                mockedValues = new object[0];

            // TODO: check each type and compile some very descriptive exception details
            if (_Mocks.Count() != mockedValues.Length)
                throw new InvalidOperationException("##");

            return _Function(entity, mockedValues);
        }
    }
}
