using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using BackToFront.Logic.Base;

namespace BackToFront.UnitTests.Utilities
{
    public class SimpleIValidate : IPathElement<object>
    {
        public EventHandler<object> ValidateCalled;
        public EventHandler<object> ValidateAllCalled;

        public IViolation Violation;

        public IViolation ValidateEntity(object subject)
        {
            if (ValidateCalled != null)
                ValidateCalled(this, subject);

            return Violation;
        }

        public void FullyValidateEntity(object subject, IList<IViolation> violationList)
        {
            if (ValidateAllCalled != null)
                ValidateAllCalled(this, subject);

            violationList.Add(Violation);
        }

        public Logic.Base.IPathElement<object> NextOption
        {
            get { throw new NotImplementedException(); }
        }
    }
}
