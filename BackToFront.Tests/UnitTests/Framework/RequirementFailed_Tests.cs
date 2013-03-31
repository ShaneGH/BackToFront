using BackToFront.Framework;
using BackToFront.Utils;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
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
            var result = subject.WithModelViolation(() => new M.Mock<IViolation>().Object);
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
            var result = subject.WithModelViolation(() => violation);
            var npe = subject.NextPathElements(null, null);

            // assert
            Assert.AreEqual(rule, result);
            Assert.NotNull(npe.ElementAt(0));
            Assert.IsInstanceOf<ThrowViolation<object>>(npe.ElementAt(0));
        }

        [Test]
        public void ValidateEntity_Test()
        {
            var rule = new Rule<object>();
            var mocks = new Mocks();
            var entity = new object();
            var violation = new M.Mock<IViolation>().Object;
            Expression<Func<object, bool>> exp = a => true;
            var subject = new M.Mock<RequirementFailed<object>>(exp, rule) { CallBase = true };

            // act
            var result = subject.Object.ValidateEntity(entity, mocks);

            // assert
            Assert.IsNull(result);
        }

        [Test]
        public void FullyValidateEntity_Test()
        {
            var rule = new Rule<object>();
            var mocks = new Mocks();
            var entity = new object();
            var violation = new M.Mock<IViolation>().Object;
            Expression<Func<object, bool>> exp = a => true;
            var subject = new M.Mock<RequirementFailed<object>>(exp, rule) { CallBase = true };
            var violations = new List<IViolation>();

            // act
            subject.Object.FullyValidateEntity(entity, violations, mocks);

            // assert
            Assert.AreEqual(0, violations.Count);
        }
    }
}