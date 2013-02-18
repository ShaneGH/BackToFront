using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BackToFront.UnitTests.Utilities.TestClasses
{
    /// <summary>
    /// ToTest: multiple rules
    ///         If, is true, model violation is
    /// </summary>
    public class TestPass1
    {
        public static SimpleViolation Violation1 = new SimpleViolation("Violation");
        public static SimpleViolation Violation2 = new SimpleViolation("Violation");

        static TestPass1()
        {
            Rules.Add<TestPass1, SimpleViolation>(rule => rule
                .If(a => a.ThrowViolation1).IsTrue().ModelViolationIs(Violation1)
                .If(a => a.ThrowViolation2).IsTrue().ModelViolationIs(Violation2));
        }

        public bool ThrowViolation1 { get; set; }
        public bool ThrowViolation2 { get; set; }
    }
}
