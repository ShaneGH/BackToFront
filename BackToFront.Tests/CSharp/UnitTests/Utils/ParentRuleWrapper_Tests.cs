using System;
using System.Collections.Generic;
using System.Linq;
using BackToFront.Dependency;
using BackToFront.Framework;
using BackToFront.Framework.Base;
using BackToFront.Utilities;
using NUnit.Framework;
using M = Moq;

namespace BackToFront.Tests.UnitTests.Utils
{
    [TestFixture]
    public class ParentRuleWrapper_Tests : Base.TestBase
    {
        public static bool first = true;
        public class TestClass { }
        public class TestClassChild : TestClass { }

        public class TestRule<T> : IRuleValidation<T>
        {
            public static IEnumerable<AffectedMembers> AffectedMembersStatic = new AffectedMembers[0];
            public static bool PropertyRequirementStatic = true;


            public IViolation Violation { get; set; }

            public List<DependencyWrapper> Dependencies
            {
                get { throw new NotImplementedException(); }
            }

            public IViolation ValidateEntity(T subject, ValidationContext context)
            {
                return Violation;
            }

            public void FullyValidateEntity(T subject, IList<IViolation> violationList, ValidationContext context)
            {
                violationList.Add(Violation);
            }

            public IEnumerable<AffectedMembers> AffectedMembers
            {
                get { return AffectedMembersStatic; }
            }

            public bool PropertyRequirement
            {
                get { return PropertyRequirementStatic; }
            }

            public IViolation ValidateEntity(object subject, Mocks mocks)
            {
                return Violation;
            }

            public IEnumerable<IViolation> FullyValidateEntity(object subject, Mocks mocks)
            {
                return new[] { Violation };
            }
        }

        public override void Setup()
        {
            base.Setup();

            if (first)
                Rules<TestClass>.Repository.Add((rule) => { });

            // added to static dictionary
            first = false;
        }

        [Test]
        public void PropertyRequirement_Test()
        {
            // arrange
            var rule = new TestRule<TestClass>();
            var subject = new ParentRuleWrapper<TestClass>(rule);

            // act
            // assert
            Assert.AreEqual(rule.PropertyRequirement, subject.PropertyRequirement);
        }

        [Test]
        public void ValidateEntity_Test_Generic()
        {
            // arrange
            var testClass = new TestClassChild();
            var mocks = new ValidationContext { Mocks = new Mocks() };
            var violation = new M.Mock<IViolation>();
            var rule = new TestRule<TestClass>() { Violation = violation.Object };

            var subject = new ParentRuleWrapper<TestClassChild>(rule);

            // act
            var result = subject.ValidateEntity(testClass, mocks);

            // assert
            Assert.AreEqual(violation.Object, result);
        }

        [Test]
        public void FullyValidateEntity_Test_Generic()
        {
            // arrange
            var testClass = new TestClassChild();
            var mocks = new ValidationContext { Mocks = new Mocks() };
            var violation = new M.Mock<IViolation>().Object;
            List<IViolation> violations = new List<IViolation>();
            var rule = new TestRule<TestClass>() { Violation = violation };
            var subject = new ParentRuleWrapper<TestClassChild>(rule);

            // act
            subject.FullyValidateEntity(testClass, violations, mocks);

            // assert
            Assert.AreEqual(1, violations.Count);
            Assert.AreEqual(violation, violations[0]);
        }

        [Test]
        public void ValidateEntity_Test_NonGeneric()
        {
            // arrange
            var testClass = new TestClassChild();
            var mocks = new Mocks();
            var violation = new M.Mock<IViolation>();
            var rule = new TestRule<TestClass>() { Violation = violation.Object };

            var subject = new ParentRuleWrapper<TestClassChild>(rule);

            // act
            var result = subject.ValidateEntity((object)testClass, mocks);

            // assert
            Assert.AreEqual(violation.Object, result);
        }

        [Test]
        public void FullyValidateEntity_Test_NonGeneric()
        {
            // arrange
            var testClass = new TestClassChild();
            var mocks = new Mocks();
            var violation = new M.Mock<IViolation>().Object;
            var rule = new TestRule<TestClass>() { Violation = violation };
            var subject = new ParentRuleWrapper<TestClassChild>(rule);

            // act
            var violations = subject.FullyValidateEntity((object)testClass, mocks);

            // assert
            Assert.AreEqual(1, violations.Count());
            Assert.AreEqual(violation, violations.ElementAt(0));
        }
    }
}
