using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using BackToFront.Framework.Base;
using BackToFront.Utilities;

namespace BackToFront.Tests.Utilities
{
    public class SimpleIValidate : PathElement<object>
    {
        public IViolation Violation;

        public SimpleIValidate()
            : base(null) { }

        public override IViolation ValidateEntity(object subject, ValidationContext context)
        {
            return Violation;
        }

        public override void FullyValidateEntity(object subject, IList<IViolation> violationList, ValidationContext context)
        {
            violationList.Add(Violation);
        }

        public override IEnumerable<PathElement<object>> NextPathElements(object subject, ValidationContext context)
        {
            yield break;
        }

        public override IEnumerable<AffectedMembers> AffectedMembers
        {
            get { throw new NotImplementedException(); }
        }

        public override bool PropertyRequirement
        {
            get { throw new NotImplementedException(); }
        }
    }
}
