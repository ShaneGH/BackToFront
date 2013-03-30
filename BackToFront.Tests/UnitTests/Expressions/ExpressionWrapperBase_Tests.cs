using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

using BackToFront.Extensions.Reflection;
using BackToFront.Tests.Utilities;
using BackToFront.Expressions;
using NUnit.Framework;
using System.Collections.ObjectModel;
using BackToFront.Utils;

namespace BackToFront.Tests.UnitTests.Expressions
{
    class ExpressionWrapperBase_Tests : Base.TestBase
    {
        public class TestClass : ExpressionWrapperBase
        {
            public static readonly Expression CompileInnerExpressionExpression = Expression.Constant(9);
            public static readonly Expression WExpression = Expression.Constant(9);

            public bool IsSame { get; set; }

            protected override Expression CompileInnerExpression(Mocks mocks)
            {
                return CompileInnerExpressionExpression;
            }

            public override bool IsSameExpression(ExpressionWrapperBase expression)
            {
                return IsSame;
            }

            public override Expression WrappedExpression
            {
                get { return WExpression; }
            }
        }

        #region constructors

        [Test]
        public void AllExpressionWrappersAreImplemented()
        {
            var ignore = new Type[] 
            { 
                typeof(Expression),
                typeof(Expression<>),
                typeof(BlockExpression),
                typeof(ConditionalExpression),                
                typeof(DebugInfoExpression),                
                typeof(DefaultExpression),                
                typeof(DynamicExpression),                
                typeof(GotoExpression),                
                typeof(IndexExpression),     
                // important: invocation of lambda
                typeof(InvocationExpression),  
                typeof(LambdaExpression),              
                typeof(LabelExpression),                
                typeof(ListInitExpression),                
                typeof(LoopExpression),                
                typeof(MemberInitExpression),                              
                typeof(NewArrayExpression),                
                typeof(NewExpression),                
                typeof(RuntimeVariablesExpression),                
                typeof(SwitchExpression),                
                typeof(TryExpression),                
                typeof(TypeBinaryExpression)              
            };

            foreach (var expType in typeof(Expression).Assembly.GetTypes().Where(t => t.Is<Expression>() && t.IsPublic))
            {
                if (ignore.Contains(expType))
                    Assert.IsFalse(ExpressionWrapperBase.Constructors.ContainsKey(expType), string.Format("{0} is implemented, remove from ignore list", expType));
                else
                    Assert.IsTrue(ExpressionWrapperBase.Constructors.ContainsKey(expType), string.Format("{0} is not implemented", expType));
            }
        }

        #endregion

        [Test]
        public void Compile_Test_NoArgs()
        {
            // arrange
            var subject = new TestClass();

            // act
            var result = subject.Compile();

            // assert
            Assert.AreEqual(TestClass.WExpression, result);
        }

        [Test]
        public void Compile_Test_BlankArgs()
        {
            // arrange
            var subject = new TestClass();

            // act
            var result1 = subject.Compile(null);
            var result2 = subject.Compile(Enumerable.Empty<Mock>());

            // assert
            Assert.AreEqual(TestClass.WExpression, result1);
            Assert.AreEqual(TestClass.WExpression, result2);
        }

        [Test]
        public void Compile_Test_Mocked()
        {
            // arrange
            var subject = new TestClass() { IsSame = true };
            var mock = new Mock(expression: Expression.Constant(4), value: null, valueType: typeof(object));
            var mocks = new Mocks(new[] { mock });

            // act
            var result = subject.Compile(mocks);

            // assert
            Assert.IsInstanceOf<UnaryExpression>(result);
            Assert.AreEqual(ExpressionType.Convert, result.NodeType);
            Assert.AreEqual(typeof(object), result.Type);

            Assert.IsInstanceOf<BinaryExpression>((result as UnaryExpression).Operand);
            Assert.AreEqual(ExpressionType.ArrayIndex, (result as UnaryExpression).Operand.NodeType);

            // this is all we really want to test here, the rest is superflous to need
            Assert.AreEqual(mocks.Parameter, ((result as UnaryExpression).Operand as BinaryExpression).Left);
        }

        [Test]
        public void Compile_Test_With_Mocks_Not_Mocked()
        {
            // arrange
            var subject = new TestClass() { IsSame = false };
            var mock = new Mock(expression: Expression.Constant(4), value: null, valueType: typeof(object));

            // act
            var result = subject.Compile(new[] { mock });

            // assert
            Assert.AreEqual(TestClass.CompileInnerExpressionExpression, result);
        }

        [Test]
        [ExpectedException(typeof(InvalidOperationException))]
        public void CreateChildWrapper_Test_InvalidExpressionType()
        {
            // arrange
            var exp = Expression.Break(Expression.Label());

            // act
            // assert
            ExpressionWrapperBase.CreateChildWrapper(exp);
        }

        [Test]
        public void CreateChildWrapper_Test_TypeTraversal_CANNOT_TEST_THIS()
        {
            Assert.Pass();
        }

        [Test]
        public void CreateChildWrapper_Test_Fopund_Constructor()
        {
            // arrange
            var exp = Expression.Constant(4);

            // act
            var result = ExpressionWrapperBase.CreateChildWrapper(exp);

            // assert
            Assert.NotNull(result);
        }

        [Test]
        [Ignore]
        public void TestLinqConstructors()
        { }
    }
}
