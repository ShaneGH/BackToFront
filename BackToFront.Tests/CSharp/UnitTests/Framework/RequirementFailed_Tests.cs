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
        public class Accessor : RequirementFailed<TestClass>
        {
            public Accessor(Expression<Func<TestClass, bool>> property)
                : base(property, null)
            {
            }

            public Action<TestClass, ValidationContextX> __NewCompile(SwapPropVisitor visitor)
            {
                return _NewCompile(visitor);
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
        [TestCase(true)]
        [TestCase(false)]
        public void _NewCompileTest(bool success)
        {
            // arrange
            var violation = new M.Mock<IViolation>();
            var subject = new Accessor(a => a.Success);
            subject.WithModelViolation(a => violation.Object);
            var item = new TestClass { Success = success };
            var ctxt = new ValidationContextX(true, null, null);

            // act
            subject.__NewCompile(new SwapPropVisitor())(item, ctxt);

            // assert
            if (success)
            {
                Assert.AreEqual(0, ctxt.Violations.Count());
            }
            else
            {
                Assert.AreEqual(1, ctxt.Violations.Count());
                Assert.AreEqual(violation.Object, ctxt.Violations.First());
            }
        }

        [Test]
        public void _NewCompileTest_NoNextElement()
        {
            // arrange
            var violation = new M.Mock<IViolation>();
            var subject = new Accessor(a => a.Success);

            // act
            var result = subject.__NewCompile(new SwapPropVisitor());

            // assert
            Assert.AreEqual(PathElement<TestClass>.DoNothing, result);
        }
    }
}
