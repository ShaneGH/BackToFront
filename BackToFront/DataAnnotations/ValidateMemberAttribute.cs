using BackToFront.Enum;
using BackToFront.Extensions.IEnumerable;
using BackToFront.Framework.NonGeneric;
using BackToFront.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using DA = System.ComponentModel.DataAnnotations;

namespace BackToFront.DataAnnotations
{
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Field, AllowMultiple = false)]
    public class ValidateMemberAttribute : DA.ValidationAttribute
    {
        public const string BackToFrontValidationContext = "BackToFront.DataAnnotations.BTFValidationContext";

        public override bool RequiresValidationContext
        {
            get
            {
                return true;
            }
        }

        /// <summary>
        /// Default IgnoreRulesWithDependencies
        /// </summary>
        public DependencyBehavior DependencyBehavior { get; set; }

        public ValidateMemberAttribute()
        {
            DependencyBehavior = DependencyBehavior.IgnoreRulesWithDependencies;
        }

        public virtual string FormatMultipleErrors(IEnumerable<string> errors)
        {
            return string.Join("\n", errors.ToArray());
        }

        private bool DependencyBehaviorChooser(RuleWrapper a)
        {
            switch (DependencyBehavior)
            {
                case DependencyBehavior.IgnoreRulesWithDependencies:
                    return a.Dependencies.Count == 0;
                case DependencyBehavior.UseInbuiltDI:
                case DependencyBehavior.UseServiceContainerAndInbuiltDI:
                    return true;
                default:
                    throw new InvalidOperationException("##");
            }
        }

        protected override DA.ValidationResult IsValid(object value, DA.ValidationContext validationContext)
        {
            var ctxt = ProcessValidationContext(validationContext);

            var member = Create(validationContext.ObjectType, validationContext.MemberName);

            var useDependencies = DependencyBehavior == DependencyBehavior.UseServiceContainerAndInbuiltDI;
            
            var violations =
                ctxt.Rules.Where(rule => DependencyBehaviorChooser(rule) && rule.RequireThatMembers.Contains(member))
                .Select(rule => rule.Result(useDependencies).Where(result => result.Violated.Contains(member))).Aggregate();

            if (!violations.Any())
                return DA.ValidationResult.Success;

            return new DA.ValidationResult(
                FormatMultipleErrors(violations.Select(v => v.UserMessage)), 
                violations.Select(v => v.Violated.Select(m => m.UltimateMember.Name)).Aggregate());
        }

        internal static BTFValidationContext ProcessValidationContext(DA.ValidationContext validationContext)
        {
            if (!validationContext.Items.ContainsKey(BackToFrontValidationContext))
            {
                var ctxt = new BTFValidationContext(validationContext);
                validationContext.Items.Add(BackToFrontValidationContext, ctxt);
            }

            if (validationContext.Items[BackToFrontValidationContext] is BTFValidationContext)
                return validationContext.Items[BackToFrontValidationContext] as BTFValidationContext;

            throw new InvalidOperationException("##");
        }

        private static MemberChainItem Create(Type type, string MemberName)
        {
            throw new InvalidOperationException();
        }
    }
}
