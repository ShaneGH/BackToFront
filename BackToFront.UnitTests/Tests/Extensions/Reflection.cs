using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using BackToFront.Extensions.Reflection;

using NUnit.Framework;

namespace BackToFront.UnitTests.Tests.Extensions
{
    [TestFixture]
    public class Reflection_Tests
    {
        class TestClass1 { }
        class TestClass2 : TestClass1 { }

        [Test]
        public void Is_Test()
        {
            // arrange            
            // act
            // assert
            Assert.IsTrue(typeof(TestClass1).Is(typeof(TestClass1)));
            Assert.IsTrue(typeof(TestClass2).Is(typeof(TestClass1)));
            Assert.IsFalse(typeof(string).Is(typeof(TestClass1)));
        }
    }
}
