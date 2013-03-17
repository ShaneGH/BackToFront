using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using NUnit.Framework;
using BackToFront.Framework;
using BackToFront.Framework.Condition;
using BackToFront.Framework.Base;
using BackToFront.Extensions.IEnumerable;
using BackToFront.UnitTests.Utilities;
using U = BackToFront.Utils;

using Moq;

namespace BackToFront.UnitTests.Tests.Framework
{
    [TestFixture]
    public class RuleCollection_Tests : BackToFront.UnitTests.Base.TestBase
    {
        public class TestClass<TEntity> : Rule<TEntity>
            where TEntity : class
        {
            public TestClass()
                : base(null) { }

            public IEnumerable<PathElement<TEntity>> _NextPathElements()
            {
                return NextPathElements(null, Enumerable.Empty<U.Mock>());
            }
        }

        [Test]
        public void ValidateEntity_Test_0Violations()
        {
            // arrange
            var subject = new RuleCollection<object>();

            var input1 = new object();
            var input2 = Enumerable.Empty<U.Mock>();

            var rule1 = new Mock<IValidate<object>>();
            var rule2 = new Mock<IValidate<object>>();
            rule1.Setup(a => a.ValidateEntity(It.Is<object>(b => b.Equals(input1)), It.Is<IEnumerable<U.Mock>>(b => b.Equals(input2))))
                .Returns<IViolation>(null);
            rule2.Setup(a => a.ValidateEntity(It.Is<object>(b => b.Equals(input1)), It.Is<IEnumerable<U.Mock>>(b => b.Equals(input2))))
                .Returns<IViolation>(null);

            subject.AddRule(rule1.Object);
            subject.AddRule(rule2.Object);

            // act
            var result = subject.ValidateEntity(input1, input2);

            // assert
            Assert.IsNull(result);
            rule1.VerifyAll();
            rule2.VerifyAll();
        }

        [Test]
        public void ValidateEntity_Test_1Violation_first()
        {
            // arrange
            var subject = new RuleCollection<object>();

            var input1 = new object();
            var input2 = Enumerable.Empty<U.Mock>();

            IViolation v1 = new SimpleViolation();
            var rule1 = new Mock<IValidate<object>>();
            var rule2 = new Mock<IValidate<object>>();
            rule1.Setup(a => a.ValidateEntity(It.Is<object>(b => b.Equals(input1)), It.Is<IEnumerable<U.Mock>>(b => b.Equals(input2))))
                .Returns(v1);
            rule2.Setup(a => a.ValidateEntity(It.Is<object>(b => b.Equals(input1)), It.Is<IEnumerable<U.Mock>>(b => b.Equals(input2))))
                .Throws(new NUnit.Framework.AssertionException("rule2.ValidateEntity Should not be called"));

            subject.AddRule(rule1.Object);
            subject.AddRule(rule2.Object);

            // act
            var result = subject.ValidateEntity(input1, input2);

            // assert
            Assert.AreEqual(v1, result);
            rule1.VerifyAll();
        }

        [Test]
        public void ValidateEntity_Test_1Violation_second()
        {
            // arrange
            var subject = new RuleCollection<object>();

            var input1 = new object();
            var input2 = Enumerable.Empty<U.Mock>();

            IViolation v2 = new SimpleViolation();
            var rule1 = new Mock<IValidate<object>>();
            var rule2 = new Mock<IValidate<object>>();
            rule1.Setup(a => a.ValidateEntity(It.Is<object>(b => b.Equals(input1)), It.Is<IEnumerable<U.Mock>>(b => b.Equals(input2))))
                .Returns<IViolation>(null);
            rule2.Setup(a => a.ValidateEntity(It.Is<object>(b => b.Equals(input1)), It.Is<IEnumerable<U.Mock>>(b => b.Equals(input2))))
                .Returns(v2);

            subject.AddRule(rule1.Object);
            subject.AddRule(rule2.Object);

            // act
            var result = subject.ValidateEntity(input1, input2);

            // assert
            Assert.AreEqual(v2, result);
            rule1.VerifyAll();
            rule2.VerifyAll();
        }

        [Test]
        public void FullyValidateEntity_Test()
        {
            // arrange
            var subject = new RuleCollection<object>();

            var input1 = new object();
            var input2 = Enumerable.Empty<U.Mock>();

            IViolation v1 = new SimpleViolation();
            IViolation v2 = new SimpleViolation();
            var rule1 = new Mock<IValidate<object>>();
            var rule2 = new Mock<IValidate<object>>();
            rule1.Setup(a => a.FullyValidateEntity(It.Is<object>(b => b.Equals(input1)), It.IsAny<IList<IViolation>>(), It.Is<IEnumerable<U.Mock>>(b => b.Equals(input2))))
                .Callback<object, IList<IViolation>, IEnumerable<U.Mock>>((a, b, c) => b.Add(v1));
            rule2.Setup(a => a.FullyValidateEntity(It.Is<object>(b => b.Equals(input1)), It.IsAny<IList<IViolation>>(), It.Is<IEnumerable<U.Mock>>(b => b.Equals(input2))))
                .Callback<object, IList<IViolation>, IEnumerable<U.Mock>>((a, b, c) => b.Add(v2));

            subject.AddRule(rule1.Object);
            subject.AddRule(rule2.Object);

            // act
            var result = subject.FullyValidateEntity(input1, input2);

            // assert
            Assert.AreEqual(2, result.Count());
            Assert.AreEqual(v1, result.ElementAt(0));
            Assert.AreEqual(v2, result.ElementAt(1));
            rule1.VerifyAll();
            rule2.VerifyAll();
        }
    }
}