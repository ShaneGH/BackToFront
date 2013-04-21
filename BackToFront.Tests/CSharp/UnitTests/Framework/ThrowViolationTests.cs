using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BackToFront.Tests.Base;

using NUnit.Framework;


using BackToFront.Framework;
namespace BackToFront.Tests.UnitTests.Framework
{
    [TestFixture]
    public class ThrowViolationTests : TestBase
    {
        ThrowViolation<object> Subject;
        readonly IViolation Violation = new Utilities.TestViolation("Hello");

        public override void Setup()
        {
            base.Setup();

            Subject = new ThrowViolation<object>(() => Violation, null);
        }

        [Test]
        public void ValidateTest()
        {
            // global arrange

            // act
            var item = new object();
            IViolation v = Subject.ValidateEntity(item, null);

            // assert
            Assert.AreEqual(Violation, v);
            Assert.AreEqual(item, v.ViolatedEntity);
        }

        [Test]
        public void ValidateAllTest()
        {
            // arrange
            List<IViolation> list = new List<IViolation>();

            // act
            var item = new object();
            Subject.FullyValidateEntity(item, list, null);

            // assert
            Assert.AreEqual(1, list.Count);
            Assert.AreEqual(Violation, list.First());
            Assert.AreEqual(item, list.First().ViolatedEntity);
        }
    }
}
