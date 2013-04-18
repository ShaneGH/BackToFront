using BackToFront.Framework;
using BackToFront.Framework.Base;
using BackToFront.Tests.Utilities;
using M = Moq;
using NUnit.Framework;
using System.Collections.Generic;
using System.Linq;
using BackToFront.Utilities;

namespace BackToFront.Tests.UnitTests.Framework
{
    [TestFixture]
    public class SubRuleCollection_Tests : BackToFront.Tests.Base.TestBase
    {
        [Test]
        public void AffectedMembers_Test()
        {
            // arrange
            var subject = new SubRuleCollection<object>(null);

            var item1 = new AffectedMembers { Member = new MemberChainItem(typeof(string)) };
            var rule1 = new M.Mock<Rule<object>>(null);
            rule1.Setup(a => a.AffectedMembers).Returns(new[] { item1 });
            subject.AddSubRule(rule1.Object);

            var item2 = new AffectedMembers { Member = new MemberChainItem(typeof(string)) };
            var rule2 = new M.Mock<Rule<object>>(null);
            rule2.Setup(a => a.AffectedMembers).Returns(new[] { item2 });
            subject.AddSubRule(rule2.Object);
            
            // act
            var actual = subject.AffectedMembers;

            // assert
            Assert.IsTrue(AreKindOfEqual(new[] { item1, item2 }, actual));
        }
    }
}