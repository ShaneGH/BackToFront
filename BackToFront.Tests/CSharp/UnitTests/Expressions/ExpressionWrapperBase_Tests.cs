using BackToFront.Expressions;
using BackToFront.Extensions.Reflection;
using BackToFront.Utilities;
using Moq.Protected;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using M = Moq;

namespace BackToFront.Tests.UnitTests.Expressions
{
    class ExpressionWrapperBase_Tests : Base.TestBase
    {
        public class TestClass1
        {
            public bool Prop { get; set; }
            public bool Func()
            {
                throw new NotImplementedException();
            }


            public TestClass2 Prop2 { get; set; }
        }

        public class TestClass2
        {
            public int Prop { get; set; }
            public bool Func(int arg)
            {
                throw new NotImplementedException();
            }
        }

        public class AccessorClass : ExpressionWrapperBase
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

            protected override IEnumerable<MemberChainItem> _GetMembersForParameter(ParameterExpression parameter)
            {
                throw new NotImplementedException();
            }

            protected override IEnumerable<ParameterExpression> _UnorderedParameters
            {
                get { return new ParameterExpression[20]; }
            }

            public override Meta.ExpressionElementMeta Meta
            {
                get { throw new NotImplementedException(); }
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
            var subject = new AccessorClass();

            // act
            var result = subject.Compile();

            // assert
            Assert.AreEqual(AccessorClass.WExpression, result);
        }

        [Test]
        public void Compile_Test_BlankArgs()
        {
            // arrange
            var subject = new AccessorClass();

            // act
            var result2 = subject.Compile(Enumerable.Empty<Mock>().ToArray());

            // assert
            Assert.AreEqual(AccessorClass.WExpression, result2);
        }

        [Test]
        public void Compile_Test_Mocked()
        {
            // arrange
            var subject = new AccessorClass() { IsSame = true };
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
            var subject = new AccessorClass() { IsSame = false };
            var mock = new Mock(expression: Expression.Constant(4), value: null, valueType: typeof(object));

            // act
            var result = subject.Compile(new[] { mock });

            // assert
            Assert.AreEqual(AccessorClass.CompileInnerExpressionExpression, result);
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
        public void CreateChildWrapper_Test_Found_Constructor()
        {
            // arrange
            var exp = Expression.Constant(4);

            // act
            var result = ExpressionWrapperBase.CreateChildWrapper(exp);

            // assert
            Assert.NotNull(result);
        }

        [Test]
        public void GetMembersForParameter_Test_NonCache()
        {
            // arrange
            var expected = new MemberChainItem[0];
            var param = Expression.Parameter(typeof(object));
            var subject = new M.Mock<ExpressionWrapperBase> { CallBase = true };
            subject.Protected().Setup<IEnumerable<MemberChainItem>>("_GetMembersForParameter", ItExpr.Is<ParameterExpression>(a => a == param)).Returns(expected);

            // act
            var actual = subject.Object.GetMembersForParameter(param);

            // assert
            Assert.AreEqual(expected, actual);
        }

        [Test]
        public void GetMembersForParameter_Test_WithCache()
        {
            // arrange
            var expected1 = new MemberChainItem[0];
            var expected2 = new MemberChainItem[0];
            var param = Expression.Parameter(typeof(object));
            var subject = new M.Mock<ExpressionWrapperBase> { CallBase = true };
            subject.Protected().Setup<IEnumerable<MemberChainItem>>("_GetMembersForParameter", ItExpr.Is<ParameterExpression>(a => a == param)).Returns(expected1);

            // act
            var actual1 = subject.Object.GetMembersForParameter(param);
            subject.Protected().Setup<IEnumerable<MemberChainItem>>("_GetMembersForParameter", ItExpr.Is<ParameterExpression>(a => a == param)).Returns(expected2);
            var actual2 = subject.Object.GetMembersForParameter(param);

            // assert
            Assert.AreEqual(expected1, actual1);
            Assert.AreEqual(actual1, actual2);
        }

        [Test]
        public void UnorderedParametersCache_Test()
        {
            // arrange
            var subject = new AccessorClass();

            // act
            // assert
            Assert.IsTrue(subject.UnorderedParameters.Equals(subject.UnorderedParameters));
        }

        #region ForChildExpression

        [Test]
        public void ForChild_Test_Success()
        {
            // arrange
            var subject = ExpressionWrapperBase.ToWrapper<TestClass1, int>(a => a.Prop2.Prop);

            // act
            var result = subject.ForChildExpression<TestClass1, TestClass2>(a => a.Prop2, Expression.Parameter(typeof(TestClass2)));

            // assert
            Assert.IsTrue(ExpressionWrapperBase.CreateChildWrapper(result).IsSameExpression(ExpressionWrapperBase.ToWrapper<TestClass2, int>(a => a.Prop)));
        }

        [Test]
        public void ForChild_Test_Success_MultiPath_Param()
        {
            // arrange
            var subject = ExpressionWrapperBase.ToWrapper<TestClass1, bool>(a => a.Prop2.Func(a.Prop2.GetHashCode()));

            // act
            var result = subject.ForChildExpression<TestClass1, TestClass2>(a => a.Prop2, Expression.Parameter(typeof(TestClass2)));

            // assert
            var ex1 = ExpressionWrapperBase.CreateChildWrapper(result);
            var ex2 = ExpressionWrapperBase.ToWrapper<TestClass2, bool>(a => a.Func(a.GetHashCode()));
            Assert.IsTrue(ex1.IsSameExpression(ex2));
        }

        [Test]
        public void ForChild_Test_Success_MultiPath_Const()
        {
            // arrange
            var subject = ExpressionWrapperBase.ToWrapper<TestClass1, bool>(a => a.Prop2.Func(5));

            // act
            var result = subject.ForChildExpression<TestClass1, TestClass2>(a => a.Prop2, Expression.Parameter(typeof(TestClass2)));

            // assert
            var ex1 = ExpressionWrapperBase.CreateChildWrapper(result);
            var ex2 = ExpressionWrapperBase.ToWrapper<TestClass2, bool>(a => a.Func(5));
            Assert.IsTrue(ex1.IsSameExpression(ex2));
        }

        [Test]
        public void ForChild_Test_Success_Param()
        {
            // arrange
            var subject = ExpressionWrapperBase.ToWrapper<TestClass1, TestClass2>(a => a.Prop2);

            // act
            var result = subject.ForChildExpression<TestClass1, TestClass2>(a => a.Prop2, Expression.Parameter(typeof(TestClass2)));

            // assert
            Assert.IsTrue(ExpressionWrapperBase.CreateChildWrapper(result).IsSameExpression(ExpressionWrapperBase.ToWrapper<TestClass2, TestClass2>(a => a)));
        }

        [Test]
        [ExpectedException(typeof(InvalidOperationException))]
        public void ForChild_Test_Fail_Invalid_MockExpression()
        {
            // arrange
            var subject = ExpressionWrapperBase.ToWrapper<TestClass1, int>(a => (true ? 1 : 7));

            // act
            // assert
            var result = subject.ForChildExpression<TestClass2, int>(null, Expression.Parameter(typeof(int)));
        }

        [Test]
        [ExpectedException(typeof(InvalidOperationException))]
        public void ForChild_Test_Fail_Wrong_Parameter_Count()
        {
            // arrange
            Expression<Func<TestClass1, int, bool>> input = (a, b) => b == 5 ? a.Prop : true;
            var subject = ExpressionWrapperBase.ToWrapper(input);

            // act
            // assert
            var result = subject.ForChildExpression<TestClass2, int>(null, Expression.Parameter(typeof(int)));
        }

        [Test]
        [ExpectedException(typeof(InvalidOperationException))]
        public void ForChild_Test_Fail_Wrong_Parameter_Type()
        {
            // arrange
            Expression<Func<TestClass1, int, bool>> input = (a, b) => a.Prop;
            var subject = ExpressionWrapperBase.ToWrapper<TestClass1, bool>(a => a.Prop);

            // act
            // assert
            var result = subject.ForChildExpression<bool, bool>(a => a, Expression.Parameter(typeof(bool)));
        }

        [Test]
        [ExpectedException(typeof(InvalidOperationException))]
        public void ForChild_Test_Fail_NotLinearInput()
        {
            // arrange
            Expression<Func<TestClass1, int, bool>> input = (a, b) => a.Prop;
            var subject = ExpressionWrapperBase.ToWrapper<TestClass1, bool>(a => a.Prop);

            // act
            // assert
            var result = subject.ForChildExpression<bool, bool>(a => (true ? true : false), Expression.Parameter(typeof(bool)));
        }

        #endregion

        [Test]
        [Ignore]
        public void TestLinqConstructors()
        { }
    }
}
