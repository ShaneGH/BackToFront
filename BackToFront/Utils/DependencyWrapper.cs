using BackToFront.Dependencies;
using System;
using System.Collections.Generic;
using System.Reflection;

namespace BackToFront.Utils
{
    public abstract class DependencyWrapper
    {
        public readonly string DependencyName;
        public abstract Type DependencyType { get; }
        private readonly static Dictionary<Type, PropertyInfo> _ValProperties = new Dictionary<Type, PropertyInfo>();

        public DependencyWrapper(string name)
        {
            DependencyName = name;
        }

        public static PropertyInfo ValProperty(Type genericType)
        {
            if (!_ValProperties.ContainsKey(genericType))
                _ValProperties[genericType] = typeof(DependencyWrapper<>).MakeGenericType(genericType).GetProperty("Val");

            return _ValProperties[genericType];
        }
    }

    public sealed class DependencyWrapper<TDependency> : DependencyWrapper
    {
        private static readonly Type _DependencyType = typeof(TDependency);
        private readonly Func<IDependencyResolver> _Resolver;

        internal DependencyWrapper(string name, Func<IDependencyResolver> resolver)
            : base(name) 
        {
            _Resolver = resolver;
        }

        public TDependency Val
        {
            get 
            {
                var dependency = (TDependency)_Resolver().GetService(typeof(TDependency));
                if (dependency != null)
                    return dependency;

                throw new InvalidOperationException("##" + "Item is not mocked"); }
        }

        public override Type DependencyType
        {
            get { return _DependencyType; }
        }
    }
}
