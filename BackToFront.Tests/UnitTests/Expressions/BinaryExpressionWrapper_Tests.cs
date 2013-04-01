using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

using BackToFront.Utils;
using BackToFront.Tests.Utilities;
using BackToFront.Expressions;
using NUnit.Framework;

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

            public Expression _CompileInnerExpression(IEnumerable<Mock> mocks)
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
            Assert.IsTrue(subject.IsSameExpression(ExpressionWrapperBase.ToWrapper(func1) as BinaryExpressionWrapper));
            Assert.IsTrue(subject.IsSameExpression(ExpressionWrapperBase.ToWrapper(func2) as BinaryExpressionWrapper));
            Assert.IsFalse(subject.IsSameExpression(ExpressionWrapperBase.ToWrapper(func3) as BinaryExpressionWrapper));
            Assert.IsFalse(subject.IsSameExpression(ExpressionWrapperBase.ToWrapper(func4) as BinaryExpressionWrapper));
        }

        [Test]
        public void CompileInnerExpression_Test_nothing_mocked()
        {
            // arange
            var subject = new TestClass(
                Expression.Add(Expression.Constant(2), Expression.Constant(3)));

            // act
            var result = subject._CompileInnerExpression(Enumerable.Empty<Mock>());

            // assert
            Assert.AreEqual(subject.Expression, result);
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
            Assert.IsTrue(AreKindOfEqual(expected, actual, (a, b) => a.AreSame(b)));
        }

        [Test]
        public void CompileInnerExpression_Test_withMocks()
        {
            const int mockedVal = 99;

            // arange
            var mockedExp = Expression.Add(Expression.Constant(4), Expression.Constant(2));
            var testExp = Expression.Add(mockedExp, Expression.Constant(3));
            var subject = new TestClass(testExp);

            // act
            var result = subject._CompileInnerExpression(new[] { new Mock(mockedExp, mockedVal, mockedVal.GetType()) }) as BinaryExpression;

            // assert
            Assert.IsNotNull(result);
            Assert.AreNotEqual(subject.Expression, result);
            Assert.IsInstanceOf<UnaryExpression>(result.Left);
            Assert.AreEqual(mockedVal.GetType(), (result.Left as UnaryExpression).Type);
            Assert.AreEqual(ExpressionType.Convert, result.Left.NodeType);
            Assert.AreEqual(testExp.Right, result.Right);
            Assert.AreEqual(testExp.NodeType, result.NodeType);
            Assert.AreEqual(testExp.Method, result.Method);
        }
    }
}
