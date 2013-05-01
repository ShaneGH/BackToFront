using BackToFront.Dependency;
using BackToFront.Utilities;
using NUnit.Framework;
using System;
using M = Moq;

namespace BackToFront.Tests.CSharp.UnitTests.Dependency
{
    [TestFixture]
    public class DependencyWrapper_Tests : Base.TestBase
    {
        [Test]
        [ExpectedException(typeof(InvalidOperationException))]
        public void Val_test_no_dependencies()
        {
            // arrange
            var di = new M.Mock<IDependencyResolver>();
            di.Setup(a => a.GetService(M.It.IsAny<Type>())).Returns(() => null);

            var subject = new DependencyWrapper<string>("Hello", di.Object);

            // act
            // assert
            var result = subject.Val;
        }

        [Test]
        public void Val_test_with_dependencies()
        {
            const string expected = "*HP*(HGG(OHGGV*&UG";

            // arrange
            var di = new M.Mock<IDependencyResolver>();
            di.Setup(a => a.GetService(M.It.IsAny<Type>())).Returns(() => expected);

            var subject = new DependencyWrapper<string>("Hello", di.Object);

            // act
            var actual = subject.Val;

            // assert
            Assert.AreEqual(actual, expected);
        }
    }
}