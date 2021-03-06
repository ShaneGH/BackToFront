﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

using BackToFront.Utilities;

namespace BackToFront.Extensions.Reflection
{
    public static class Reflection
    {
        /// <summary>
        /// Mimicks "this is parentClass"
        /// </summary>
        /// <param name="target"></param>
        /// <param name="parentClass"></param>
        /// <returns></returns>
        public static bool Is(this Type target, Type parentClass)
        {
            return target == parentClass || 
                (parentClass.IsInterface ? target.GetInterfaces().Any(a => a == parentClass || (a.IsGenericType && a.GetGenericTypeDefinition() == parentClass)) : target.IsSubclassOf(parentClass));
        }

        /// <summary>
        /// Mimicks "this is parentClass"
        /// </summary>
        /// <param name="target"></param>
        /// <param name="parentClass"></param>
        /// <returns></returns>
        public static bool Is<T>(this Type target)
        {
            return Is(target, typeof(T));
        }

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

        public static Type MemberType(this MemberInfo member)
        {
            if (member is Type)
                return member as Type;
            if (member is MethodInfo)
                return (member as MethodInfo).ReturnType;
            if (member is PropertyInfo)
                return (member as PropertyInfo).PropertyType;
            if (member is FieldInfo)
                return (member as FieldInfo).FieldType;

            return null;
        }
    }
}
