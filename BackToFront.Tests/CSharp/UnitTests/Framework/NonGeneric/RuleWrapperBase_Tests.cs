using BackToFront.Dependency;
using BackToFront.Framework;
using BackToFront.Framework.NonGeneric;
using NUnit.Framework;

namespace BackToFront.Tests.UnitTests.Framework.NonGeneric
{
    [TestFixture]
    public class RuleWrapperBase_Tests
    {
        [Test]
        public void Dependencies_Test()
        {
            // arrange
            var rule = new Rule<object>();
            rule._Dependencies.Add(new DependencyWrapper<object>("Hello", null));
            var subject = new RuleWrapperBase(rule);

            // act
            // assert
            Assert.AreEqual(rule._Dependencies, subject.Dependencies);
        }

        [Test]
        public void AffectedProperties_Test()
        {
            // arrange
            var rule = new Rule<object>();
            var subject = new RuleWrapperBase(rule);

            // act
            // assert
            Assert.AreEqual(rule.AffectedMembers, subject.AffectedMembers);
        }
    }
}
