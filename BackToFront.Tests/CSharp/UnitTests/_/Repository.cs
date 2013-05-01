using BackToFront.Dependency;
using BackToFront.Framework;
using NUnit.Framework;
using System.Linq;
using M = Moq;

namespace BackToFront.Tests.CSharp.UnitTests
{
    [TestFixture]
    public partial class Repository_Tests : Base.TestBase
    {
        [Test]
        public void Rules_Test_Generic_NonGeneric_and_Caching()
        {
            // arrange
            Repository subject = new Repository();

            // act
            var r1 = subject.Rules(typeof(object));
            var r2 = subject.Rules<object>();

            // assert
            Assert.NotNull(r1);
            Assert.AreEqual(r1, r2);
        }

        [Test]
        public void Add_Test()
        {
            // arrange
            Repository subject = new Repository();
            Rule<object> rule = null;

            // act
            subject.AddRule<object>(a => rule = (Rule<object>)a);

            // assert
            Assert.NotNull(rule);
            Assert.IsTrue(subject.Rules<object>().Contains(rule));
        }

        [Test]
        public void Add_Test_1Generic()
        {
            // arrange
            var di = new M.Mock<IDependencyResolver>();
            Repository subject = new Repository();
            Rule<object> rule = null;
            DependencyWrapper<object> dep = null;

            // act
            subject.AddRule<object, object>((a, b) => { rule = (Rule<object>)a; dep = b; });

            // assert
            Assert.NotNull(rule);
            Assert.NotNull(dep);
            Assert.IsTrue(subject.Rules<object>().Contains(rule));
        }

        [Test]
        public void HasRulesTest()
        {
            // arrange
            Repository r = new Repository();

            Assert.IsFalse(r.HasRules(typeof(object)));
            Assert.IsFalse(r.HasRules<object>());

            r.AddRule<object>(rule => { });

            Assert.IsTrue(r.HasRules(typeof(object)));
            Assert.IsTrue(r.HasRules<object>());
        }
    }
}
