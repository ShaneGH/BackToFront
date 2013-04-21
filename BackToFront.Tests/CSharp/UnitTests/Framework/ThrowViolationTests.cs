using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BackToFront.Tests.Base;

using NUnit.Framework;


using BackToFront.Framework;
using BackToFront.Tests.Utilities;
namespace BackToFront.Tests.UnitTests.Framework
{
    [TestFixture]
    public class ThrowViolationTests : TestBase
    {
        public class TestClass : Utilities.TestViolation
        {
            public TestClass(string message)
                : base(message)
            { }

            public object Property { get; set; }
        }

        [Test]
        public void ValidateTest()
        {
            // global arrange
            var violation = new TestClass("Hello");
            var subject = new ThrowViolation<object>(a => { violation.Property = a; return violation; }, null);

            // act
            var item = new object();
            IViolation v = subject.ValidateEntity(item, null);

            // assert
            Assert.AreEqual(violation, v);
            Assert.AreEqual(item, v.ViolatedEntity);
            Assert.AreEqual(item, violation.Property);
        }

        [Test]
        public void ValidateAllTest()
        {
            // arrange
            var violation = new TestClass("Hello");
            var subject = new ThrowViolation<object>(a => { violation.Property = a; return violation; }, null);
            List<IViolation> list = new List<IViolation>();

            // act
            var item = new object();
            subject.FullyValidateEntity(item, list, null);

            // assert
            Assert.AreEqual(1, list.Count);
            Assert.AreEqual(violation, list.First());
            Assert.AreEqual(item, list.First().ViolatedEntity);
            Assert.AreEqual(item, violation.Property);
        }
    }
}
