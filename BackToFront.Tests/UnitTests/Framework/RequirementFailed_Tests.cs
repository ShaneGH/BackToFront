using BackToFront.Framework;
using NUnit.Framework;
using System.Linq;
using M = Moq;

namespace BackToFront.Tests.UnitTests.Framework
{
    [TestFixture]
    public class RequirementFailed_Tests : BackToFront.Tests.Base.TestBase
    {
        [Test]
        public void NextPathElements_Test()
        {
            // arrange
            var subject = new RequirementFailed<object>(a => true, null);

            // act
            var result = subject.OrModelViolationIs(new M.Mock<IViolation>().Object);
            var npe = subject.NextPathElements(null, null);

            // assert
            Assert.AreEqual(1, npe.Count());
            Assert.NotNull(npe.ElementAt(0));
        }

        [Test]
        public void OrModelViolationIs_Test()
        {
            // arrange
            var rule = new Rule<object>();
            var subject = new RequirementFailed<object>(a => true, rule);
            var violation = new M.Mock<IViolation>().Object;

            // act
            var result = subject.OrModelViolationIs(violation);
            var npe = subject.NextPathElements(null, null);

            // assert
            Assert.AreEqual(rule, result);
            Assert.NotNull(npe.ElementAt(0));
            Assert.IsInstanceOf<ThrowViolation<object>>(npe.ElementAt(0));
        }

        //[Test]
        //public void ValidateEntity
    }
}