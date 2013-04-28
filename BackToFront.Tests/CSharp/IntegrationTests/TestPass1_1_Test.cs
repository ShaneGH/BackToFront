using BackToFront.Enum;
using BackToFront.Tests.Utilities;
using BackToFront.Validate;
using NUnit.Framework;
using System.Linq;

namespace BackToFront.Tests.CSharp.IntegrationTests
{
    /// <summary>
    /// ToTest: multiple rules
    ///         If, is true, model violation is
    /// </summary>
    [TestFixture]
    public class TestPass1_1_Test : Base.TestBase
    {
        public static TestViolation Violation1 = new TestViolation("Violation");
        public class TestClass
        {
            static TestClass()
            {
                Rules<TestClass>.AddRule<Dependency>((rule, hhh) => rule
                    .RequireThat(a => a.Match == hhh.Val.Match).WithModelViolation(() => Violation1));

                Rules<TestClass>.AddRule<Dependency>((rule, hhh) => rule
                    .RequireThat(a => a.Match == hhh.Val.MatchMethod()).WithModelViolation(() => Violation1));
            }

            public bool Match { get; set; }
        }

        public class Dependency
        {
            public bool Match { get; set; }
            public bool MatchMethod() 
            {
                return Match;
            }
        }

        [Test]
        [TestCase(false, false)]
        [TestCase(false, true)]
        [TestCase(true, false)]
        [TestCase(true, true)]
        public void Test_InjectDependency1(bool item1, bool item2)
        {
            // arrange
            var subject = new TestClass { Match = item1 };

            var dependency = new Dependency { Match = item2 };

            // act
            var violation = subject.Validate(new { hhh = dependency }).FirstViolation;

            // assert
            if (item1 == item2)
                Assert.IsNull(violation);
            else
                Assert.AreEqual(Violation1, violation);
        }
    }
}
