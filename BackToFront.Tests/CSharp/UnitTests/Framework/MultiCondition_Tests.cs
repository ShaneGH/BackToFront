using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using M = Moq;
using NUnit.Framework;
using BackToFront.Framework;
using BackToFront.Framework.Base;
using BackToFront.Extensions.IEnumerable;
using BackToFront.Utilities;
using BackToFront.Expressions.Visitors;
using System.Linq.Expressions;

namespace BackToFront.Tests.CSharp.UnitTests.Framework
{
    [TestFixture]
    public class MultiCondition_Tests : BackToFront.Tests.Base.TestBase
    {
        class TestClass<TEntity> : MultiCondition<TEntity>
        {
            public TestClass()
                : base(null) { }

            public Expression __NewCompile(SwapPropVisitor visitor)
            {
                return _NewCompile(visitor);
            }
        }

        [Test]
        public void AffectedMembers_Test()
        {
            // arrange
            var subject = new MultiCondition<object>(null);

            var conditions = new[]
            {
                new BackToFront.Framework.RequireOperator<object>(a => a.GetHashCode() == 6, null),
                new BackToFront.Framework.RequireOperator<object>(a => a.GetHashCode() == 6, null),
                new BackToFront.Framework.RequireOperator<object>(a => a.GetHashCode() == 6, null)
            };

            subject.If.AddRange(conditions);
            var expected = subject.If.Select(i => i.AffectedMembers).Aggregate();
            
            // act
            var actual = subject.AffectedMembers;

            // assert
            Assert.IsTrue(AreKindOfEqual(expected, actual, (a, b) => a.Requirement == b.Requirement && a.Member.Equals(b.Member)));
        }
    }
}
