using BackToFront.Framework;
using BackToFront.Framework.Base;
using BackToFront.Utilities;
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
        public class TestClass
        {
            public bool Success { get; set; }
        }

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
        public void ValidateEntity_Test_success()
        {
            var violation = new M.Mock<IViolation>().Object;
            Expression<Func<TestClass, bool>> exp = a => a.Success;
            var subject = new M.Mock<RequirementFailed<TestClass>>(exp, null) { CallBase = true };

            // act
            var result = subject.Object.ValidateEntity(new TestClass { Success = true }, new ValidationContext { Mocks = new Mocks() });

            // assert
            Assert.IsNull(result);
        }

        [Test]
        public void ValidateEntity_Test_failure()
        {
            var violation = new M.Mock<IViolation>().Object;
            Expression<Func<TestClass, bool>> exp = a => a.Success;
            var subject = new M.Mock<RequirementFailed<TestClass>>(exp, null) { CallBase = true };
            subject.Object.WithModelViolation("efrwsrtw4f");
            var ctxt = new ValidationContext { Mocks = new Mocks() };

            // act
            var result = subject.Object.ValidateEntity(new TestClass { Success = false }, ctxt);
            
            // assert
            // do not test more of result, it comes from a different class
            Assert.NotNull(result);

            Assert.AreEqual(1, ctxt.ViolatedMembers.Count());
            Assert.AreEqual(new MemberChainItem(typeof(TestClass)) { NextItem = new MemberChainItem(typeof(TestClass).GetProperty("Success")) }, ctxt.ViolatedMembers.First());
        }

        [Test]
        public void FullyValidateEntity_Test()
        {
            var rule = new Rule<object>();
            var mocks = new ValidationContext { Mocks = new Mocks() };
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
