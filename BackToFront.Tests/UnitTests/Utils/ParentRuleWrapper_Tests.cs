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

namespace BackToFront.Tests.UnitTests.Utils
{
    [TestFixture]
    public class ParentRuleWrapper_Tests : Base.TestBase
    {
        public static bool first = true;
        public class TestClass { }
        public class TestClassChild : TestClass { }

        public class TestRule<T> : IRuleXXX<T>
        {
            public IViolation Violation { get; set; }

            public List<DependencyWrapper> Dependencies
            {
                get { throw new NotImplementedException(); }
            }

            public IViolation ValidateEntity(T subject, Mocks mocks)
            {
                return Violation;
            }

            public void FullyValidateEntity(T subject, IList<IViolation> violationList, Mocks mocks)
            {
                violationList.Add(Violation);
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
        [ExpectedException(typeof(AccessViolationException))]
        public void Constructor_InvalidType()
        {
            // arrange
            // act
            // assert
            new ParentRuleWrapper<TestClass>(typeof(string), null);
        }

        [Test]
        [ExpectedException(typeof(InvalidOperationException))]
        public void Constructor_InvalidRule()
        {
            // arrange
            // act
            // assert
            new ParentRuleWrapper<TestClassChild>(typeof(TestClass), new Rule<string>());
        }

        [Test]
        public void Dependencies_Test()
        {
            // arrange
            var rule = new Rule<TestClass>();
            rule.Dependencies.Add(new DependencyWrapper<TestClass>("Hello"));
            var subject = new ParentRuleWrapper<TestClassChild>(typeof(TestClass), rule);

            // act
            // assert
            Assert.AreEqual(rule.Dependencies, subject.Dependencies);
        }

        [Test]
        public void ValidateEntity_Test()
        {
            // arrange
            var testClass = new TestClassChild();
            var mocks = new Mocks();
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
            var mocks = new Mocks();
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
