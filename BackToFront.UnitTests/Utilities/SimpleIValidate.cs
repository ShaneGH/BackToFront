using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BackToFront.UnitTests.Utilities
{
    public class SimpleIValidate : IValidatablePathElement<object>
    {
        public EventHandler<object> ValidateCalled;
        public EventHandler<object> ValidateAllCalled;

        public IViolation Violation;

        public IViolation Validate(object subject)
        {
            if (ValidateCalled != null)
                ValidateCalled(this, subject);

            return Violation;
        }

        public void ValidateAll(object subject, IList<IViolation> violationList)
        {
            if (ValidateAllCalled != null)
                ValidateAllCalled(this, subject);

            violationList.Add(Violation);
        }

        public Logic.Base.IPathElement NextOption
        {
            get { throw new NotImplementedException(); }
        }
    }
}
