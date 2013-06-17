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
    }
}