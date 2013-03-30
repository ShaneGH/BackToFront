using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using NUnit.Framework;
using BackToFront.Framework;
using BackToFront.Framework.Base;
using BackToFront.Extensions.IEnumerable;
using BackToFront.Utils;

namespace BackToFront.Tests.UnitTests.Framework
{
    [TestFixture]
    public class RequireOperator_Tests : BackToFront.Tests.Base.TestBase
    {
        [Test]
        public void NextPathElementsTest_Then()
        {
            // arrange
            var subject = new RequireOperator<object>(a => true, null);

            // act
            var result = subject.Then((a) => { });
            var npe = subject.NextPathElements(null, null);

            // assert
            Assert.AreEqual(2, npe.Count());
            Assert.NotNull(npe.ElementAt(0));
            Assert.IsNull(npe.ElementAt(1));
        }

        [Test]
        public void NextPathElementsTest_RequireThat()
        {
            // arrange
            var subject = new RequireOperator<object>(a => true, null);

            // act
            var result = subject.RequireThat(a => true);
            var npe = subject.NextPathElements(null, null);

            // assert
            Assert.AreEqual(2, npe.Count());
            Assert.IsNull(npe.ElementAt(0));
            Assert.AreEqual(result, npe.ElementAt(1));
        }

        [Test]
        public void RequireThat_Test()
        {
            // arrange
            var subject = new RequireOperator<object>(a => true, null);

            // act
            var result = subject.RequireThat(a => true);
            var npe = subject.NextPathElements(null, null);

            // assert
            Assert.AreEqual(result, npe.ElementAt(1));
        }

        [Test]
        public void Then_Test()
        {
            // arrange
            bool passed = false;
            var rule = new Rule<object>();
            var subject = new RequireOperator<object>(a => true, rule);

            // act
            var result = subject.Then((a) => 
            {
                Assert.NotNull(a);
                passed = true;
            });
            var npe = subject.NextPathElements(null, null);

            // assert
            Assert.AreEqual(rule, result);
            Assert.True(passed);
        }
    }
}