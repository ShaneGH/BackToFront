using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using NUnit.Framework;
using BackToFront.Utils;

namespace BackToFront.Tests.UnitTests
{
    [TestFixture]
    public class Rules_Tests : Base.TestBase
    {
        public static bool notFirst = false;
        public class TestClass { }

        public override void Setup()
        {
            base.Setup();

            if (notFirst)
                Assert.IsTrue(Rules<TestClass>.Repository.Registered.Any());
            else
                Assert.IsFalse(Rules<TestClass>.Repository.Registered.Any());

            // added to static dictionary
            notFirst = true;
        }

        [Test]
        public void Add_Test()
        {
            // arrange
            IRule<TestClass> rule = null;

            // act
            Rules<TestClass>.AddRule(a => rule = a);

            // assert
            Assert.NotNull(rule);
            Assert.IsTrue(Rules<TestClass>.Repository.Registered.Contains(rule));
        }

        [Test]
        public void Add_Test_1Generic()
        {
            // arrange
            IRule<TestClass> rule = null;
            DependencyWrapper<object> dependency = null;

            // act
            Rules<TestClass>.AddRule<object>((a, asdsad) => { rule = a; dependency = asdsad; });

            // assert
            Assert.NotNull(rule);
            Assert.NotNull(dependency);
            Assert.AreEqual("asdsad", dependency.DependencyName);
            Assert.IsTrue(Rules<TestClass>.Repository.Registered.Contains(rule));
        }
    }
}
