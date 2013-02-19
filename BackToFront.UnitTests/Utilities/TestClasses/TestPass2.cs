using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BackToFront.UnitTests.Utilities.TestClasses
{
    /// <summary>
    /// ToTest: and
    ///         If, is true, and, is true, model violation is
    /// </summary>
    public class TestPass2
    {
        public static SimpleViolation Violation = new SimpleViolation("Violation");

        static TestPass2()
        {
            Rules.Add<TestPass2>(rule => rule
                .If(a => a.ThrowViolationSwitch1).IsTrue().And(a => a.ThrowViolationSwitch2).IsTrue().ModelViolationIs(Violation));
        }

        public bool ThrowViolationSwitch1 { get; set; }
        public bool ThrowViolationSwitch2 { get; set; }
    }
}
