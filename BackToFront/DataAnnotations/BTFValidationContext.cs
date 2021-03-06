﻿using BackToFront.Dependency;
using System;
using System.Collections.Generic;
using System.Linq;
using DA = System.ComponentModel.DataAnnotations;
using BackToFront.Framework;
using BackToFront.Validation;

namespace BackToFront.DataAnnotations
{
    internal class BTFValidationContext
    {
        public readonly Dictionary<INonGenericRule, IEnumerable<IViolation>> ResultCache = new Dictionary<INonGenericRule, IEnumerable<IViolation>>();
        public readonly INonGenericRule[] Rules;
        public readonly IRuleDependencies DI;
        public readonly object ObjectInstance;

        public BTFValidationContext(DA.ValidationContext validationContext, Domain domain)
        {
            var rules = new List<INonGenericRule>();
            if (domain == null)
                throw new InvalidOperationException("##" + "need a domain");

            DI = (IRuleDependencies)validationContext.ServiceContainer.GetService(typeof(IRuleDependencies));
            ObjectInstance = validationContext.ObjectInstance;

            Type current = validationContext.ObjectType;
            while (current != null)
            {
                rules.AddRange(domain.Rules(current));
                current = current.BaseType;
            }

            Rules = rules.ToArray();
        }
    }
}
