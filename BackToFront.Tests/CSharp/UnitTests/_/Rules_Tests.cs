using BackToFront.Dependency;
using BackToFront.Framework;
using NUnit.Framework;
using System.Linq;

namespace BackToFront.Tests.CSharp.UnitTests
{
    [TestFixture]
    public partial class Rules_Tests : Base.TestBase
    {
        [Test]
        public void EntityType_Test()
        {
            // arrange
            var subject1 = new Rules<object>();
            var subject2 = new Rules<int>();

            // act
            // assert
            Assert.AreEqual(typeof(object), subject1.EntityType);
            Assert.AreEqual(typeof(int), subject2.EntityType);
        }

        [Test]
        public void Add_Test()
        {
            // arrange
            var subject = new Rules<object>();
            Rule<object> rule = null;

            // act
            subject.Add(a => rule = (Rule<object>)a);

            // assert
            Assert.NotNull(rule);
            Assert.IsTrue(subject.Contains(rule));
        }

        [Test]
        public void Add_Test_1Generic()
        {
            // arrange
            var subject = new Rules<object>();
            Rule<object> rule = null;
            DependencyWrapper<object> dep = null;

            // act
            subject.Add<object>((a, b) => { rule = (Rule<object>)a; dep = b; });

            // assert
            Assert.NotNull(rule);
            Assert.NotNull(dep);
            Assert.IsTrue(subject.Contains(rule));
        }
    }
}
