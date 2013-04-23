using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

using BackToFront.Extensions;

using BackToFront.Utilities;
using BackToFront.Logic;
using BackToFront.Framework.Base;
using NUnit.Framework;

using M = Moq;

using BackToFront.Tests.Utilities;
using BackToFront.Framework;

namespace BackToFront.Tests.UnitTests.Framework.Base
{
    [TestFixture]
    public class PathElementTests
    {
        public abstract class PathElement_Test<T> : PathElement<T>
        {
            public PathElement_Test()
                : base(null)
            { }

            public TOutput _Do<TOutput>(Func<TOutput> action)
            {
                return Do(action);
            }

            public void _Do(Action action)
            {
                Do(action);
            }
        }

        [Test]
        public void NextOption_Test_no_Options()
        {
            // arrange
            var entity = new object();
            var mocks = new ValidationContext { Mocks = new Mocks(new Mocks(new Mock[0])) };
            var subject = new M.Mock<PathElement<object>>(null);
            subject.Setup(a => a.NextPathElements(M.It.Is<object>(b => b == entity), M.It.Is<ValidationContext>(b => b == mocks)))
                .Returns(() =>
                {
                    return new PathElement<object>[] { null, null, null, null };
                });

            // act
            var result = subject.Object.NextOption(entity, mocks);

            // assert
            Assert.IsInstanceOf<DeadEnd<object>>(result);
        }

        [Test]
        public void NextOption_Test_1_Option()
        {
            // arrange
            var v = new SimpleIValidate { Violation = new TestViolation("violation") };
            var entity = new object();
            var mocks = new ValidationContext { Mocks = new Mocks(new Mocks(new Mock[0])) };
            var subject = new M.Mock<PathElement<object>>(null);
            subject.Setup(a => a.NextPathElements(M.It.Is<object>(b => b == entity), M.It.Is<ValidationContext>(b => b == mocks)))
                .Returns(() =>
                {
                    return new[] { null, null, v, null };
                });

            // act
            var result = subject.Object.NextOption(entity, mocks);

            // assert
            Assert.AreEqual(v, result);
        }

        [Test]
        [ExpectedException(typeof(InvalidOperationException))]
        public void NextOption_Test_2_Options()
        {
            // arrange
            var entity = new object();
            var mocks = new ValidationContext { Mocks = new Mocks(new Mocks(new Mock[0])) };
            var subject = new M.Mock<PathElement<object>>(null);
            subject.Setup(a => a.NextPathElements(M.It.Is<object>(b => b == entity), M.It.Is<ValidationContext>(b => b == mocks)))
                .Returns(() =>
                {
                    return new[] 
                    { 
                        new SimpleIValidate { Violation = new TestViolation("violation") }, 
                        new SimpleIValidate { Violation = new TestViolation("violation") } 
                    };
                });

            // act
            var result = subject.Object.NextOption(entity, mocks);
        }

        [Test]
        public void ValidateEntity_Test_NextHasValue()
        {
            // arrange
            var entity = new object();
            var mocks = new ValidationContext { Mocks = new Mocks(new Mocks(new Mock[0])) };
            var nextElement = new M.Mock<PathElement<object>>(null);
            var violation = new M.Mock<IViolation>().Object;
            nextElement.Setup(a => a.ValidateEntity(M.It.Is<object>(b => b == entity), M.It.Is<ValidationContext>(b => b == mocks)))
                .Returns(() => violation);

            var subject = new M.Mock<PathElement<object>>(null) { CallBase = true };
            subject.Setup(a => a.NextPathElements(M.It.Is<object>(b => b == entity), M.It.Is<ValidationContext>(b => b == mocks)))
                .Returns(() =>
                {
                    return new[] { nextElement.Object };
                });

            // act
            var result = subject.Object.ValidateEntity(entity, mocks);

            // assert
            Assert.AreEqual(violation, result);
        }

        [Test]
        public void ValidateEntity_Test_NextDoesNotHaveValue()
        {
            // arrange
            var entity = new object();
            var mocks = new ValidationContext { Mocks = new Mocks(new Mocks(new Mock[0])) };

            var subject = new M.Mock<PathElement<object>>(null) { CallBase = true };
            subject.Setup(a => a.NextPathElements(M.It.Is<object>(b => b == entity), M.It.Is<ValidationContext>(b => b == mocks)))
                .Returns(() =>
                {
                    return new PathElement<object>[0];
                });

            // act
            var result = subject.Object.ValidateEntity(entity, mocks);

            // assert
            Assert.IsNull(result);
        }

        [Test]
        public void FullyValidateEntity_Test_NextHasValue()
        {
            // arrange
            var entity = new object();
            var mocks = new ValidationContext { Mocks = new Mocks(new Mocks(new Mock[0])) };
            var nextElement = new M.Mock<PathElement<object>>(null);
            var violation1 = new M.Mock<IViolation>().Object;
            var violation2 = new M.Mock<IViolation>().Object;
            nextElement.Setup(a => a.FullyValidateEntity(M.It.Is<object>(b => b == entity), M.It.IsAny<IList<IViolation>>(), M.It.Is<ValidationContext>(b => b == mocks)))
                .Callback<object, IList<IViolation>, ValidationContext>((a, b, c) => { b.Add(violation1); b.Add(violation2); });

            var subject = new M.Mock<PathElement<object>>(null) { CallBase = true };
            subject.Setup(a => a.NextPathElements(M.It.Is<object>(b => b == entity), M.It.Is<ValidationContext>(b => b == mocks)))
                .Returns(() =>
                {
                    return new[] { nextElement.Object };
                });

            // act
            List<IViolation> result = new List<IViolation>();
            subject.Object.FullyValidateEntity(entity, result, mocks);

            // assert
            Assert.AreEqual(2, result.Count);
            Assert.IsTrue(result.Contains(violation1));
            Assert.IsTrue(result.Contains(violation2));
        }

        [Test]
        public void FullyValidateEntity_Test_NextDoesNotHaveValue()
        {
            // arrange
            var entity = new object();
            var mocks = new ValidationContext { Mocks = new Mocks(new Mocks(new Mock[0])) };

            var subject = new M.Mock<PathElement<object>>(null) { CallBase = true };
            subject.Setup(a => a.NextPathElements(M.It.Is<object>(b => b == entity), M.It.Is<ValidationContext>(b => b == mocks)))
                .Returns(() =>
                {
                    return new PathElement<object>[0];
                });

            // act
            List<IViolation> result = new List<IViolation>();
            subject.Object.FullyValidateEntity(entity, result, mocks);

            // assert
            Assert.AreEqual(0, result.Count);
        }

        [Test]
        public void Do_Action()
        {
            // arrange
            var subject = new M.Mock<PathElement_Test<object>>();
            object val = new object();
            object newVal = new object();

            // act
            subject.Object._Do(() => { val = newVal; });

            // assert
            Assert.AreEqual(val, newVal);
        }

        [Test]
        public void Do_Func()
        {
            // arrange
            var subject = new M.Mock<PathElement_Test<object>>();
            object val = new object();

            // act
            var result = subject.Object._Do(() => val);

            // assert
            Assert.AreEqual(val, result);
        }

        [Test]
        public void Do_Locked()
        {
            // arrange
            var subject = new M.Mock<PathElement_Test<object>>();

            // act
            subject.Object._Do(() => { });

            // assert
            Assert.Throws<InvalidOperationException>(() => { subject.Object._Do(() => { }); });
        }
    }
}
