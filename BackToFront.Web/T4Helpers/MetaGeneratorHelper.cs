using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace BackToFront.Web.T4Helpers
{
    public static class MetaGeneratorHelper
    {
        public static string TypeNameFor(this IDictionary<Type, string> registeredTypes, MemberInfo member)
        {
            Type type = null;
            if (member is MethodInfo)
                type = (member as MethodInfo).ReturnType;

            if (member is PropertyInfo)
                type = (member as PropertyInfo).PropertyType;

            if (member is FieldInfo)
                type = (member as FieldInfo).FieldType;

            if(type == null)
                throw new InvalidOperationException("Invalid member type");

            if(!registeredTypes.ContainsKey(type))
                throw new InvalidOperationException("Invalid type: " + type.FullName);

            return registeredTypes[type];
        }
    }
}
