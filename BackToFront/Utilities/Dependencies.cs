using BackToFront.Dependency;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using E = System.Linq.Expressions;

using BackToFront.Extensions.Reflection;

namespace BackToFront.Utilities
{
    public class Dependencies : ReadonlyDictionary<string, object>
    {
        public readonly E.Expression Parameter;
        private readonly Dictionary<string, E.UnaryExpression> Params = new Dictionary<string, E.UnaryExpression>();

        public Dependencies() : this(new Dictionary<string, object>(), E.Expression.Empty()) { }

        public Dependencies(IDictionary<string, object> dependencies, E.Expression parameter)
            : base(dependencies.ToDictionary(a => a.Key, a => a.Value))
        {
            if (dependencies.Any() && !parameter.Type.Is(typeof(IDictionary<string,object>)))
                throw new InvalidOperationException("##");

            Parameter = parameter;
            foreach (var key in Keys)
            {
                // TODO: is this correct? Needed to stop exception in ParameterForDependency
                if (this[key] == null)
                    throw new InvalidOperationException("##");
            }
        }

        public E.UnaryExpression ParameterForDependency(string dependencyName)
        {
            if (!Params.ContainsKey(dependencyName))
            {
                if(!ContainsKey(dependencyName))
                    throw new InvalidOperationException("##");

                Params.Add(dependencyName, E.Expression.Convert(E.Expression.Property(Parameter, "Item", E.Expression.Constant(dependencyName)), this[dependencyName].GetType()));
            }

            return Params[dependencyName];
        }
    }
}
