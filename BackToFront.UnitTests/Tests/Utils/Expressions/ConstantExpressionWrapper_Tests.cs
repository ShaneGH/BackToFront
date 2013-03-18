using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

using BackToFront.Utils;
using BackToFront.UnitTests.Utilities;
using BackToFront.Utils.Expressions;
using NUnit.Framework;

namespace BackToFront.UnitTests.Tests.Utils.Expressions
{
    [TestFixture]
    public class ConstantExpressionWrapper_Tests : Base.TestBase
    {
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
        public void EvaluateTest()
        {            
            // arange
            var subject = ExpressionWrapperBase.ToWrapper<object, int>(a => 4);
            var ex = Mock.Create<object, int>(a => 4, 5);

            // act
            // assert            
            Assert.AreEqual(4, subject.CompileAndCall<object, int>(null));
            Assert.AreEqual(5, subject.CompileAndCall<object, int>(null, new[] { ex }));
        }

        [Test]
        public void DeepEvaluateTest()
        {
            // arange
            var subject = ExpressionWrapperBase.ToWrapper<object, string>(a => 4.ToString());
            var ex = Mock.Create<object, int>(a => 4, 5);

            // act
            // assert            
            Assert.AreEqual(4.ToString(), subject.CompileAndCall<object, string>(null));
            Assert.AreEqual(5.ToString(), subject.CompileAndCall<object, string>(null, new[] { ex }));
        }
    }
}
