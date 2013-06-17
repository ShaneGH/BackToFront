using BackToFront.Expressions.Visitors;
using BackToFront.Framework;
using BackToFront.Framework.Base;
using BackToFront.Utilities;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using M = Moq;

namespace BackToFront.Tests.CSharp.UnitTests.Framework
{
    [TestFixture]
    public class RequirementFailed_Tests : BackToFront.Tests.Base.TestBase
    {
        public class Accessor : RequirementFailed<object>
        {
            public Accessor(Expression<Func<object, bool>> property)
                : base(property, null)
            {
            }

            public Expression __Compile(SwapPropVisitor visitor)
            {
                return _Compile(visitor);
            }
        }

        public class TestClass
        {
            public bool Success { get; set; }
        }

        [Test]
        public void OrModelViolationIs_Test()
        {
            // arrange
            var rule = new Rule<object>();
            var subject = new RequirementFailed<object>(a => true, rule);
            var violation = new M.Mock<IViolation>().Object;

            // act
            var result = subject.WithModelViolation(() => violation);
            var npe = subject.AllPossiblePaths;

            // assert
            Assert.AreEqual(rule, result);
            Assert.NotNull(npe.ElementAt(0));
            Assert.IsInstanceOf<ThrowViolation<object>>(npe.ElementAt(0));
        }

        [Test]
        public void _CompileTest()
        {
            // arrange
            var violation = new M.Mock<IViolation>();
            var subject = new Accessor(a => true);
            subject.WithModelViolation(a => violation.Object);

            // act
            var actual = subject.__Compile(new SwapPropVisitor(typeof(object)));

            // assert
            Assert.IsInstanceOf<ConditionalExpression>(actual);
            Assert.AreEqual(ExpressionType.Not, (((ConditionalExpression)actual).Test as UnaryExpression).NodeType);
            Assert.AreEqual(subject.Descriptor.WrappedExpression, (((ConditionalExpression)actual).Test as UnaryExpression).Operand);
            Assert.IsInstanceOf<DefaultExpression>(((ConditionalExpression)actual).IfFalse);

            //TODO: cannot test this one with any accuracy
            //Assert.IsInstanceOf<DefaultExpression>(((ConditionalExpression)actual).IfTrue);
        }

        [Test]
        public void _CompileTest_NoNextElement()
        {
            // arrange
            var violation = new M.Mock<IViolation>();
            var subject = new Accessor(a => true);

            // act
            var result = subject.__Compile(new SwapPropVisitor(typeof(object)));

            // assert
            Assert.IsInstanceOf<DefaultExpression>(result);
        }

        [Test]
        public void ValidationSubjects_Test()
        {
            // arrange
            var violation = new M.Mock<IViolation>();
            Expression<Func<TestClass, bool>> prop = a => a.Success;
            var subject = new RequirementFailed<TestClass>(prop, null);
            var expected = new MemberChainItem(typeof(TestClass))
            {
                NextItem = new MemberChainItem(typeof(TestClass).GetProperty("Success"))
            };

            // act
            var actual = subject.ValidationSubjects;

            // assert
            Assert.AreEqual(1, actual.Count());
            Assert.AreEqual(expected, actual.ElementAt(0));
        }
    }
}
