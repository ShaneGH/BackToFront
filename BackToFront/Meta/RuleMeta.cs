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
using WebExpressions.Meta;

namespace BackToFront.Meta
{
    [DataContract]
    public class RuleMeta
    {
        [DataMember]
        public ExpressionMeta Expression { get; private set; }

        [DataMember]
        public string EntityParameter { get; private set; }

        [DataMember]
        public string ContextParameter { get; private set; }

        [DataMember]
        public MemberChainItem[] ValidationSubjects { get; private set; }

        [DataMember]
        public MemberChainItem[] RequiredForValidation { get; private set; }

        public RuleMeta() { }

        public RuleMeta(IEnumerable<MemberChainItem> validationSubjects, IEnumerable<MemberChainItem> requiredForValidation, ExpressionMeta expression, string entity, string contextParameter)
        {
            ValidationSubjects = validationSubjects.ToArray();
            RequiredForValidation = requiredForValidation.ToArray();
            Expression = expression;
            EntityParameter = entity;
            ContextParameter = contextParameter;
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
                var current = m.NextItem;
                var member = new List<string>();
                var finish = false;
                while (current != null)
                {
                    if (current.Member is PropertyInfo || current.Member is FieldInfo)
                        member.Add(current.Member.Name);
                    else
                    {
                        // don't want methods
                        finish = true;
                        break;
                    }

                    current = current.NextItem;
                }

                if (!finish && member.Count > 0)
                    yield return string.Join(".", member);
            }
        }
    }
}
