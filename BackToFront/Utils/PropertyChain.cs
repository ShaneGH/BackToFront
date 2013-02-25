using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Threading.Tasks;

using BackToFront.Extensions.Expressions;
using BackToFront.Attributes;

namespace BackToFront.Utils
{
    public class EX : Exception { public EX(string message) : base(message) { } }

    public class PropertyChain<TEntity>
    {
        public readonly IEnumerable<MemberInfo> Members;
        public PropertyChain(Expression<Func<TEntity, object>> expression)
            : this(expression.ReferencedProperties())
        {
        }

        public PropertyChain(IEnumerable<MemberInfo> members)
        {
            Members = members.ToArray();
            if (!Members.All(m => 
                m.GetCustomAttribute(typeof(AffectsMembersAttribute)) == null &&
                (m is PropertyInfo || m is FieldInfo)))
                throw new EX("##");

            for (int i = 0, ii = Members.Count() - 1; i < ii; i++)
            {
                // TestMembers (above) will make sure no exception is thrown
                var type = Members.ElementAt(i) is FieldInfo ? (Members.ElementAt(i) as FieldInfo).FieldType : (Members.ElementAt(i) as PropertyInfo).PropertyType;
                if (type != Members.ElementAt(i + 1).DeclaringType)
                {
                    throw new EX("##");
                }
            }
        }

        public override int GetHashCode()
        {
            return string.Join("_", Members.Select(m => m.GetHashCode().ToString())).GetHashCode();
        }

        public override bool Equals(object obj)
        {
            if (!base.Equals(obj))
            {
                var pChain = obj as PropertyChain<TEntity>;
                if (pChain == null || Members.Count() != pChain.Members.Count())
                    return false;

                for (int i = 0, ii = Members.Count(); i < ii; i++)
                {
                    if (Members.ElementAt(i) != pChain.Members.ElementAt(i))
                        return false;
                }
            }

            return true;
        }

        public static bool operator !=(PropertyChain<TEntity> c1, PropertyChain<TEntity> c2)
        {
            return !(c1 == c2);
        }

        public static bool operator ==(PropertyChain<TEntity> c1, PropertyChain<TEntity> c2)
        {
            try
            {
                return c1.Equals(c2);
            }
            catch (NullReferenceException)
            {
                return false;
            }
        }
    }
}
