using BackToFront.Dependency;
using BackToFront.Utils;
using System;
using System.Collections.Generic;
using System.Linq;

namespace BackToFront.Framework.NonGeneric
{
    public class RuleWrapper
    {
        public readonly INonGenericRuleXX Rule;

        public readonly object ValidationSubject;
        private readonly Func<IRuleDependencies> ServiceContainer;
        private Dictionary<bool, IViolation[]> CachedResults = new Dictionary<bool, IViolation[]>();

        public RuleWrapper(INonGenericRuleXX rule, object toValidate, Func<IRuleDependencies> serviceContainer)
        {
            Rule = rule;
            ValidationSubject = toValidate;
            ServiceContainer = serviceContainer;
        }

        public IEnumerable<MemberChainItem> RequireThatMembers
        {
            get
            {
                return Rule.AffectedMembers.Where(a => a.Requirement).Select(a => a.Member);
            }
        }

        public List<DependencyWrapper> Dependencies
        {
            get
            {
                return Rule.Dependencies;
            }
        }

        public IEnumerable<IViolation> Result(bool useServiceContainerDI)
        {
            if (!CachedResults.ContainsKey(useServiceContainerDI))
            {
                Mocks mocks = null;
                if (useServiceContainerDI)
                {
                    var di = ServiceContainer();
                    if (di == null)
                        throw new InvalidOperationException("##");

                    mocks = new Mocks(Rule.Dependencies.Select(d => di.GetDependency(d.DependencyName, d.DependencyType, Rule).ToMock()));
                }
                else
                {
                    mocks = new Mocks();
                }

                CachedResults[useServiceContainerDI] = Rule.FullyValidateEntity(ValidationSubject, mocks).ToArray();
            }

            return CachedResults[useServiceContainerDI];
        }
    }
}
