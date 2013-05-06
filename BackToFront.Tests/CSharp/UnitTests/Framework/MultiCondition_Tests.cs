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

            subject.Add(a => a.GetHashCode() == 6);
            subject.Add(a => a.GetHashCode() == 6);
            subject.Add(a => a.GetHashCode() == 6);

            // act
            var actual = subject.AffectedMembers;

            // assert
            Assert.AreEqual(3, actual.Count());
            Assert.IsTrue(actual.All(a => a.Member.UltimateMember == typeof(object).GetMethod("GetHashCode")));
        }

        [Test]
        [ExpectedException(typeof(ArgumentNullException))]
        public void Add_Test_No_Input()
        {
            // arrange
            var subject = new MultiCondition<object>(null);

            // act
            // assert
            subject.Add(null);
        }

        [Test]
        public void Add_Test_Ok()
        {
            // arrange
            var subject = new MultiCondition<object>(null);

            Expression<Func<object, bool>> exp = a => a.GetHashCode() == 6;

            // act
            var result = subject.Add(exp);
            var actual = subject.If;

            // assert
            Assert.AreEqual(1, actual.Count());
            Assert.AreEqual(actual.First().Item1.WrappedExpression, exp.Body);
            Assert.AreEqual(actual.First().Item2, exp.Parameters.First());
            Assert.AreEqual(actual.First().Item3, result);
        }
    }
}
