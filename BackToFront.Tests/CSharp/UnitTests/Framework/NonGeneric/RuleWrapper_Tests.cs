﻿using BackToFront.Dependency;
using BackToFront.Framework;
using BackToFront.Framework.Base;
using BackToFront.Framework.NonGeneric;
using BackToFront.Utilities;
using NUnit.Framework;
using System;
using System.Linq;
using System.Collections.Generic;
using M = Moq;
using BackToFront.Validation;

namespace BackToFront.Tests.UnitTests.Framework.NonGeneric
{
    [TestFixture]
    public class RuleWrapper_Tests
    {
        public class RuleTest : IValidate
        {
            readonly object Test;
            readonly IEnumerable<IViolation> Violations;

            public RuleTest(object test, IEnumerable<IViolation> violations)
            {
                Test = test;
                Violations = violations;
            }

            public IViolation ValidateEntity(object subject, Mocks mocks)
            {
                throw new NotImplementedException();
            }

            public IEnumerable<IViolation> FullyValidateEntity(object subject, Mocks mocks)
            {
                Assert.AreEqual(Test, subject);
                return Violations;
            }
        }

        [Test]
        public void RequireThatMembers_Test()
        {
            // arrange
            var member = new MemberChainItem(typeof(string));
            var affected = new M.Mock<IValidate<object>>();
            affected.Setup(a => a.AffectedMembers).Returns(new[] 
            { 
                new AffectedMembers { Member = new MemberChainItem(typeof(object)), Requirement = false },
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
        [TestCase(true)]
        [TestCase(false)]
        public void Result_Test_NoCache(bool useDI)
        {
            const string dependencyName = "(OIHP*HG";
            var dependencyType = typeof(string);

            // arrange
            var test = new object();
            var violations = new[] { new M.Mock<IViolation>().Object };
            var injected = new RuleDependency(dependencyName, "sduhfoishdf09u");

            var dependency = new M.Mock<DependencyWrapper>(dependencyName);
            dependency.Setup(d => d.DependencyType).Returns(dependencyType);

            var rule = new M.Mock<INonGenericRule>();
            rule.Setup(a => a.Dependencies).Returns(new List<DependencyWrapper> { dependency.Object });
            if (useDI)
            {
                rule.Setup(a => a.FullyValidateEntity(test, M.It.Is<Mocks>(m => m.Count() == 1 && m.ElementAt(0).Value == injected.Value))).Returns(violations);
            }
            else
            {
                rule.Setup(a => a.FullyValidateEntity(test, M.It.Is<Mocks>(m => m.Count() == 0))).Returns(violations);
            }

            var di = new M.Mock<IRuleDependencies>();
            di.Setup(d => d.GetDependency(dependencyName, dependencyType, rule.Object)).Returns(injected);

            var subject = new RuleWrapper(rule.Object, test, () => di.Object);

            // act
            var result = subject.Result(useDI);

            // assert
            Assert.AreEqual(violations, result);
        }

        [Test]
        [TestCase(true)]
        [TestCase(false)]
        public void Result_Test_WithCache(bool useDI)
        {
            const string dependencyName = "(OIHP*HG";
            var dependencyType = typeof(string);

            // arrange
            var test = new object();
            var violations = new[] { new M.Mock<IViolation>().Object };
            var injected = new RuleDependency(dependencyName, "sduhfoishdf09u");

            var dependency = new M.Mock<DependencyWrapper>(dependencyName);
            dependency.Setup(d => d.DependencyType).Returns(dependencyType);

            var rule = new M.Mock<INonGenericRule>();
            rule.Setup(a => a.Dependencies).Returns(new List<DependencyWrapper> { dependency.Object });
            if (useDI)
            {
                rule.Setup(a => a.FullyValidateEntity(test, M.It.Is<Mocks>(m => m.Count() == 1 && m.ElementAt(0).Value == injected.Value))).Returns(violations);
            }
            else
            {
                rule.Setup(a => a.FullyValidateEntity(test, M.It.Is<Mocks>(m => m.Count() == 0))).Returns(violations);
            }

            var di = new M.Mock<IRuleDependencies>();
            di.Setup(d => d.GetDependency(dependencyName, dependencyType, rule.Object)).Returns(injected);

            var subject = new RuleWrapper(rule.Object, test, () => di.Object);

            // act
            var result1 = subject.Result(useDI);
            var result2 = subject.Result(useDI);

            // assert
            Assert.IsTrue(result1 == result2);
        }
    }
}
