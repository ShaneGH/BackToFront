using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BackToFront.Logic
{
    internal class RequireOperators<TEntity> : IRequireOperators<TEntity>
    {
        public IModelViolation2<TEntity> IsTrue()
        {
            throw new NotImplementedException();
        }

        public IModelViolation2<TEntity> IsFalse()
        {
            throw new NotImplementedException();
        }

        public IModelViolation2<TEntity> IsNull()
        {
            throw new NotImplementedException();
        }

        public IModelViolation2<TEntity> IsNotNull()
        {
            throw new NotImplementedException();
        }

        public IModelViolation2<TEntity> IsEqualTo(Func<TEntity, object> value)
        {
            throw new NotImplementedException();
        }

        public IModelViolation2<TEntity> IsEqualTo(object value)
        {
            throw new NotImplementedException();
        }

        public IModelViolation2<TEntity> IsNotEqualTo(Func<TEntity, object> value)
        {
            throw new NotImplementedException();
        }

        public IModelViolation2<TEntity> IsNotEqualTo(object value)
        {
            throw new NotImplementedException();
        }

        public IModelViolation2<TEntity> GreaterThan(Func<TEntity, IComparable> value)
        {
            throw new NotImplementedException();
        }

        public IModelViolation2<TEntity> GreaterThan(IComparable value)
        {
            throw new NotImplementedException();
        }

        public IModelViolation2<TEntity> LessThan(Func<TEntity, IComparable> value)
        {
            throw new NotImplementedException();
        }

        public IModelViolation2<TEntity> LessThan(IComparable value)
        {
            throw new NotImplementedException();
        }

        public IModelViolation2<TEntity> GreaterThanOrEqualTo(Func<TEntity, IComparable> value)
        {
            throw new NotImplementedException();
        }

        public IModelViolation2<TEntity> GreaterThanOrEqualTo(IComparable value)
        {
            throw new NotImplementedException();
        }

        public IModelViolation2<TEntity> LessThanOrEqualTo(Func<TEntity, IComparable> value)
        {
            throw new NotImplementedException();
        }

        public IModelViolation2<TEntity> LessThanOrEqualTo(IComparable value)
        {
            throw new NotImplementedException();
        }
    }
}
