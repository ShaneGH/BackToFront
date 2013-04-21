using BackToFront.Framework;
using BackToFront.Framework.Base;
using BackToFront.Tests.Utilities;
using BackToFront.Utilities;
using M = Moq;
using NUnit.Framework;
using System.Collections.Generic;
using System.Linq;
using U = BackToFront.Utilities;

namespace BackToFront.Tests.UnitTests.Framework
{
    [TestFixture]
    public class RuleCollection_Tests : BackToFront.Tests.Base.TestBase
    {
        public class TestClass<TEntity> : Rule<TEntity>
            where TEntity : class
        {
            public TestClass()
                : base(null) { }

            public IEnumerable<PathElement<TEntity>> _NextPathElements()
            {
                return NextPathElements(null, new U.ValidationContext { Mocks = new U.Mocks(new U.Mock[0]) });
            }
        }

        [Test]
        public void ValidateEntity_Test_0Violations()
        {
            // arrange
            var subject = new RuleCollection<object>();

            var input1 = new object();
            var input2 = new U.Mocks(new[] { new U.Mock(wrapperExpression: null, value: null, valueType: null, behavior: Enum.MockBehavior.MockAndSet) });

            var rule1 = new M.Mock<IValidate<object>>();
            var rule2 = new M.Mock<IValidate<object>>();
            rule1.Setup(a => a.ValidateEntity(M.It.Is<object>(b => b.Equals(input1)), M.It.Is<U.ValidationContext>(b => AreKindOfEqual(b.Mocks, input2))))
                .Returns<IViolation>(null);
            rule2.Setup(a => a.ValidateEntity(M.It.Is<object>(b => b.Equals(input1)), M.It.Is<U.ValidationContext>(b => AreKindOfEqual(b.Mocks, input2))))
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
            var input2 = new U.Mocks(new U.Mock[0]);

            IViolation v1 = new SimpleViolation();
            var rule1 = new M.Mock<IValidate<object>>();
            var rule2 = new M.Mock<IValidate<object>>();
            rule1.Setup(a => a.ValidateEntity(M.It.Is<object>(b => b.Equals(input1)), M.It.Is<U.ValidationContext>(b => AreKindOfEqual(b.Mocks, input2))))
                .Returns(v1);
            rule2.Setup(a => a.ValidateEntity(M.It.Is<object>(b => b.Equals(input1)), M.It.Is<U.ValidationContext>(b => AreKindOfEqual(b.Mocks, input2))))
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
            var input2 = new U.Mocks();

            IViolation v2 = new SimpleViolation();
            var rule1 = new M.Mock<IValidate<object>>();
            var rule2 = new M.Mock<IValidate<object>>();
            rule1.Setup(a => a.ValidateEntity(M.It.Is<object>(b => b.Equals(input1)), M.It.Is<U.ValidationContext>(b => AreKindOfEqual(b.Mocks, input2))))
                .Returns<IViolation>(null);
            rule2.Setup(a => a.ValidateEntity(M.It.Is<object>(b => b.Equals(input1)), M.It.Is<U.ValidationContext>(b => AreKindOfEqual(b.Mocks, input2))))
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
            var input2 = new U.Mocks();

            IViolation v1 = new SimpleViolation();
            IViolation v2 = new SimpleViolation();
            var rule1 = new M.Mock<IValidate<object>>();
            var rule2 = new M.Mock<IValidate<object>>();
            rule1.Setup(a => a.FullyValidateEntity(M.It.Is<object>(b => b.Equals(input1)), M.It.IsAny<IList<IViolation>>(), M.It.Is<U.ValidationContext>(b => AreKindOfEqual(b.Mocks, input2))))
                .Callback<object, IList<IViolation>, U.ValidationContext>((a, b, c) => b.Add(v1));
            rule2.Setup(a => a.FullyValidateEntity(M.It.Is<object>(b => b.Equals(input1)), M.It.IsAny<IList<IViolation>>(), M.It.Is<U.ValidationContext>(b => AreKindOfEqual(b.Mocks, input2))))
                .Callback<object, IList<IViolation>, U.ValidationContext>((a, b, c) => b.Add(v2));

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

        [Test]
        public void AffectedMembers_Test()
        {
            // arrange
            var subject = new RuleCollection<object>();

            var item1 = new AffectedMembers { Member = new MemberChainItem(typeof(string)) };
            var rule1 = new M.Mock<IValidate<object>>();
            rule1.Setup(a => a.AffectedMembers).Returns(new[] { item1 });
            subject.AddRule(rule1.Object);

            var item2 = new AffectedMembers { Member = new MemberChainItem(typeof(string)) };
            var rule2 = new M.Mock<IValidate<object>>();
            rule2.Setup(a => a.AffectedMembers).Returns(new[] { item2 });
            subject.AddRule(rule2.Object);

            // act
            var actual = subject.AffectedMembers;

            // assert
            Assert.IsTrue(AreKindOfEqual(new[] { item1, item2 }, actual));
        }
    }
}