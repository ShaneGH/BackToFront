using BackToFront.Expressions;
using BackToFront.Utils;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace BackToFront.Tests.UnitTests.Expressions
{
    [TestFixture]
    public class ElementAtExpressionWrapper_Tests : Base.TestBase
    {
        public class TestSubjectWrapper : ElementAtExpressionWrapper
        {
            public TestSubjectWrapper(MethodCallExpression expression)
                : base(expression)
            {
            }

            public IEnumerable<MemberChainItem> __GetMembersForParameter(ParameterExpression p)
            {
                return base._GetMembersForParameter(p);
            }
        }

        public class TestClass
        {
            public void Hello(TestClass input) { }
        }

        [Test]
        public void _GetMembersForParameter_Test()
        {
            // arange
            Expression<Func<IEnumerable<int>, int>> exp = a => a.ElementAt(1);

            // act
            var actual = subject.__GetMembersForParameter(param);
            var expected = new ParameterExpressionWrapper(param).GetMembersForParameter(param).ElementAt(0);

            // assert
            Assert.AreEqual(2, actual.Count());
            Assert.AreEqual(actual.ElementAt(0).Member, expected.Member);
            Assert.NotNull(actual.ElementAt(0).NextItem);
            Assert.AreEqual(actual.ElementAt(0).NextItem.Member, member);
            Assert.IsNull(actual.ElementAt(0).NextItem.NextItem);

            var tmp = actual.ElementAt(1);
            Assert.IsTrue(tmp.Equals(expected));
        }
    }
}
