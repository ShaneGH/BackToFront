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

namespace BackToFront.Tests.CSharp.UnitTests.Framework
{
    [TestFixture]
    public class RequireOperator_Tests : BackToFront.Tests.Base.TestBase
    {
        [Test]
        public void RequireThat_Test()
        {
            // arrange
            var subject = new RequireOperator<object>(null);

            // act
            var result = subject.RequireThat(a => true);
            var npe = subject.AllPossiblePaths;

            // assert
            Assert.AreEqual(result, npe.ElementAt(1));
        }

        [Test]
        public void Then_Test()
        {
            // arrange
            bool passed = false;
            var rule = new Rule<object>();
            var subject = new RequireOperator<object>(rule);

            // act
            var result = subject.Then((a) => 
            {
                Assert.NotNull(a);
                passed = true;
            });
            var npe = subject.AllPossiblePaths;

            // assert
            Assert.AreEqual(rule, result);
            Assert.True(passed);
        }
    }
}