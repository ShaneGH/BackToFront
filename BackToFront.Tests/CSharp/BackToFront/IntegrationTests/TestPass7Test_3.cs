using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using BackToFront.Validate;
using BackToFront.Tests.Utilities;

using NUnit.Framework;

namespace BackToFront.Tests.CSharp.IntegrationTests
{
    /// <summary>
    /// ToTest: Require And
    ///         If.....
    ///         Require, is false, OR, is false, model violation is
    /// </summary>
    [TestFixture]
    public class TestPass7Test_3 : Base.RulesRepositoryTestBase
    {
        public class TestEntity
        {
            public bool RequiredSwitch1 { get; set; }
            public bool RequiredSwitch2 { get; set; }
        }

        public static IEnumerable<Tuple<bool, bool, bool, bool>> GetData()
        {
            for (var i = 0; i < 16; i++)
            {
                var bits = new System.Collections.BitArray(new[] { (byte)i });
                yield return new Tuple<bool, bool, bool, bool>(bits[3], bits[2], bits[1], bits[0]);
            }
        }

        public override void TestFixtureSetUp()
        {
            base.TestFixtureSetUp();

            Repository.AddRule<TestEntity>(rule => rule
                // pass through if
                    .If(a => true).Then(subRule =>
                    {
                        subRule.RequireThat(a => a.RequiredSwitch1).WithModelViolation("RequiredSwitch1");
                        subRule.RequireThat(a => a.RequiredSwitch2).WithModelViolation("RequiredSwitch2");
                    }));
        }

        [Test]
        [TestCase(false, false)]
        [TestCase(false, true)]
        [TestCase(true, false)]
        [TestCase(true, true)]
        public void TestViolatedMembers(bool required1, bool required2)
        {
            // arrange
            var v = new List<string>();
            if (!required1)
                v.Add("RequiredSwitch1");
            if (!required2)
                v.Add("RequiredSwitch2");

            var subject = new TestEntity
            {
                RequiredSwitch1 = required1,
                RequiredSwitch2 = required2,
            };

            // act
            var violation = subject.Validate(Repository).AllViolations;

            // assert
            Assert.AreEqual(v.Count, violation.Count());
            for (int i = 0; i < v.Count; i++)
            {
                Assert.AreEqual(v[i], violation.ElementAt(i).UserMessage);

                Assert.AreEqual(1, violation.ElementAt(i).Violated.Count());
                if (violation.ElementAt(i).UserMessage == "RequiredSwitch1")
                {
                    Assert.AreEqual("RequiredSwitch1", violation.ElementAt(i).Violated.ElementAt(0).UltimateMember.Name);
                }
                else if (violation.ElementAt(i).UserMessage == "RequiredSwitch2")
                {
                    Assert.AreEqual("RequiredSwitch2", violation.ElementAt(i).Violated.ElementAt(0).UltimateMember.Name);
                }
                else
                    Assert.Fail("Invalid violation");
            }
        }
    }
}
