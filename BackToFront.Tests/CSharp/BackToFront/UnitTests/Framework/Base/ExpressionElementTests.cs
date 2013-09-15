using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Threading.Tasks;

using BackToFront.Extensions;

using BackToFront.Utilities;
using BackToFront.Logic;
using BackToFront.Framework.Base;
using NUnit.Framework;

using M = Moq;

using BackToFront.Tests.Utilities;
using BackToFront.Expressions;
using BackToFront.Framework;

namespace BackToFront.Tests.CSharp.UnitTests.Framework.Base
{
    [TestFixture]
    public class ExpressionElementTests : BackToFront.Tests.Base.TestBase
    {
        class TestClass : ExpressionElement<object, object>
        {
            public TestClass(Expression<Func<object, object>> descriptor, Rule<object> rule)
                : base(descriptor, rule)
            { }

            public override IEnumerable<PathElement<object>> AllPossiblePaths
            {
                get { throw new NotImplementedException(); }
            }

            protected override Expression _Compile(BackToFront.Expressions.Visitors.ExpressionMocker visitor)
            {
                throw new NotImplementedException();
            }
        }

        [Test]
        [ExpectedException(typeof(ArgumentNullException))]
        public void ConstructorTest_NoDescriptor()
        {
            // arrange
            // act
            // assert
            new TestClass(null, null);
        }

        [Test]
        public void ConstructorTest_OK()
        {
            // arrange
            Expression<Func<object, object>> expected = a => a;

            // act
            var actual = new TestClass(expected, null);


            // assert
            Assert.AreEqual(expected.Parameters[0], actual.EntityParameter);
        }
    }
}