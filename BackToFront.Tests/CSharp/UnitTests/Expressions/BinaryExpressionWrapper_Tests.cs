using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

using BackToFront.Utilities;
using BackToFront.Tests.Utilities;
using BackToFront.Expressions;
using NUnit.Framework;
using BackToFront.Expressions.Visitors;

using M = Moq;

namespace BackToFront.Tests.CSharp.UnitTests.Expressions
{
    [TestFixture]
    public class BinaryExpressionWrapper_Tests : Base.TestBase
    {
        public class TestClass : BinaryExpressionWrapper
        {
            public TestClass(BinaryExpression expression)
                : base(expression)
            {
            }

            public IEnumerable<MemberChainItem> __GetMembersForParameter(ParameterExpression p)
            {
                return base._GetMembersForParameter(p);
            }
        }

        [Test]
        public void IsSameExpression_Test()
        {
            // arrange
            Expression<Func<int, bool>> func1 = a => a == 4;
            Expression<Func<int, bool>> func2 = a => a == 4;
            Expression<Func<int, bool>> func3 = a => a == 5;
            Expression<Func<int, bool>> func4 = a => a != 4;
            var subject = ExpressionWrapperBase.ToWrapper(func1) as BinaryExpressionWrapper;

            // act
            // assert
            Assert.IsTrue(subject.IsSameExpression(func1.Body));
            Assert.IsTrue(subject.IsSameExpression(func2.Body));
            Assert.IsFalse(subject.IsSameExpression(func3.Body));
            Assert.IsFalse(subject.IsSameExpression(func4.Body));
        }

        [Test]
        public void _GetMembersForParameter_Test()
        {
            // arange
            var param = Expression.Parameter(typeof(int));
            var item2 = Expression.Call(param, typeof(int).GetMethod("GetHashCode"));
            var subject = new TestClass(Expression.Add(param, item2));
            
            // act
            var actual = subject.__GetMembersForParameter(param);
            var expected = new MemberChainItem[] 
            { 
                new ParameterExpressionWrapper(param).GetMembersForParameter(param).ElementAt(0), 
                new MethodCallExpressionWrapper(item2).GetMembersForParameter(param).ElementAt(0) 
            };

            // assert
            Assert.IsTrue(AreKindOfEqual(expected, actual, (a, b) => a.Equals(b)));
        }

        [Test]
        public void UnorderedParameters_Test()
        {
            // arange
            var param1 = Expression.Parameter(typeof(int));
            var param2 = Expression.Parameter(typeof(int));

            var mockedExp = Expression.Add(param1, param2);
            var testExp = Expression.Add(mockedExp, param1);
            var subject = new TestClass(testExp);

            // act
            // assert
            Assert.IsTrue(AreKindOfEqual(subject.UnorderedParameters, new[] { param1, param2 }));
        }
    }
}
