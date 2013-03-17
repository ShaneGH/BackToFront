using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using NUnit.Framework;

namespace BackToFront.UnitTests.Tests
{
    [TestFixture]
    public class Rules_Tests
    {
        public static bool notFirst = false;
        public class TestClass { }

        [Test]
        public void Add_Test()
        {

            // arrange

            // act
            if (notFirst)
                Assert.IsTrue(Rules<TestClass>.Repository.Registered.Any());
            else
                Assert.IsFalse(Rules<TestClass>.Repository.Registered.Any());

            // added to static dictionary
            notFirst = true;

            Rules<TestClass>.Add(a => a.If(b => b == null));

            // assert
            Assert.NotNull(Rules<TestClass>.Repository.Registered.Any());
        }
    }
}
