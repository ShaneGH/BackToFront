using BackToFront.Framework;
using BackToFront.Framework.Base;
using BackToFront.Tests.Utilities;
using BackToFront.Utilities;
using M = Moq;
using NUnit.Framework;
using System.Collections.Generic;
using System.Linq;
using U = BackToFront.Utilities;
using BackToFront.Validation;
using BackToFront.Expressions.Visitors;
using BackToFront.Dependency;
using System.Linq.Expressions;
using BackToFront.Expressions;

namespace BackToFront.Tests.CSharp.UnitTests.Framework
{
    [TestFixture]
    public class RuleCollection_Tests : BackToFront.Tests.Base.TestBase
    {
        public class TestClass<TEntity> : Rule<TEntity>
            where TEntity : class
        {
            public TestClass()
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
            var subject = new RuleCollection<object>();

            var item1 = new AffectedMembers { Member = new MemberChainItem(typeof(string)) };
            var rule1 = new M.Mock<IValidate<object>>();
            rule1.Setup(a => a.AffectedMembers).Returns(new[] { item1 });
            subject.AddRule(rule1.Object);

            var item2 = new AffectedMembers { Member = new MemberChainItem(typeof(string)) };
            var rule2 = new M.Mock<IValidate<object>>();
            rule2.Setup(a => a.AffectedMembers).Returns(new[] { item2 });
            subject.AddRule(rule2.Object);

            // act
            var actual = subject.AffectedMembers;

            // assert
            Assert.IsTrue(AreKindOfEqual(new[] { item1, item2 }, actual));
        }

        [Test]
        public void _Compile_Test()
        {
            // arrange
            var subject = new RuleCollection<object>();
            subject.AddRule(new Rule<object>());
            subject.AddRule(new Rule<object>());

            var v = new SwapPropVisitor(typeof(object));

            // act
            var actual = ExpressionWrapperBase.CreateChildWrapper(subject.Compile(v)) as BlockExpressionWrapper;
            var expected = Expression.Block(subject.Rules.Select(r => r.Compile(v)));

            // act
            Assert.AreEqual(2, actual.Expression.Expressions.Count);
            Assert.IsTrue(actual.IsSameExpression(expected));
        }
    }
}