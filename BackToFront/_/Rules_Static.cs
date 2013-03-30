using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BackToFront
{
    public static class Rules
    {
        private static Type RepositoryType = typeof(Rules<>);
        private static readonly Dictionary<Type, Func<IEnumerable<object>>> _Rules = new Dictionary<Type, Func<IEnumerable<object>>>();

        public static IEnumerable<object> GetRules(Type forType)
        {
            if (!_Rules.ContainsKey(forType))
            {
                var repositoryType = RepositryFor(forType);
                var repository = repositoryType.GetField("Repository").GetValue(null);
                var rules = repositoryType.GetProperty("Registered");

                _Rules.Add(forType, () => (IEnumerable<object>)rules.GetValue(repository));
            }

            return _Rules[forType]();
        }

        private static Type RepositryFor(Type type)
        {
            return RepositoryType.MakeGenericType(type);
        }
    }
}
