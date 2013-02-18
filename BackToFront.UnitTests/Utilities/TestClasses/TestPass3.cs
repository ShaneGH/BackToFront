using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BackToFront.UnitTests.Utilities.TestClasses
{
    /// <summary>
    /// ToTest: and
    ///         If, is true, OR, is true, model violation is
    /// </summary>
    public class TestPass3
    {
        public static SimpleViolation Violation = new SimpleViolation("Violation");

        static TestPass3()
        {
            Rules.Add<TestPass3, SimpleViolation>(rule => rule
                .If(a => a.ThrowViolationSwitch1).IsTrue().Or(a => a.ThrowViolationSwitch2).IsTrue().ModelViolationIs(Violation));
        }

        public bool ThrowViolationSwitch1 { get; set; }
        public bool ThrowViolationSwitch2 { get; set; }
    }
}
