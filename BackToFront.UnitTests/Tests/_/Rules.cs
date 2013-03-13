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
                Assert.IsTrue(Rules.Repository.Registered.ContainsKey(typeof(TestClass)));
            else
                Assert.IsFalse(Rules.Repository.Registered.ContainsKey(typeof(TestClass)));

            // added to static dictionary
            notFirst = true;

            Rules.Add<TestClass>(a => a.If(b => b == null));

            // assert
            Assert.NotNull(Rules.Repository.Registered[typeof(TestClass)]);
        }
    }
}
