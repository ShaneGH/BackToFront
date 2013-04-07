using BackToFront.Dependency;
using BackToFront.Framework.Base;
using BackToFront.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BackToFront.Framework.NonGeneric
{
    public class RuleWrapperBase
    {
        public readonly ReflectionWrapper Rule;

        public RuleWrapperBase(object rule)
        {
            Rule = new ReflectionWrapper(rule);
        }

        public IEnumerable<AffectedMembers> AffectedMembers
        {
            get
            {
                return Rule.Property<IEnumerable<AffectedMembers>>("AffectedMembers");
            }
        }

        public List<DependencyWrapper> Dependencies
        {
            get
            {
                return Rule.Property<List<DependencyWrapper>>("Dependencies");
            }
        }
    }
}
