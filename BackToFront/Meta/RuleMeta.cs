//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using System.Threading.Tasks;
//using BackToFront.Utilities;
//using BackToFront.Validation;

//namespace BackToFront.Meta
//{
//    public class RuleMeta
//    {
//        public ExpressionMeta Expression { get; private set; }
//        public MemberChainItem[] AffectedMembers { get; private set; }

//        public RuleMeta() { }

//        public RuleMeta(IEnumerable<MemberChainItem> affectedMembers, ExpressionMeta expression)
//        {
//            AffectedMembers = affectedMembers.ToArray();
//            Expression = expression;
//        }

//        public RuleMeta(INonGenericRule rule)
//            : this(rule.AffectedMembers, ExpressionMeta.CreateMeta(rule.PreCompiled))
//        {
//        }
//    }
//}
