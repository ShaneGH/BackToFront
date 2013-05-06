using BackToFront.Framework;
using BackToFront.Framework.Base;
using BackToFront.Tests.Utilities;
using M = Moq;
using NUnit.Framework;
using System.Collections.Generic;
using System.Linq;
using BackToFront.Utilities;
using System.Linq.Expressions;
using BackToFront.Expressions.Visitors;
using BackToFront.Expressions;

namespace BackToFront.Tests.CSharp.UnitTests.Framework
{
    [TestFixture]
    public class SubRuleCollection_Tests : BackToFront.Tests.Base.TestBase
    {
        public class Accessor : SubRuleCollection<object>
        {
            public Accessor()
                : base(null) { }

            public Expression __Compile(SwapPropVisitor v)
            {
                return _Compile(v);
            }
        }

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

        [Test]
        public void _Compile_Test()
        {
            // arrange
            var subject = new Accessor();
            subject.If(a => true);
            var v = new SwapPropVisitor(typeof(object));
            
            // act
            var actual = ExpressionWrapperBase.CreateChildWrapper(subject.__Compile(v));
            var expected = ((RuleCollection<object>)GetPrivateProperty(subject, typeof(SubRuleCollection<object>), "_subRules")).Compile(v);

            // act
            Assert.IsTrue(actual.IsSameExpression(expected));
        }
    }
}