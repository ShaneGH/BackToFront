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
    public class Dependencies_Tests : Base.TestBase
    {
        [Test]
        public void ConstructorTest()
        {
            // arrange
            // act
            // assert
            new Dependencies(new Dictionary<string, object> { { "asiugdsa", new object() } });
        }

        [Test]
        [ExpectedException(typeof(InvalidOperationException))]
        public void ConstructorTest_Fail()
        {
            // arrange
            // act
            // assert
            new Dependencies(new Dictionary<string, object> { { "asiugdsa", null } });
        }

        [Test]
        public void ParameterForDependency_test_creation()
        {
            const string name = "sd09usfd";

            // arrange
            var value = new DateTime();
            var subject = new Dependencies(new Dictionary<string, object> { { name, value } });

            // act
            var param = subject.ParameterForDependency(name);

            // assert
            Assert.IsInstanceOf<UnaryExpression>(param);
            Assert.AreEqual(ExpressionType.Convert, param.NodeType);
            Assert.AreEqual(value.GetType(), param.Type);

            Assert.IsInstanceOf<IndexExpression>((param as UnaryExpression).Operand);
            Assert.AreEqual(ExpressionType.Index, (param as UnaryExpression).Operand.NodeType);

            Assert.AreEqual(subject.Parameter, ((param as UnaryExpression).Operand as IndexExpression).Object);
            Assert.AreEqual(1, ((param as UnaryExpression).Operand as IndexExpression).Arguments.Count());
            Assert.IsInstanceOf<ConstantExpression>(((param as UnaryExpression).Operand as IndexExpression).Arguments.First());
            Assert.AreEqual(name, (((param as UnaryExpression).Operand as IndexExpression).Arguments.First() as ConstantExpression).Value);
        }

        [Test]
        public void ParameterForDependency_test_cache()
        {
            // arrange
            var name1 = "asdsad";
            var name2 = "safd98gyasd";
            var subject = new Dependencies(new Dictionary<string, object> { { name1, 6 }, { name2, new object() } });

            // act
            var param1 = subject.ParameterForDependency(name1);
            var param2 = subject.ParameterForDependency(name2);
            var param3 = subject.ParameterForDependency(name2);

            // assert
            Assert.AreNotSame(param1, param2);
            Assert.AreSame(param2, param3);
        }

        [Test]
        [ExpectedException(typeof(InvalidOperationException))]
        public void ParameterForDependency_test_invalidMock()
        {
            // arrange
            var subject = new Dependencies();

            // act
            // assert
            var param1 = subject.ParameterForDependency("OIHOIJH");
        }
    }
}