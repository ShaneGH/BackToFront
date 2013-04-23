using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BackToFront.Framework;
using BackToFront.Validation;

namespace BackToFront
{
    public static class Rules
    {
        private static Type RepositoryType = typeof(Rules<>);
        private static readonly Dictionary<Type, Func<IEnumerable<INonGenericRule>>> _Rules = new Dictionary<Type, Func<IEnumerable<INonGenericRule>>>();

        public static IEnumerable<INonGenericRule> GetRules(Type forType)
        {
            if (!_Rules.ContainsKey(forType))
            {
                var repositoryType = RepositryFor(forType);
                var repository = repositoryType.GetField("Repository").GetValue(null);
                var rules = repositoryType.GetProperty("Registered");

                _Rules.Add(forType, () => (IEnumerable<INonGenericRule>)rules.GetValue(repository));
            }

            return _Rules[forType]();
        }

        private static Type RepositryFor(Type type)
        {
            return RepositoryType.MakeGenericType(type);
        }
    }
}
