using BackToFront.Dependency;
using BackToFront.Framework;
using NUnit.Framework;
using System.Linq;

namespace BackToFront.Tests.UnitTests
{
    [TestFixture]
    public partial class Rules_Tests : Base.TestBase
    {
        public static bool notFirst = false;
        public class TestClass1 { }
        public class TestClass2 : TestClass1 { }

        public override void TestFixtureSetUp()
        {
            base.TestFixtureSetUp();

            Rules<TestClass1>.AddRule(a => { });
        }

        public override void Setup()
        {
            base.Setup();

            if (notFirst)
                Assert.IsTrue(Rules<TestClass2>.Repository.Registered.Any());
            else
                Assert.IsFalse(Rules<TestClass2>.Repository.Registered.Any());

            // added to static dictionary
            notFirst = true;
        }

        [Test]
        public void ParentClassRepositories_Test()
        {
            // arrange
            // act
            var parent = Rules<TestClass2>.ParentClassRepositories;

            // assert
            Assert.NotNull(parent);
            Assert.AreEqual(2, parent.Count());
            Assert.AreEqual(typeof(TestClass1), parent.First().EntityType);
            Assert.AreEqual(typeof(object), parent.Last().EntityType);
        }

        [Test]
        public void Add_Test()
        {
            // arrange
            Rule<TestClass2> rule = null;

            // act
            Rules<TestClass2>.AddRule(a => rule = (Rule<TestClass2>)a);

            // assert
            Assert.NotNull(rule);
            Assert.IsTrue(Rules<TestClass2>.Repository.Registered.Contains(rule));
        }

        [Test]
        public void Add_Test_1Generic()
        {
            // arrange
            Rule<TestClass2> rule = null;
            DependencyWrapper<object> dependency = null;

            // act
            Rules<TestClass2>.AddRule<object>((a, asdsad) => { rule = (Rule<TestClass2>)a; dependency = asdsad; });

            // assert
            Assert.NotNull(rule);
            Assert.NotNull(dependency);
            Assert.AreEqual("asdsad", dependency.DependencyName);
            Assert.IsTrue(Rules<TestClass2>.Repository.Registered.Contains(rule));
        }
    }
}
