using BackToFront.Dependency;
using BackToFront.Framework.NonGeneric;
using System;
using System.Collections.Generic;
using System.Linq;
using DA = System.ComponentModel.DataAnnotations;
using BackToFront.Framework;

namespace BackToFront.DataAnnotations
{
    internal class BTFValidationContext
    {
        public readonly RuleWrapper[] Rules;

        public BTFValidationContext(DA.ValidationContext validationContext)
        {
            var rules = new List<RuleWrapper>();

            Func<IRuleDependencies> di = () => (IRuleDependencies)validationContext.ServiceContainer.GetService(typeof(IRuleDependencies));

            Type current = validationContext.ObjectType;
            while (current != null)
            {
                rules.AddRange(BackToFront.Rules.GetRules(current).Select(r => new RuleWrapper(r, validationContext.ObjectInstance, di)));
                current = current.BaseType;
            }

            Rules = rules.ToArray();
        }
    }
}
