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

namespace BackToFront.Tests.UnitTests.Framework.Base
{
    [TestFixture]
    public class ExpressionElementTests :BackToFront.Tests.Base.TestBase
    {
        [Test]
        public void CompileTest()
        {
            // arrange
            var hello = "hello";
            Expression<Func<object, string>> desc = a => hello;
            var subject = new M.Mock<ExpressionElement<object, string>>(desc, null) { CallBase = true };

            // act
            var actual = subject.Object.Compile();

            // assert
            Assert.AreEqual(hello, actual.Invoke(new object(), null, null));
        }

        [Test]
        public void AffectedMembers_Test()
        {
            // arrange
            Expression<Func<object, string>> desc = a => a.ToString();
            var subject = new M.Mock<ExpressionElement<object, string>>(desc, null) { CallBase = true };
            var expected = ((MethodCallExpressionWrapper)GetPrivateProperty(subject.Object, "Descriptor")).GetMembersForParameter(desc.Parameters.First());

            // act
            var actual = subject.Object.AffectedMembers;

            // assert
            Assert.AreEqual(1, actual.Count());
            Assert.AreEqual(expected, actual.Select(a => a.Member));
            foreach (var r in actual.Select(a => a.Requirement))
                Assert.AreEqual(subject.Object.PropertyRequirement, r);
        }
    }
}