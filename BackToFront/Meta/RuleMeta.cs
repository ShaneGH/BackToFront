using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using BackToFront.Utilities;
using BackToFront.Validation;

namespace BackToFront.Meta
{
    [DataContract]
    public class RuleMeta
    {
        [DataMember]
        public ExpressionMeta Expression { get; private set; }

        [DataMember]
        public MemberChainItem[] AffectedMembers { get; private set; }

        public RuleMeta() { }

        public RuleMeta(IEnumerable<MemberChainItem> affectedMembers, ExpressionMeta expression)
        {
            AffectedMembers = affectedMembers.ToArray();
            Expression = expression;
        }

        public RuleMeta(INonGenericRule rule)
            : this(rule.ValidatableMembers, ExpressionMeta.CreateMeta(rule.PreCompiled))
        {
        }
    }
}
