using BackToFront.Enum;
using BackToFront.Extensions.IEnumerable;
using BackToFront.Extensions.Reflection;
using BackToFront.Framework.NonGeneric;
using BackToFront.Utilities;
using System;
using System.Text.RegularExpressions;
using System.Collections.Generic;
using System.Linq;
using DA = System.ComponentModel.DataAnnotations;
using System.Reflection;

namespace BackToFront.DataAnnotations
{
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Field, AllowMultiple = false)]
    public class ValidateMemberAttribute : DA.ValidationAttribute
    {
        private static readonly Regex _propertyChain;
        private static readonly Regex _indexedProperty;
        static ValidateMemberAttribute()
        {
            var index = @"\[[0-9]+\]";
            var indexedProperty = @"[_a-zA-Z][_a-zA-Z0-9]*(" + index + ")?";
            var dotIndexedProperty = @"\." + indexedProperty;

            _indexedProperty = new Regex(index + "$");
            _propertyChain = new Regex(@"^" + indexedProperty + @"(" + dotIndexedProperty + @")*$");
        }

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
                    return a.Rule.Dependencies.Count == 0;
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

        public static MemberChainItem Create(Type type, string memberName)
        {
            if (!_propertyChain.IsMatch(memberName))
                throw new InvalidOperationException("##" + memberName); // exception must say that functions are not supported

            var result = new MemberChainItem(type);
            MemberChainItem current = result; 
            foreach (var m in memberName.Split('.'))
            {
                string member = m;
                MemberIndex index = null;
                if (_indexedProperty.IsMatch(member))
                {
                    var last = member.LastIndexOf('[');
                    index = new MemberIndex(int.Parse(member.Substring(last + 1, member.Length - last - 2)));
                    member = member.Substring(0, last);
                }

                var nextType = current.Index != null ? current.IndexedType : current.Member.MemberType();
                var newMember = nextType.GetMember(member).FirstOrDefault(a => a is PropertyInfo || a is FieldInfo);
                if (newMember == null)
                    throw new InvalidOperationException("##");

                current = current.NextItem = new MemberChainItem(newMember, index);
            }

            return result;
        }
    }
}
