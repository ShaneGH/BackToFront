using BackToFront.Extensions.IEnumerable;
using BackToFront.Framework.Base;
using BackToFront.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using DA = System.ComponentModel.DataAnnotations;

namespace BackToFront.DataAnnotations
{
    public class RuleWrapper
    {
        public readonly ReflectionWrapper Rule;
        public readonly object ValidationSubject;

        public RuleWrapper(object rule, object toValidate)
        {
            Rule = new ReflectionWrapper(rule);
            ValidationSubject = toValidate;
        }

        public List<DependencyWrapper> Dependencies
        {
            get
            {
                return Rule.Property<List<DependencyWrapper>>("Dependencies");
            }
        }

        public IEnumerable<MemberChainItem> RequireThatMembers
        {
            get
            {
                return Rule.Property<IEnumerable<AffectedMembers>>("AffectedMembers").Where(a => a.Requirement).Select(a => a.Member);
            }
        }

        private IEnumerable<IViolation> _Result;
        public IEnumerable<IViolation> Result
        {
            get
            {
                if (_Result == null)
                {
                    // TODO: dependencies
                    _Result = Rule.Method<IEnumerable<IViolation>, object, Mocks>("FullyValidateEntity", ValidationSubject, new Mocks());
                }

                return _Result;
            }
        }
    }

    /// <summary>
    /// Describes how the ValidateMemberAttribute handles dependencies
    /// </summary>
    public enum DependencyBehavior
    {
        /// <summary>
        /// When validating a property, the rules which require dependencies will be ignored
        /// </summary>
        IgnoreRulesWithDependencies,
        /// <summary>
        /// When validating the property, rules which require dependencies will not be given them, forcing rules to searcdh for dependencies via dependency injection
        /// </summary>
        UseInbuiltDI,
        /// <summary>
        /// When validating the property, rules which require dependencies will be given them if they exist in the ServiceContainer. Otherwise DI is used
        /// </summary>
        UseServiceContainerAndInbuiltDI
    }

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

            // TODO: dependencies
            var violations =
                ctxt.Rules.Where(rule => DependencyBehaviorChooser(rule) && rule.RequireThatMembers.Contains(member))
                .Select(rule => rule.Result.Where(result => result.Violated.Contains(member))).Aggregate();

            if (!violations.Any())
                return DA.ValidationResult.Success;

            return new DA.ValidationResult(
                FormatMultipleErrors(violations.Select(v => v.UserMessage)), 
                violations.Select(v => v.Violated.Select(m => m.UltimateMember.Name)).Aggregate());
        }

        private static BTFValidationContext ProcessValidationContext(DA.ValidationContext validationContext)
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
