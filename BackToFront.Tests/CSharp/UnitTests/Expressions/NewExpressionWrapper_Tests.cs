using BackToFront.Expressions;
using BackToFront.Expressions.Visitors;
using BackToFront.Utilities;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using M = Moq;

namespace BackToFront.Tests.CSharp.UnitTests.Expressions
{
    [TestFixture]
    public class NewExpressionWrapper_Tests : Base.TestBase
    {
        public class Accessor : NewExpressionWrapper
        {
            public Accessor(NewExpression expression)
                : base(expression)
            {
            }

            public IEnumerable<MemberChainItem> __GetMembersForParameter(ParameterExpression p)
            {
                return base._GetMembersForParameter(p);
            }

            public IEnumerable<ParameterExpression> __UnorderedParameters
            {
                get
                {
                    return base._UnorderedParameters;
                }
            }
        }

        public class TestClass
        {
            public static readonly ConstructorInfo Constructor1 = typeof(TestClass).GetConstructor(new Type[0]);
            public static readonly ConstructorInfo Constructor2 = typeof(TestClass).GetConstructor(new[] { typeof(string), typeof(int), typeof(object) });
            public TestClass(string i1, int i2, object i3) { }
            public TestClass() { }

            public string Parameter { get; set; }
        }

        [Test]
        public void IsSameExpression_Test()
        {
            // arrange
            var obj = new object();
            var ex1 = Expression.New(TestClass.Constructor2, Expression.Constant("JHVJHV"), Expression.Constant(32423), Expression.Constant(obj));
            var ex2 = Expression.New(TestClass.Constructor2, Expression.Constant("JHVJHV"), Expression.Constant(32423), Expression.Constant(obj));
            var ex3 = Expression.New(TestClass.Constructor2, Expression.Constant("JHVJHV"), Expression.Constant(32423), Expression.Constant(new object()));
            var ex4 = Expression.New(TestClass.Constructor1);

            var subject = new NewExpressionWrapper(ex1);

            // act
            // assert
            Assert.IsTrue(subject.IsSameExpression(ex1));
            Assert.IsTrue(subject.IsSameExpression(ex2));
            Assert.IsFalse(subject.IsSameExpression(ex3));
            Assert.IsFalse(subject.IsSameExpression(ex4));
        }

        [Test]
        public void IsSameExpression_Test_AnonymousConstructor()
        {
            // arrange
            var obj = new object();
            Expression<Func<object>> ex1 = () => new { aaa = "AAA", bbb = 234, ccc = obj };
            Expression<Func<object>> ex2 = () => new { aaa = "AAA", bbb = 234, ccc = obj };
            Expression<Func<object>> ex3 = () => new { aaa = "AAA", bbb = 234, ccc = new object() };
            Expression<Func<object>> ex4 = () => new { aaa = "AAA" };

            var subject = new NewExpressionWrapper((NewExpression)ex1.Body);

            // act
            // assert
            Assert.IsTrue(subject.IsSameExpression(ex1.Body));
            Assert.IsTrue(subject.IsSameExpression(ex2.Body));
            Assert.IsFalse(subject.IsSameExpression(ex3.Body));
            Assert.IsFalse(subject.IsSameExpression(ex4.Body));
        }

        [Test]
        public void _GetMembersForParameter_Test()
        {
            // arange
            Expression<Func<TestClass, TestClass>> exp = a => new TestClass(a.Parameter, 2, null);
            var subject = new Accessor((NewExpression)exp.Body);

            // act
            var actual = subject.__GetMembersForParameter(exp.Parameters.First());
            var expected = new MemberChainItem(typeof(TestClass))
            {
                NextItem = new MemberChainItem(typeof(TestClass).GetProperty("Parameter"))
            };

            // assert
            Assert.AreEqual(1, actual.Count());
            Assert.AreEqual(expected, actual.First());
        }

        [Test]
        public void UnorderedParameters_Test()
        {
            // arange
            Expression<Func<TestClass, TestClass>> exp = a => new TestClass(a.Parameter, 2, null);
            var subject = new Accessor((NewExpression)exp.Body);

            // act
            // assert
            Assert.AreEqual(subject.UnorderedParameters, exp.Parameters);
        }
    }
}