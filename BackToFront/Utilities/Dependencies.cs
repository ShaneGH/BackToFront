using BackToFront.Dependency;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using E = System.Linq.Expressions;

namespace BackToFront.Utilities
{
    public class Dependencies : ReadonlyDictionary<string, object>
    {
        public readonly E.ParameterExpression Parameter = E.Expression.Parameter(typeof(IDictionary<string, object>));
        private readonly Dictionary<string, E.UnaryExpression> Params = new Dictionary<string, E.UnaryExpression>();

        //TODO delete this constructor
        public Dependencies(IEnumerable<RuleDependency> d) : this(d.ToDictionary(a => a.Name, a => a.Value)) { }

        public Dependencies() : this(new Dictionary<string, object>()) { }

        public Dependencies(IDictionary<string, object> dependencies)
            : base(dependencies.ToDictionary(a => a.Key, a => a.Value))
        {
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
