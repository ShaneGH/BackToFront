using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using BackToFront.Framework.Base;

namespace BackToFront.Tests.Utilities
{
    public class SimpleIValidate : PathElement<object>
    {
        public IViolation Violation;

        public SimpleIValidate()
            : base(null) { }

        public override IViolation ValidateEntity(object subject, Utils.Mocks mocks)
        {
            return Violation;
        }

        public override void FullyValidateEntity(object subject, IList<IViolation> violationList, Utils.Mocks mocks)
        {
            violationList.Add(Violation);
        }

        public override IEnumerable<PathElement<object>> NextPathElements(object subject, Utils.Mocks mocks)
        {
            yield break;
        }
    }
}
