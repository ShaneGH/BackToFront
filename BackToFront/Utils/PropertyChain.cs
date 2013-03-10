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

    static class MemberExtensions
    {
        public static object Get(this MemberInfo member, object subject)
        {
            if (member is PropertyInfo)
                return (member as PropertyInfo).GetValue(subject);
            else if (member is FieldInfo)
                return (member as FieldInfo).GetValue(subject);

            throw new EX("##");
        }

        public static void Set(this MemberInfo member, object subject, object value)
        {
            if (member is PropertyInfo)
                (member as PropertyInfo).SetValue(subject, value);
            else if (member is FieldInfo)
                (member as FieldInfo).SetValue(subject, value);
            else
            throw new EX("##");
        }
    }

    public class PropertyChain<TEntity>
    {

        public readonly IEnumerable<MemberInfo> Members;
        public PropertyChain(Expression<Func<TEntity, object>> expression)
            : this(expression.ReferencedProperties())
        {
        }

        public PropertyChain(MemberExpression expression)
            : this(expression.ReferencedProperties())
        {
        }

        public PropertyChain(IEnumerable<MemberInfo> members)
        {
            Members = members.ToArray();
            if (!Members.All(m =>
                m.GetCustomAttribute(typeof(AffectsMembersAttribute)) == null &&
                (m is PropertyInfo || m is FieldInfo)))
                throw new EX("##7");

            for (int i = 0, ii = Members.Count() - 1; i < ii; i++)
            {
                // TestMembers (above) will make sure no exception is thrown
                var type = Members.ElementAt(i) is FieldInfo ? (Members.ElementAt(i) as FieldInfo).FieldType : (Members.ElementAt(i) as PropertyInfo).PropertyType;
                if (type != Members.ElementAt(i + 1).DeclaringType)
                {
                    throw new EX("##8");
                }
            }
        }

        public object Get(TEntity subject)
        {
            string message;
            object result;
            if (!string.IsNullOrEmpty(message = _TryGet(subject, out result)))
                // "##"
                throw new EX(string.Format("Member {0} was null.", message));

            return result;
        }

        public bool TryGet(TEntity subject, out object result)
        {
            string message = _TryGet(subject, out result);
            return string.IsNullOrEmpty(message);
        }

        public string _TryGet(TEntity subject, out object result)
        {
            result = subject;
            var members = Members.ToArray();
            for (int i = 0, ii = members.Length; i < ii; i++)
            {
                // TODO: full path description
                if (result == null)
                {
                    return i == 0 ? "root" : members[i - 1].Name;
                }

                result = members[i].Get(result);
            }

            return null;
        }

        public void Set(TEntity subject, object value)
        {
            string result;
            if (!string.IsNullOrEmpty(result = _TrySet(subject, value)))
                // "##"
                throw new EX(string.Format("Member {0} was null.", result));
        }

        public bool TrySet(TEntity subject, object value)
        {
            string message = _TrySet(subject, value);
            return string.IsNullOrEmpty(message);
        }

        public string _TrySet(TEntity subject, object value)
        {
            object result = subject;
            var members = Members.ToArray();

            int i, ii;
            for (i = 0, ii = members.Length - 1; i < ii; i++)
            {
                if (result == null)
                    break;

                result = members[i].Get(result);
            }

            // TODO: full path description
            if (result == null || i != ii)
            {
                return i == 0 ? "root" : members[i - 1].Name;
            }
            else
            {
                members[i].Set(result, value);
                return null;
            }
        }

        #region Equality

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

        #endregion
    }
}
