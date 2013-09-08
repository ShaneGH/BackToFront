using BackToFront.Dependency;
using System;
using System.Collections.Generic;
using System.Reflection;

namespace BackToFront.Dependency
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
        private readonly IDependencyResolver _Resolver;

        internal DependencyWrapper(string name, IDependencyResolver resolver)
            : base(name) 
        {
            _Resolver = resolver;
        }

        public TDependency Val
        {
            get 
            {
                // this property will be mocked out if an explicit value is set

                var dependency = (TDependency)_Resolver.GetService(typeof(TDependency));
                if (dependency != null)
                    return dependency;

                throw new InvalidOperationException("##" + "Item is not mocked"); 
            }
        }

        public override Type DependencyType
        {
            get { return _DependencyType; }
        }
    }
}
