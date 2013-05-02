using BackToFront.Dependency;
using BackToFront.Expressions.Visitors;
using BackToFront.Utilities;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace BackToFront.Tests.CSharp.UnitTests.Expressions.Visitors
{
    [TestFixture]
    public class SwapPropVisitor_Tests
    {
        public class TestClass
        {
            public int Prop { get; set; }
        }

        public class Accessor : SwapPropVisitor
        {
            public Accessor(IDictionary<string, object> dependences, Type type)
                : base(null, dependences, type) { }

            public Accessor(Type type)
                : this(null, type) { }

            public Expression _VisitMember(MemberExpression node)
            {
                return VisitMember(node);
            }

            public Expression _VisitParameter(ParameterExpression node)
            {
                return VisitParameter(node);
            }
        }

        [Test]
        public void MockValues_Test()
        {
            const int val1 = 3;
            const int val2 = 4;

            // arrange
            Expression<Func<TestClass, int>> exp = a => a.Prop;
            var subject = new SwapPropVisitor(new Mocks(new[] { new Mock(exp.Body, val1, typeof(bool)), new Mock(exp.Body, val2, typeof(bool)) }, Expression.Constant(new object[0])), null, typeof(TestClass));

            // act
            var values = subject.MockValues;

            // assert
            Assert.AreEqual(2, values.Count());
            Assert.True(values.Contains(val1));
            Assert.True(values.Contains(val2));
        }

        [Test]
        public void DependencyValues_Test()
        {
            const string val1 = "asdsad";
            const string val2 = "qrd09uesa";

            // arrange
            Expression<Func<TestClass, int>> exp = a => a.Prop;
            var subject = new SwapPropVisitor(null, new Dictionary<string, object> { {val1, new object()}, {val2, new object()} }, typeof(TestClass));

            // act
            var values = subject.Dependences;

            // assert
            Assert.AreEqual(2, values.Count());
            Assert.True(values.ContainsKey(val1));
            Assert.True(values.ContainsKey(val2));
        }

        [Test]
        public void ContainsNothing_Tests()
        {
            // arrange
            var t1 = new SwapPropVisitor(typeof(TestClass));
            var t2 = new SwapPropVisitor(new Mocks(new[] { new Mock(wrapperExpression: null, value: null, valueType: null, behavior: Enum.MockBehavior.MockAndSet) }, Expression.Constant(new object[0])), new Dependencies().ToDictionary(), typeof(TestClass));
            var t3 = new SwapPropVisitor(new Mocks(), new Dictionary<string, object> { { "PIUOHBOIUHG", new object() } }, typeof(TestClass));

            // act
            // assert
            Assert.IsTrue(t1.ContainsNothing);
            Assert.IsFalse(t2.ContainsNothing);
            Assert.IsFalse(t3.ContainsNothing);
        }

        public void VisitParameter_Test_NoParam()
        {
            // arrange
            var param = Expression.Parameter(typeof(Type));

            // act
            // assert
            Assert.AreEqual(new Accessor(typeof(object))._VisitParameter(param), param);
        }

        public void VisitParameter_And_WithEntityParameter_Test_WithParam()
        {
            // arrange
            var subject = new Accessor(typeof(object));
            var param = Expression.Parameter(typeof(Type));
            Expression result = null;

            // act
            using (subject.WithEntityParameter(param))
            {
                result = subject._VisitParameter(param);
            }

            // assert
            Assert.AreEqual(result, subject.EntityParameter);
        }

        [Test]
        public void Visit_Tests_IsMocked()
        {
            // arrange
            var mockedExp = Expression.Property(Expression.Parameter(typeof(TestClass)), "Prop");
            var t1 = new SwapPropVisitor(new[] { new Mock(mockedExp, 4, typeof(int)) }, new Dictionary<string, object>(), typeof(TestClass));

            // act
            var result = t1.Visit(mockedExp);

            // assert
            Assert.AreNotEqual(mockedExp, result);
            Assert.AreEqual(t1.Mocks.ParameterForMock(t1.Mocks.ElementAt(0)), result);
        }

        [Test]
        public void Visit_Tests_IsNotMocked()
        {
            // arrange
            var expected = Expression.Property(Expression.Parameter(typeof(TestClass)), "Prop");
            var t1 = new SwapPropVisitor(typeof(TestClass));

            // act
            var actual = t1.Visit(expected);

            // assert
            Assert.AreEqual(expected, actual);
        }

        [Test]
        public void VisitMember_Tests_HasDependency()
        {
            // arrange
            var de = new TestClass();
            var aName = "KHIKHLKIH";
            Expression<Func<TestClass>> expected = () => new DependencyWrapper<TestClass>(aName, null).Val;
            var t1 = new Accessor(new Dictionary<string, object> { { aName, de } }, typeof(TestClass));

            // act
            var actual = t1._VisitMember((MemberExpression)expected.Body);

            // assert
            Assert.AreNotEqual(expected.Body, actual);
            Assert.AreEqual(t1.Dependences.ParameterForDependency(aName), actual);
        }

        [Test]
        public void VisitMember_Tests_NoDependency()
        {
            // arrange
            var expected = Expression.Property(Expression.Parameter(typeof(TestClass)), "Prop");
            var t1 = new SwapPropVisitor(typeof(TestClass));

            // act
            var actual = t1.Visit(expected);

            // assert
            Assert.AreEqual(expected, actual);
        }
    }
}
