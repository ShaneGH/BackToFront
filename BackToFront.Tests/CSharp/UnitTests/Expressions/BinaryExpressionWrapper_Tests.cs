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

namespace BackToFront.Tests.UnitTests.Expressions
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

            public Expression _CompileInnerExpression(ISwapPropVisitor mocks)
            {
                return CompileInnerExpression(mocks);
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
        public void CompileInnerExpression_Test_nothing_mocked()
        {
            // arange
            var subject = new TestClass(
                Expression.Add(Expression.Constant(2), Expression.Constant(3)));
            var input = new M.Mock<ISwapPropVisitor>();
            input.Setup(a => a.ContainsNothing).Returns(true);
            input.Setup(a => a.Visit(M.It.IsAny<Expression>())).Returns<Expression>(a => a);

            // act
            var result = subject._CompileInnerExpression(input.Object);

            // assert
            Assert.AreEqual(subject.Expression, result);
        }

        [Test]
        public void CompileInnerExpression_Test_withMocks()
        {
            // arange
            var mockedVal = Expression.Constant(43534);
            var lhs = Expression.Constant(4);
            var mockedExp = Expression.Add(lhs, Expression.Constant(2));
            var subject = new TestClass(mockedExp);
            var input = new M.Mock<ISwapPropVisitor>();
            input.Setup(a => a.ContainsNothing).Returns(false);
            input.Setup(a => a.Visit(M.It.Is<Expression>(x => x == lhs))).Returns<Expression>(a => mockedVal);
            input.Setup(a => a.Visit(M.It.Is<Expression>(x => x != lhs))).Returns<Expression>(a => a);

            // act
            var result = subject._CompileInnerExpression(input.Object) as BinaryExpression;

            // assert
            Assert.IsNotNull(result);
            Assert.AreNotEqual(subject.Expression, result);
            Assert.AreEqual(mockedVal, result.Left);
            Assert.AreEqual(mockedExp.Right, result.Right);
            Assert.AreEqual(mockedExp.NodeType, result.NodeType);
            Assert.AreEqual(mockedExp.Method, result.Method);
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
