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

namespace BackToFront.Tests.CSharp.UnitTests.Framework.Base
{
    [TestFixture]
    public class ExpressionElementTests :BackToFront.Tests.Base.TestBase
    {
        //[Test]
        //public void CompileTest()
        //{
        //    // arrange
        //    var hello = "hello";
        //    Expression<Func<object, string>> desc = a => hello;
        //    var subject = new M.Mock<ExpressionElement<object, string>>(desc, null) { CallBase = true };

        //    // act
        //    var actual = subject.Object.Compile();

        //    // assert
        //    Assert.AreEqual(hello, actual.Invoke(new object(), null, null));
        //}

        [Test]
        public void ValidatableMembers_Test()
        {
            // arrange
            Expression<Func<object, string>> desc = a => a.ToString();
            var subject = new M.Mock<ExpressionElement<object, string>>(desc, null) { CallBase = true };
            var expected = subject.Object.Descriptor.GetMembersForParameter(desc.Parameters.First());

            // act
            subject.Setup(a => a.PropertyRequirement).Returns(false);
            var actual1 = subject.Object.ValidationSubjects;

            // assert
            Assert.AreEqual(0, actual1.Count());


            // act
            subject.Setup(a => a.PropertyRequirement).Returns(true);
            var actual2 = subject.Object.ValidationSubjects;

            // assert
            Assert.AreEqual(1, actual2.Count());
            Assert.AreEqual(expected, actual2);
        }

        [Test]
        public void RequiredForValidationMembers_Test()
        {
            // arrange
            Expression<Func<object, string>> desc = a => a.ToString();
            var subject = new M.Mock<ExpressionElement<object, string>>(desc, null) { CallBase = true };
            var expected = subject.Object.Descriptor.GetMembersForParameter(desc.Parameters.First());

            // act
            subject.Setup(a => a.PropertyRequirement).Returns(true);
            var actual1 = subject.Object.RequiredForValidation;

            // assert
            Assert.AreEqual(0, actual1.Count());


            // act
            subject.Setup(a => a.PropertyRequirement).Returns(false);
            var actual2 = subject.Object.RequiredForValidation;

            // assert
            Assert.AreEqual(1, actual2.Count());
            Assert.AreEqual(expected, actual2);
        }
    }
}