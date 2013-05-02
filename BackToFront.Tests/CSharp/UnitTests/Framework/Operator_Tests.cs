using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using NUnit.Framework;
using BackToFront.Framework;
using BackToFront.Framework.Base;
using BackToFront.Extensions.IEnumerable;
using BackToFront.Utilities;
using BackToFront.Expressions.Visitors;

namespace BackToFront.Tests.CSharp.UnitTests.Framework
{
    [TestFixture]
    public class Operator_Tests : BackToFront.Tests.Base.TestBase
    {
        [Test]
        public void RequirementFailed_Test()
        {
            // arrange
            var subject = new Operator<object>(a => true, null);

            // act
            var result = subject.RequirementFailed;
            var npe = subject.AllPossiblePaths;

            // assert
            Assert.AreEqual(3, npe.Count());
            Assert.AreEqual(result, npe.ElementAt(0));
            Assert.IsNull(npe.ElementAt(1));
            Assert.IsNull(npe.ElementAt(2));
        }
    }
}