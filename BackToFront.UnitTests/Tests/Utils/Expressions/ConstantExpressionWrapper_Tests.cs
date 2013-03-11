﻿using System;
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
    public class ConstantExpressionWrapper_Tests : Base.TestBase
    {
        [Test]
        public void IsSameExpression_Test()
        {
            // arrange
            Expression<Func<object, int>> func1 = a => 4;
            Expression<Func<object, int>> func2 = a => 4;
            Expression<Func<object, int>> func3 = a => 5;
            var subject = new FuncExpressionWrapper<object, int>(func1).Body as ConstantExpressionWrapper;

            // act
            // assert
            Assert.IsTrue(subject.IsSameExpression(new FuncExpressionWrapper<object, int>(func1).Body as ConstantExpressionWrapper));
            Assert.IsTrue(subject.IsSameExpression(new FuncExpressionWrapper<object, int>(func2).Body as ConstantExpressionWrapper));
            Assert.IsFalse(subject.IsSameExpression(new FuncExpressionWrapper<object, int>(func3).Body as ConstantExpressionWrapper));
        }

        [Test]
        public void EvaluateTest()
        {
            // arange
            var subject = new FuncExpressionWrapper<object, int>(a => 4);
            var ex = new Tuple<Expression<Func<object, int>>, int>(a => 4, 5);

            // act
            // assert            
            Assert.AreEqual(4, subject.Evaluate(new object[0]));
            Assert.AreEqual(5, subject.Evaluate(new object[0], new[] { ex }));
        }

        [Test]
        public void DeepEvaluateTest()
        {
            // arange
            var subject = new FuncExpressionWrapper<object, string>(a => 4.ToString());
            var ex = new Tuple<Expression<Func<object, int>>, int>(a => 4, 5);

            // act
            // assert            
            Assert.AreEqual(4.ToString(), subject.Evaluate(new object[0]));
            Assert.AreEqual(5.ToString(), subject.Evaluate(new object[0], new[] { ex }));
        }
    }
}