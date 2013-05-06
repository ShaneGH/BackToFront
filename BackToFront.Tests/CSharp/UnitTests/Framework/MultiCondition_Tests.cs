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
using BackToFront.Tests.Base;

namespace BackToFront.Tests.CSharp.UnitTests.Framework
{
    [TestFixture]
    public class MultiCondition_Tests : TestBase
    {
        class TestClass<TEntity> : MultiCondition<TEntity>
        {
            public TestClass()
                : base(null) { }

            public Expression __NewCompile(SwapPropVisitor visitor)
            {
                return _Compile(visitor);
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
            Assert.AreEqual(actual.First().If.WrappedExpression, exp.Body);
            Assert.AreEqual(actual.First().EntityParameter, exp.Parameters.First());
            Assert.AreEqual(actual.First().Action, result);
        }

        [Test]
        public void _NewCompile_Test()
        {
            // arrange
            var subject = new TestClass<object>();
            Expression<Func<object, bool>> e1 = a => true;
            Expression<Func<object, bool>> e2 = a => true;
            Expression<Func<object, bool>> e3 = a => true;
            RequireOperator<object> r1 = subject.Add(e1);
            RequireOperator<object> r2 = subject.Add(e2);
            RequireOperator<object> r3 = subject.Add(e3);
            var asArray = new []
            {
                new Tuple<Expression, RequireOperator<object>>(e1.Body, r1),
                new Tuple<Expression, RequireOperator<object>>(e2.Body, r2),
                new Tuple<Expression, RequireOperator<object>>(e3.Body, r3)
            };

            // act
            ConditionalExpression actual = subject.__NewCompile(new SwapPropVisitor(typeof(object))) as ConditionalExpression;

            // assert
            Func<ConditionalExpression, Expression, bool, RequireOperator<object>, ConditionalExpression> assert = (ex, currentIf, isLast, result) =>
                {
                    Assert.AreEqual(currentIf, ex.Test);

                    // compensate for NewCompile (not _NewCompile)
                    Assert.IsInstanceOf<DefaultExpression>(((ConditionalExpression)ex.IfTrue).IfTrue);
                    if (isLast)
                    {
                        Assert.IsInstanceOf<DefaultExpression>(ex.IfFalse);
                        return null;
                    }
                    else
                    {
                        Assert.IsInstanceOf<ConditionalExpression>(ex.IfFalse);
                        return (ConditionalExpression)ex.IfFalse;
                    }
                };
            
            for (var i = 0; i < 3; i++)
                actual = assert(actual, asArray[i].Item1, i + 1 >= asArray.Length, asArray[i].Item2);
        }
    }
}
