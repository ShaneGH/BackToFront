using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

using BackToFront.Utils;
using BackToFront.UnitTests.Utilities;
using BackToFront.Expressions;
using NUnit.Framework;
using System.Collections.ObjectModel;

namespace BackToFront.UnitTests.Tests.Expressions
{
    [TestFixture]
    public class ParameterExpressionWrapper_Tests : Base.TestBase
    {
        [Test]
        public void IsSameExpression_Test()
        {
            // arrange
            Expression<Func<object, object>> func1 = a => a;
            Expression<Func<object, object>> func2 = a => a;
            var subject = ExpressionWrapperBase.ToWrapper(func1) as ParameterExpressionWrapper;

            // act
            var test1 = subject.IsSameExpression(ExpressionWrapperBase.ToWrapper(func1) as ParameterExpressionWrapper);
            var test2 = subject.IsSameExpression(ExpressionWrapperBase.ToWrapper(func2) as ParameterExpressionWrapper);

            // assert
            Assert.IsTrue(test1);
            Assert.IsTrue(test2);
        }

        [Test]
        public void EvaluateTest()
        {
            // arange
            Expression<Func<object, object>> func1 = a => a;
            ReadOnlyCollection<ParameterExpression> parameters;
            var subject = ExpressionWrapperBase.ToWrapper(func1, out parameters);
            var input1 = new object();
            var input2 = new object();
            var ex = Mock.Create<object, object>(a => a, input2);

            // act
            // assert            
            Assert.AreSame(input1, subject.CompileAndCall<object, object>(parameters, input1));
            Assert.AreNotSame(input2, subject.CompileAndCall<object, object>(parameters, input1));
            Assert.AreSame(input2, subject.CompileAndCall<object, object>(parameters, input1, new[] { ex }));
            Assert.AreNotSame(input1, subject.CompileAndCall<object, object>(parameters, input1, new[] { ex }));
        }

        [Test]
        public void Deep_EvaluateTest()
        {
            // arange
            ReadOnlyCollection<ParameterExpression> parameters;
            Expression<Func<object, int>> func1 = a => a.GetHashCode();
            var subject = ExpressionWrapperBase.ToWrapper(func1, out parameters);
            var input1 = new object();
            var input2 = new object();
            var ex = Mock.Create<object, object>(a => a, input2);

            // act
            // assert            
            Assert.AreEqual(input1.GetHashCode(), subject.CompileAndCall<object, int>(parameters, input1));
            Assert.AreEqual(input2.GetHashCode(), subject.CompileAndCall<object, int>(parameters, input1, new[] { ex }));
        }
    }
}
