using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

using BackToFront.Utils;
using BackToFront.UnitTests.Utilities;
using BackToFront.Expressions;
using NUnit.Framework;

namespace BackToFront.UnitTests.Tests.Expressions
{
    [TestFixture]
    public class BinaryExpressionWrapper_Tests : Base.TestBase
    {
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
        public void EvaluateTest()
        {
            // arange
            ReadOnlyCollection<ParameterExpression> parameters;
            var subject = ExpressionWrapperBase.ToWrapper<int, bool>(a => a == 0, out parameters);
            var ex = Mock.Create<int, bool>(a => a == 0, true);

            // act
            // assert            
            Assert.IsTrue(subject.CompileAndCall<int, bool>(parameters, 0));
            Assert.IsFalse(subject.CompileAndCall<int, bool>(parameters, 1));

            Assert.IsTrue(subject.CompileAndCall<int, bool>(parameters, 0, new[] { ex }));
            Assert.IsTrue(subject.CompileAndCall<int, bool>(parameters, 1, new[] { ex }));
        }

        [Test]
        public void Deep_EvaluateTest()
        {
            // arange
            ReadOnlyCollection<ParameterExpression> parameters;
            var subject = ExpressionWrapperBase.ToWrapper<int, string>(a => (a == 0).ToString(), out parameters);
            var ex = Mock.Create<int, bool>(a => a == 0, true);

            // act
            // assert          
            Assert.AreEqual(true.ToString(), subject.CompileAndCall<int, string>(parameters, 0));
            Assert.AreEqual(false.ToString(), subject.CompileAndCall<int, string>(parameters, 1));

            Assert.AreEqual(true.ToString(), subject.CompileAndCall<int, string>(parameters, 0, new[] { ex }));
            Assert.AreEqual(true.ToString(), subject.CompileAndCall<int, string>(parameters, 1, new[] { ex }));
        }
    }
}
