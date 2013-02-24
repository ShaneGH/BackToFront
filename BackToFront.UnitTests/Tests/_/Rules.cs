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
        public class TestClass { }

        [Test]
        public void Add_Test()
        {
            // arrange

            // act
            Assert.IsFalse(Rules.Repository.Registered.ContainsKey(typeof(TestClass)));
            Rules.Add<TestClass>(a => a.If(b => b));

            // assert
            Assert.NotNull(Rules.Repository.Registered[typeof(TestClass)]);
        }
    }
}
