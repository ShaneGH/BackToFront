using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BackToFront.Dependency;
using BackToFront.Framework;
using BackToFront.Validation;

namespace BackToFront
{
    /// <summary>
    /// Used to get or set a global dependency resolver for back to front
    /// </summary>
    public static class BackToFrontDependency
    {
        private static readonly IDependencyResolver DummyResolver = new DummyDependencyResolver();

        public static IDependencyResolver Resolver;

        internal static IDependencyResolver ProtectedResolver
        {
            get
            {
                return Resolver ?? DummyResolver;
            }
        }
    }
}

namespace BackToFront.Dependency
{
    public interface IDependencyResolver
    {
        object GetService(Type serviceType);
        IEnumerable<object> GetServices(Type serviceType);
    }

    internal class DummyDependencyResolver : IDependencyResolver
    {
        public object GetService(Type serviceType)
        {
            return null;
        }

        public IEnumerable<object> GetServices(Type serviceType)
        {
            yield break;
        }
    }

    public interface IRuleDependencies
    {
        RuleDependency GetDependency(string dependencyName, Type dependencyType, INonGenericRule rule);
    }
}