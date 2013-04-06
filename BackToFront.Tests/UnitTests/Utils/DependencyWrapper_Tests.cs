using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

using NUnit.Framework;
using BackToFront.Utils;
using M = Moq;
using BackToFront.Dependencies;

namespace BackToFront.Tests.UnitTests.Utils
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

            var subject = new DependencyWrapper<string>("Hello", () => di.Object);

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

            var subject = new DependencyWrapper<string>("Hello", () => di.Object);

            // act
            var actual = subject.Val;

            // assert
            Assert.AreEqual(actual, expected);
        }
    }
}