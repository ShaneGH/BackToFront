using BackToFront.Dependency;
using BackToFront.Framework;
using BackToFront.Framework.Base;
using BackToFront.Framework.NonGeneric;
using BackToFront.Utils;
using NUnit.Framework;
using System;
using System.Linq;
using System.Collections.Generic;
using M = Moq;

namespace BackToFront.Tests.UnitTests.Framework.NonGeneric
{
    [TestFixture]
    public class RuleWrapper_Tests
    {
        [Test]
        public void RequireThatMembers_Test()
        {
            // arrange
            var member = new MemberChainItem(typeof(string));
            var affected = new M.Mock<IValidate<object>>();
            affected.Setup(a => a.AffectedMembers).Returns(new[] 
            { 
                new AffectedMembers { Member = new MemberChainItem(typeof(object)), Requirement = false } ,
                new AffectedMembers { Member = member, Requirement = true } 
            });

            var rule = new Rule<object>();
            rule.Register(affected.Object);
            var subject = new RuleWrapper(rule, null, null);

            // act
            var result = subject.RequireThatMembers;

            // assert
            Assert.AreEqual(1, result.Count());
            Assert.AreEqual(member, result.First());
        }

        [Test]
        public void Result_Test_noDi_NoCache()
        {
            // arrange
            var test = new object();
            var rule = new M.Mock<IValidate>();
            var violations = new[] { new M.Mock<IViolation>().Object };
            rule.Setup(r => r.FullyValidateEntity(test, M.It.IsAny<Mocks>())).Returns(() => violations);

            var subject = new RuleWrapper(rule.Object, test, null);

            // act
            var result = subject.Result(false);

            // assert
            Assert.AreEqual(violations, result);
        }

        [Test]
        public void Result_Test_noDi_WithCache()
        {
            // arrange
            var test = new object();
            var rule = new M.Mock<IValidate>();
            rule.Setup(r => r.FullyValidateEntity(test, M.It.IsAny<Mocks>())).Returns(() => new[] { new M.Mock<IViolation>().Object });

            var subject = new RuleWrapper(rule.Object, test, null);

            // act
            var result1 = subject.Result(false);
            var result2 = subject.Result(false);

            // assert
            Assert.IsTrue(result1 == result2);
        }
    }
}
