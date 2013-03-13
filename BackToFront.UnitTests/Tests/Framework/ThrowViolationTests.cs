using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BackToFront.UnitTests.Base;

using NUnit.Framework;


using BackToFront.Framework;
namespace BackToFront.UnitTests.Tests.Framework
{
    [TestFixture]
    public class ThrowViolationTests : TestBase
    {
        ThrowViolation<object> Subject;
        readonly IViolation Violation = new Utilities.SimpleViolation("Hello");

        public override void Setup()
        {
            base.Setup();

            Subject = new ThrowViolation<object>(Violation, null);
        }

        [Test]
        public void ValidateTest()
        {
            // global arrange

            // act
            IViolation v = Subject.ValidateEntity(null, null);

            // assert
            Assert.AreEqual(Violation, v);
        }

        [Test]
        public void ValidateAllTest()
        {
            // arrange
            List<IViolation> list = new List<IViolation>();

            // act
            Subject.FullyValidateEntity(null, list, null);

            // assert
            Assert.AreEqual(1, list.Count);
            Assert.AreEqual(Violation, list.First());
        }
    }
}
