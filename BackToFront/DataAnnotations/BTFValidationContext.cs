using System;
using System.Collections.Generic;
using System.Linq;
using DA = System.ComponentModel.DataAnnotations;

namespace BackToFront.DataAnnotations
{
    internal class BTFValidationContext
    {
        public readonly RuleWrapper[] Rules;

        public BTFValidationContext(DA.ValidationContext validationContext)
        {
            var rules = new List<RuleWrapper>();

            Type current = validationContext.ObjectType;
            while (current != null)
            {
                rules.AddRange(BackToFront.Rules.GetRules(current).Select(r => new RuleWrapper(r, validationContext.ObjectInstance)));
                current = current.BaseType;
            }

            Rules = rules.ToArray();
        }
    }
}
