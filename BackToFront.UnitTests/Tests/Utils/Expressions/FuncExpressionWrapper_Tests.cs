using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

using BackToFront.UnitTests.Utilities;
using BackToFront.Utils.Expressions;
using NUnit.Framework;

namespace BackToFront.UnitTests.Tests.Utils.Expressions
{
    [TestFixture]
    public class FuncExpressionWrapper_Tests : Base.TestBase
    {
        [Test]
        public void IsSameExpression_Test()
        {
            // arrange
            Expression<Func<object, object>> func1 = a => a.ToString();
            Expression<Func<object, object>> func2 = a => a.ToString();
            Expression<Func<object, object>> func3 = a => a.GetType();
            var subject = new FuncExpressionWrapper<object, object>(func1);

            // act
            // assert
            Assert.IsTrue(subject.IsSameExpression(new FuncExpressionWrapper<object, object>(func1)));
            Assert.IsTrue(subject.IsSameExpression(new FuncExpressionWrapper<object, object>(func2)));
            Assert.IsFalse(subject.IsSameExpression(new FuncExpressionWrapper<object, object>(func3)));
        }
    }
}
