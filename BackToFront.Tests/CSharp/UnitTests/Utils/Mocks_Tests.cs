using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

using NUnit.Framework;
using BackToFront.Utilities;

namespace BackToFront.Tests.CSharp.UnitTests.Utils
{
    [TestFixture]
    public class Mocks_Tests : Base.TestBase
    {
        public class TestClass
        {
            public int Prop { get; set; }
        }

        [Test]
        public void ParameterForMock_test_creation()
        {
            const int testElementAt = 1;

            // arrange
            Type mockType = typeof(object);
            Expression<Func<TestClass, int>> exp = a => a.Prop;
            var subject = new Mocks(new[] { new Mock(exp.Body, null, mockType), new Mock(exp.Body, null, mockType) });

            // act
            var param = subject.ParameterForMock(subject.ElementAt(testElementAt));

            // assert
            Assert.IsInstanceOf<UnaryExpression>(param);
            Assert.AreEqual(ExpressionType.Convert, param.NodeType);
            Assert.AreEqual(mockType, param.Type);

            Assert.IsInstanceOf<BinaryExpression>((param as UnaryExpression).Operand);
            Assert.AreEqual(ExpressionType.ArrayIndex, (param as UnaryExpression).Operand.NodeType);

            Assert.AreEqual(subject.Parameter, ((param as UnaryExpression).Operand as BinaryExpression).Left);
            Assert.IsInstanceOf<ConstantExpression>(((param as UnaryExpression).Operand as BinaryExpression).Right);
            Assert.AreEqual(testElementAt, (((param as UnaryExpression).Operand as BinaryExpression).Right as ConstantExpression).Value);
        }

        [Test]
        public void ParameterForMock_test_cache()
        {
            // arrange
            Expression<Func<TestClass, int>> exp = a => a.Prop;
            var subject = new Mocks(new[] { new Mock(exp.Body, null, typeof(bool)), new Mock(exp.Body, null, typeof(bool)) });

            // act
            var param1 = subject.ParameterForMock(subject.ElementAt(0));
            var param2 = subject.ParameterForMock(subject.ElementAt(1));
            var param3 = subject.ParameterForMock(subject.ElementAt(1));

            // assert
            Assert.AreNotSame(param1, param2);
            Assert.AreSame(param2, param3);
        }

        [Test]
        [ExpectedException(typeof(InvalidOperationException))]
        public void ParameterForMock_test_invalidMock()
        {
            // arrange
            Expression<Func<TestClass, int>> exp = a => a.Prop;
            var subject = new Mocks(new[] { new Mock(exp.Body, null, typeof(bool)), new Mock(exp.Body, null, typeof(bool)) });

            // act
            // assert
            var param1 = subject.ParameterForMock(new Mock(exp.Body, null, typeof(bool)));
        }
    }
}