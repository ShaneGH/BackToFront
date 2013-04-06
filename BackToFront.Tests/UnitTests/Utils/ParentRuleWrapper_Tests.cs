using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

using M = Moq;

using NUnit.Framework;
using BackToFront.Utils;
using BackToFront.Framework;
using BackToFront.Framework.Base;

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
        public void Dependencies_Test()
        {
            // arrange
            var rule = new Rule<TestClass>();
            rule._Dependencies.Add(new DependencyWrapper<TestClass>("Hello", null));
            var subject = new ParentRuleWrapper<TestClassChild>(typeof(TestClass), rule);

            // act
            // assert
            Assert.AreEqual(rule._Dependencies, subject.Dependencies);
        }

        [Test]
        public void AffectedProperties_Test()
        {
            // arrange
            var rule = new TestRule<TestClass>();
            var subject = new ParentRuleWrapper<TestClass>(typeof(TestClass), rule);

            // act
            // assert
            Assert.AreEqual(rule.AffectedMembers, subject.AffectedMembers);
        }

        [Test]
        public void PropertyRequirement_Test()
        {
            // arrange
            var rule = new TestRule<TestClass>();
            var subject = new ParentRuleWrapper<TestClass>(typeof(TestClass), rule);

            // act
            // assert
            Assert.AreEqual(rule.PropertyRequirement, subject.PropertyRequirement);
        }

        [Test]
        public void ValidateEntity_Test()
        {
            // arrange
            var testClass = new TestClassChild();
            var mocks = new ValidationContext { Mocks = new Mocks() };
            var violation = new M.Mock<IViolation>();
            var rule = new TestRule<TestClass>() { Violation = violation.Object };

            var subject = new ParentRuleWrapper<TestClassChild>(typeof(TestClass), rule);

            // act
            var result = subject.ValidateEntity(testClass, mocks);

            // assert
            Assert.AreEqual(violation.Object, result);
        }

        [Test]
        public void FullyValidateEntity_Test()
        {
            // arrange
            var testClass = new TestClassChild();
            var mocks = new ValidationContext { Mocks = new Mocks() };
            var violation = new M.Mock<IViolation>().Object;
            List<IViolation> violations = new List<IViolation>();
            var rule = new TestRule<TestClass>() { Violation = violation };
            var subject = new ParentRuleWrapper<TestClassChild>(typeof(TestClass), rule);

            // act
            subject.FullyValidateEntity(testClass, violations, mocks);

            // assert
            Assert.AreEqual(1, violations.Count);
            Assert.AreEqual(violation, violations[0]);
        }
    }
}
