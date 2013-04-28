﻿using BackToFront.Dependency;
using BackToFront.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using BackToFront.Validation;
using BackToFront.Expressions.Visitors;

namespace BackToFront.Framework.NonGeneric
{
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
                Dependencies dependencies = null;
                if (useServiceContainerDI)
                {
                    var di = ServiceContainer();
                    if (di == null)
                        throw new InvalidOperationException("##");

                    dependencies = new Dependencies(Rule.Dependencies.Select(d => di.GetDependency(d.DependencyName, d.DependencyType, Rule)));
                }
                else
                {
                    dependencies = new Dependencies(Enumerable.Empty<KeyValuePair<string, object>>());
                }

                CachedResults[useServiceContainerDI] = Rule.FullyValidateEntity(ValidationSubject, new SwapPropVisitor(new Mocks(), dependencies)).ToArray();
            }

            return CachedResults[useServiceContainerDI];
        }
    }
}
