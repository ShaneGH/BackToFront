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

        public override IViolation ValidateEntity(object subject, IEnumerable<Utils.Mock> mocks)
        {
            return Violation;
        }

        public override void FullyValidateEntity(object subject, IList<IViolation> violationList, IEnumerable<Utils.Mock> mocks)
        {
            violationList.Add(Violation);
        }

        protected override IEnumerable<PathElement<object>> NextPathElements(object subject, IEnumerable<Utils.Mock> mocks)
        {
            yield break;
        }
    }
}
