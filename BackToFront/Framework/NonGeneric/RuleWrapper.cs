using BackToFront.Dependency;
using BackToFront.Utils;
using System;
using System.Collections.Generic;
using System.Linq;

namespace BackToFront.Framework.NonGeneric
{
    public class RuleWrapper : RuleWrapperBase
    {
        public readonly object ValidationSubject;
        private readonly Func<IRuleDependencies> ServiceContainer;
        private Dictionary<bool, IViolation[]> CachedResults = new Dictionary<bool, IViolation[]>();

        public RuleWrapper(object rule, object toValidate, Func<IRuleDependencies> serviceContainer)
            : base(rule)
        {
            ValidationSubject = toValidate;
            ServiceContainer = serviceContainer;
        }

        public IEnumerable<MemberChainItem> RequireThatMembers
        {
            get
            {
                return AffectedMembers.Where(a => a.Requirement).Select(a => a.Member);
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

                    mocks = new Mocks(Dependencies.Select(d => di.GetDependency(d.DependencyName, d.DependencyType, Rule.Item).ToMock()));
                }
                else
                {
                    mocks = new Mocks();
                }

                CachedResults[useServiceContainerDI] = Rule.Method<IEnumerable<IViolation>, object, Mocks>("FullyValidateEntity", ValidationSubject, mocks).ToArray();
            }

            return CachedResults[useServiceContainerDI];
        }
    }
}
