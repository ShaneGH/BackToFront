using BackToFront.Expressions;
using BackToFront.Expressions.Visitors;
using BackToFront.Framework;
using NUnit.Framework;
using System.Linq;
using System.Linq.Expressions;

namespace BackToFront.Tests.CSharp.UnitTests.Framework
{
    [TestFixture]
    public class RequireOperator_Tests : BackToFront.Tests.Base.TestBase
    {
        class Accessor : RequireOperator<object>
        {
            public Accessor()
                : base(null) { }

            public Expression __NewCompile(SwapPropVisitor visitor)
            {
                return _Compile(visitor);
            }
        }

        [Test]
        public void RequireThat_Test()
        {
            // arrange
            var subject = new RequireOperator<object>(null);

            // act
            var result = subject.RequireThat(a => true);
            var npe = subject.AllPossiblePaths;

            // assert
            Assert.AreEqual(result, npe.ElementAt(1));
        }

        [Test]
        public void Then_Test()
        {
            // arrange
            bool passed = false;
            var rule = new Rule<object>();
            var subject = new RequireOperator<object>(rule);

            // act
            var result = subject.Then((a) =>
            {
                Assert.NotNull(a);
                passed = true;
            });
            var npe = subject.AllPossiblePaths;

            // assert
            Assert.AreEqual(rule, result);
            Assert.True(passed);
        }

        [Test]
        public void _NewCompile_Test_No_Body()
        {
            // arrange
            var subject = new Accessor();

            // act
            var actual = subject.__NewCompile(new SwapPropVisitor(typeof(object)));

            // assert
            Assert.IsTrue(new DefaultExpressionWrapper().IsSameExpression(actual));
        }

        [Test]
        public void _NewCompile_Test_With_Body()
        {
            // arrange
            var subject = new Accessor();
            subject.RequireThat(a => true);
            var v = new SwapPropVisitor(typeof(object));

            // act
            var actual = ExpressionWrapperBase.CreateChildWrapper(subject.__NewCompile(v));
            var expected = ((RequirementFailed<object>)GetPrivateProperty(subject, typeof(RequireOperator<object>), "_RequireThat")).Compile(v);

            // assert
            Assert.IsTrue(actual.IsSameExpression(expected));
        }
    }
}