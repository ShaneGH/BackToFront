using BackToFront.Dependency;
using BackToFront.Framework;
using NUnit.Framework;
using System;
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
            Domain subject = new Domain();

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
            Domain subject = new Domain();
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
            Domain subject = new Domain();
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
            Domain r = new Domain();

            Assert.IsFalse(r.HasRules(typeof(object)));
            Assert.IsFalse(r.HasRules<object>());

            r.AddRule<object>(rule => { });

            Assert.IsTrue(r.HasRules(typeof(object)));
            Assert.IsTrue(r.HasRules<object>());
        }

        [Test]
        public void IdentifierFor_Generic_NonGeneric_Cache()
        {
            // arrange
            Domain r = new Domain();

            // act
            var id1 = r.IdentifierFor(typeof(int));
            var id2 = r.IdentifierFor(typeof(int));

            // assert
            Assert.AreNotEqual(Guid.Empty, id1);
            Assert.AreEqual(id1, id2);
        }
    }
}
