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

namespace BackToFront.Tests.CSharp.UnitTests.Framework
{
    [TestFixture]
    public class MultiCondition_Tests : BackToFront.Tests.Base.TestBase
    {
        class TestClass<TEntity> : MultiCondition<TEntity>
        {
            public TestClass()
                : base(null) { }

            public IEnumerable<PathElement<TEntity>> _NextPathElements(TEntity subject)
            {
                return NextPathElements(subject, new ValidationContext { ExpressionModifier = new SwapPropVisitor() });
            }
        }

        [Test]
        [TestCase(false, false, false)]
        [TestCase(false, false, true)]
        [TestCase(false, true, false)]
        [TestCase(false, true, true)]
        [TestCase(true, false, false)]
        [TestCase(true, false, true)]
        [TestCase(true, true, false)]
        [TestCase(true, true, true)]
        public void NextPathElements_Test(bool cond1Result, bool cond2Result, bool cond3Result)
        {
            // arrange
            var subject = new TestClass<object>();

            var conditions = new []
            {
                new BackToFront.Framework.Operator<object>(a => cond1Result, null),
                new BackToFront.Framework.Operator<object>(a => cond2Result, null),
                new BackToFront.Framework.Operator<object>(a => cond3Result, null)
            };

            int pos = 0;
            foreach (var i in new[] { cond1Result, cond2Result, cond3Result })
                if (!i)
                    pos++;
                else
                    break;

            if (pos >= conditions.Count())
                pos = -1;

            subject.If.AddRange(conditions);

            // act
            var result = subject._NextPathElements(null);
            var index = Array.IndexOf(conditions, result.FirstOrDefault(a => a != null));

            // assert
            Assert.IsTrue(result.Count() > 0 && result.Count(a => a != null) <= 1);
            Assert.AreEqual(pos, index);

        }

        [Test]
        public void AffectedMembers_Test()
        {
            // arrange
            var subject = new MultiCondition<object>(null);

            var conditions = new[]
            {
                new BackToFront.Framework.Operator<object>(a => a.GetHashCode() == 6, null),
                new BackToFront.Framework.Operator<object>(a => a.GetHashCode() == 6, null),
                new BackToFront.Framework.Operator<object>(a => a.GetHashCode() == 6, null)
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
