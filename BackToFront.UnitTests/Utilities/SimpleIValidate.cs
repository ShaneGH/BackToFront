using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using BackToFront.Framework.Base;

namespace BackToFront.UnitTests.Utilities
{
    internal class SimpleIValidate : PathElement<object>
    {
        public IViolation Violation;

        public SimpleIValidate()
            : base(null) { }

        public override void ValidateEntity(object subject, out IViolation violation)
        {
            violation = Violation;
        }

        public override void FullyValidateEntity(object subject, IList<IViolation> violationList)
        {
            violationList.Add(Violation);
        }

        protected override IEnumerable<PathElement<object>> NextPathElements(object subject)
        {
            throw new InvalidOperationException();
        }
    }
}
