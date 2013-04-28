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
            public Accessor(Dependencies dependences)
                : base(new Mocks(), dependences) { }

            public Expression _VisitMember(MemberExpression node)
            {
                return VisitMember(node);
            }
        }

        [Test]
        public void AsValueArray_Test()
        {
            const int val1 = 3;
            const int val2 = 4;

            // arrange
            Expression<Func<TestClass, int>> exp = a => a.Prop;
            var subject = new SwapPropVisitor(new Mocks(new[] { new Mock(exp.Body, val1, typeof(bool)), new Mock(exp.Body, val2, typeof(bool)) }), null);

            // act
            var values = subject.MockValues;

            // assert
            Assert.AreEqual(2, values.Count());
            Assert.True(values.Contains(val1));
            Assert.True(values.Contains(val2));
        }

        [Test]
        public void ContainsNothing_Tests()
        {
            // arrange
            var t1 = new SwapPropVisitor(new Mocks(), new Dependencies());
            var t2 = new SwapPropVisitor(new Mocks(new[] { new Mock(wrapperExpression: null, value: null, valueType: null, behavior: Enum.MockBehavior.MockAndSet) }), new Dependencies());
            var t3 = new SwapPropVisitor(new Mocks(), new Dependencies(new Dictionary<string, object> { { "PIUOHBOIUHG", new object() } }));

            // act
            // assert
            Assert.IsTrue(t1.ContainsNothing);
            Assert.IsFalse(t2.ContainsNothing);
            Assert.IsFalse(t3.ContainsNothing);
        }

        [Test]
        public void Parameter_Tests()
        {
            // arrange
            var t1 = new SwapPropVisitor(new Mocks(), new Dependencies());

            // act
            var p = t1.Parameters;

            // assert
            Assert.AreEqual(2, p.Count());
            Assert.AreEqual(t1.Mocks.Parameter, p.ElementAt(0));
            Assert.AreEqual(t1.Dependences.Parameter, p.ElementAt(1));
        }

        [Test]
        public void Visit_Tests_IsMocked()
        {
            // arrange
            var mockedExp = Expression.Property(Expression.Parameter(typeof(TestClass)), "Prop");
            var mocks = new Mocks(new[] { new Mock(mockedExp, 4, typeof(int)) });
            var t1 = new SwapPropVisitor(mocks, new Dependencies());

            // act
            var result = t1.Visit(mockedExp);

            // assert
            Assert.AreNotEqual(mockedExp, result);
            Assert.AreEqual(mocks.ParameterForMock(mocks.ElementAt(0)), result);
        }

        [Test]
        public void Visit_Tests_IsNotMocked()
        {
            // arrange
            var expected = Expression.Property(Expression.Parameter(typeof(TestClass)), "Prop");
            var t1 = new SwapPropVisitor(new Mocks(), new Dependencies());

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
            var dependencies = new Dependencies(new Dictionary<string, object> { { aName, de } });
            var t1 = new Accessor(dependencies);

            // act
            var actual = t1._VisitMember((MemberExpression)expected.Body);

            // assert
            Assert.AreNotEqual(expected.Body, actual);
            Assert.AreEqual(dependencies.ParameterForDependency(aName), actual);
        }

        [Test]
        public void VisitMember_Tests_NoDependency()
        {
            // arrange
            var expected = Expression.Property(Expression.Parameter(typeof(TestClass)), "Prop");
            var t1 = new SwapPropVisitor(new Mocks(), new Dependencies());

            // act
            var actual = t1.Visit(expected);

            // assert
            Assert.AreEqual(expected, actual);
        }
    }
}
