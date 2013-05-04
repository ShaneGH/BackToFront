using BackToFront.Dependency;
using BackToFront.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using BackToFront.Validation;
using BackToFront.Expressions.Visitors;
using System.Linq.Expressions;

namespace BackToFront.Framework.NonGeneric
{
    //TODO: remove this class and call a Repository directly
    public class RuleWrapper
    {
        public readonly INonGenericRule Rule;

        public readonly object ValidationSubject;
        private readonly Func<IRuleDependencies> ServiceContainer;
        private Dictionary<bool, IViolation[]> CachedResults = new Dictionary<bool, IViolation[]>();

        public RuleWrapper(INonGenericRule rule, object toValidate, Func<IRuleDependencies> serviceContainer)
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

        public IEnumerable<IViolation> Result(bool useServiceContainerDI)
        {
            if (!CachedResults.ContainsKey(useServiceContainerDI))
            {
                IDictionary<string, object> dependencies = null;
                if (useServiceContainerDI)
                {
                    var di = ServiceContainer();
                    if (di == null)
                        throw new InvalidOperationException("##");

                    dependencies = Rule.Dependencies.Select(d => di.GetDependency(d.DependencyName, d.DependencyType, Rule)).ToDictionary(a => a.Key, a => a.Value);
                }
                else
                {
                    dependencies = new Dictionary<string, object>();
                }

                var ctxt = new ValidationContext(false, new object[0], dependencies);
                Rule.NewCompile(new SwapPropVisitor(new Mocks(), dependencies, ValidationSubject.GetType()))(ValidationSubject, ctxt);
                CachedResults[useServiceContainerDI] = ctxt.Violations.ToArray();
            }

            return CachedResults[useServiceContainerDI];
        }
    }
}
