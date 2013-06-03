using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using BackToFront.Utilities;
using BackToFront.Validation;

namespace BackToFront.Meta
{
    [DataContract]
    public class RuleMeta : IMeta
    {
        [DataMember]
        public ExpressionMeta Expression { get; private set; }

        [DataMember]
        public string EntityParameter { get; private set; }

        [DataMember]
        public string ContextParameter { get; private set; }

        [IgnoreDataMember]
        public MemberChainItem[] ValidationSubjects { get; private set; }

        [IgnoreDataMember]
        public MemberChainItem[] RequiredForValidation { get; private set; }

        [DataMember]
        public string[] ValidationSubjectNames { get; set; }

        [DataMember]
        public string[] RequiredForValidationNames { get; set; }

        public RuleMeta() { }

        public RuleMeta(IEnumerable<MemberChainItem> validationSubjects, IEnumerable<MemberChainItem> requiredForValidation, ExpressionMeta expression, string entity, string contextParameter)
        {
            ValidationSubjects = validationSubjects.ToArray();
            RequiredForValidation = requiredForValidation.ToArray();
            Expression = expression;
            EntityParameter = entity;
            ContextParameter = contextParameter;

            ValidationSubjectNames = GetProperties(validationSubjects).ToArray();
            RequiredForValidationNames = GetProperties(requiredForValidation).ToArray();
        }

        public RuleMeta(INonGenericRule rule)
            : this(rule.ValidationSubjects, rule.RequiredForValidation, rule.Meta.Meta.Expression, rule.Meta.Entity.Name, rule.Meta.Context.Name)
        {
        }

        private static IEnumerable<string> GetProperties(IEnumerable<MemberChainItem> members)
        {
            foreach (var m in members)
            {
                if (!(m.Member is Type))
                    throw new ArgumentException("##");

                // don't want initial type
                var current = m.Member;
                var member = new List<string>();
                var finish = false;
                while (current != null)
                {
                    if (m.Member is PropertyInfo || m.Member is FieldInfo)
                        member.Add(current.Name);
                    else
                    {
                        // don't want methods
                        finish = true;
                        break;
                    }
                }

                if (!finish && member.Count > 0)
                    yield return string.Join(".", member);
            }
        }
    }
}
