using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Threading.Tasks;

using BackToFront.Extensions.Expressions;

namespace BackToFront.Utils
{
    public abstract class PropertySetterBase<TProp>
    {
        TProp Value;
        public abstract MemberExpression Pointer { get; }
    }

    public class PropertySetter<TClass, TProp> : PropertySetterBase<TProp>
    {
        public readonly MemberExpression Property;
        public PropertySetter(Expression<Func<TClass, TProp>> property)
        {
            if (property.Body is MemberExpression && property.ReferencedProperties().All(a => a is PropertyInfo || a is FieldInfo))
            {
                Property = property.Body as MemberExpression;
            }
            else
            {
                throw new InvalidOperationException("##9");
            }
        }

        public override MemberExpression Pointer
        {
            get { return Property; }
        }
    }
}
