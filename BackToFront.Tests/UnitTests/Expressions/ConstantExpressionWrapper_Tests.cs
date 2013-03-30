using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using System.Collections.ObjectModel;

using BackToFront.Utils;
using BackToFront.Tests.Utilities;
using BackToFront.Expressions;
using NUnit.Framework;

namespace BackToFront.Tests.UnitTests.Expressions
{
    [TestFixture]
    public class ConstantExpressionWrapper_Tests : Base.TestBase
    {
        public class TestClass : ConstantExpressionWrapper
        {
            public TestClass(ConstantExpression expression)
                : base(expression)
            {
            }

            public Expression _CompileInnerExpression(IEnumerable<Mock> mocks)
            {
                return CompileInnerExpression(mocks);
            }
        }

        [Test]
        public void IsSameExpression_Test()
        {
            // arrange
            Expression<Func<object, int>> func1 = a => 4;
            Expression<Func<object, int>> func2 = a => 4;
            Expression<Func<object, int>> func3 = a => 5;
            var subject = ExpressionWrapperBase.ToWrapper(func1) as ConstantExpressionWrapper;

            // act
            // assert
            Assert.IsTrue(subject.IsSameExpression(ExpressionWrapperBase.ToWrapper(func1) as ConstantExpressionWrapper));
            Assert.IsTrue(subject.IsSameExpression(ExpressionWrapperBase.ToWrapper(func2) as ConstantExpressionWrapper));
            Assert.IsFalse(subject.IsSameExpression(ExpressionWrapperBase.ToWrapper(func3) as ConstantExpressionWrapper));
        }

        [Test]
        public void CompileInnerExpression_Test()
        {
            // arange
            var subject = new TestClass(Expression.Constant(4));

            // act
            var result = subject._CompileInnerExpression(Enumerable.Empty<Mock>());

            // assert
            Assert.AreEqual(subject.Expression, result);
        }
    }
}
