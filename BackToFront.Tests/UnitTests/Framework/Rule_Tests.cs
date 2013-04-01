using BackToFront.Framework;
using BackToFront.Framework.Base;
using BackToFront.Tests.Utilities;
using Moq;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using U = BackToFront.Utils;

namespace BackToFront.Tests.UnitTests.Framework
{
    [TestFixture]
    public class Rule_Tests : BackToFront.Tests.Base.TestBase
    {
        public class TestClass<TEntity> : Rule<TEntity>
            where TEntity : class
        {
            public TestClass()
                : base(null) { }

            public IEnumerable<PathElement<TEntity>> _NextPathElements()
            {
                return NextPathElements(null, new ValidationContext { Mocks = new U.Mocks(new U.Mock[0]) });
            }
        }

        [Test]
        public void RequireThat_Test()
        {
            // arrange
            var subject = new TestClass<object>();

            // act
            var result = subject.RequireThat(a => true);
            var pe = subject._NextPathElements();

            // assert
            Assert.AreEqual(1, pe.Count(a => a != null));
            Assert.AreEqual(result, pe.First(a => a != null));
        }

        [Test]
        public void If_Test()
        {
            // arrange
            var subject = new TestClass<object>();

            // act
            var result = subject.If(a => true);
            var pe = subject._NextPathElements();

            // assert
            Assert.AreEqual(1, pe.Count(a => a != null));
            Assert.AreEqual(result, ((MultiCondition<object>)pe.First(a => a != null)).If.Last());
        }

        [Test]
        public void Else_Test()
        {
            // arrange
            var subject = new TestClass<object>();

            // act
            var result = subject.Else;
            var pe = subject._NextPathElements();

            // assert
            Assert.IsTrue(((Operator<object>)result).ConditionIsTrue(null, new U.Mocks()));
            Assert.AreEqual(1, pe.Count(a => a != null));
            Assert.AreEqual(result, ((MultiCondition<object>)pe.First(a => a != null)).If.Last());
        }

        [Test]
        public void ValidateEntity_Test()
        {
            // arrange
            var subject = new Mock<TestClass<object>>();
            var violation = new SimpleViolation();
            var input1 = new object();
            var input2 = new  U.Mocks();
            subject.Setup(a => a.ValidateEntity(It.Is<object>(b => b.Equals(input1)), It.Is<ValidationContext>(b => b.Mocks == input2))).Returns(violation);

            // act
            var result = ((IValidate)subject.Object).ValidateEntity(input1, input2);

            // assert
            Assert.AreEqual(violation, result);
        }

        [Test]
        public void FullyValidateEntity_Test()
        {
            // arrange
            var subject = new Mock<TestClass<object>>();
            var violation = new SimpleViolation();
            var input1 = new object();
            var input2 = new U.Mocks();
            subject.Setup(a => a.FullyValidateEntity(It.Is<object>(b => b.Equals(input1)), It.IsAny<IList<IViolation>>(), It.Is<ValidationContext>(b => b.Mocks == input2)))
                .Callback<object, IList<IViolation>, ValidationContext>((a, b, c) => b.Add(violation));

            // act
            var result = ((IValidate)subject.Object).FullyValidateEntity(input1, input2);

            // assert
            Assert.AreEqual(1, result.Count());
            Assert.AreEqual(violation, result.First());
        }

        [Test]
        [ExpectedException(typeof(InvalidOperationException))]
        public void FullyValidateEntity_Test_InvalidOperation()
        {
            // arrange
            var subject = new TestClass<string>();

            // act
            var result = ((IValidate)subject).FullyValidateEntity(76, null);
        }

        [Test]
        [ExpectedException(typeof(InvalidOperationException))]
        public void ValidateEntity_Test_InvalidOperation()
        {
            // arrange
            var subject = new TestClass<string>();

            // act
            var result = ((IValidate)subject).ValidateEntity(76, null);
        }
    }
}
