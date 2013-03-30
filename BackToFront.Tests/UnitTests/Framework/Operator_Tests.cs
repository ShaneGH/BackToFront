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
    public class Operator_Tests : BackToFront.Tests.Base.TestBase
    {
        [Test]
        public void NextPathElementsTest_RequirementFailed()
        {
            // arrange
            var subject = new Operator<object>(a => true, null);

            // act
            var result = subject.RequirementFailed;
            var npe = subject.NextPathElements(null, null);

            // assert
            Assert.AreEqual(3, npe.Count());
            Assert.AreEqual(result, npe.ElementAt(0));
            Assert.IsNull(npe.ElementAt(1));
            Assert.IsNull(npe.ElementAt(2));
        }

        [Test]
        public void NextPathElementsTest_Then()
        {
            // arrange
            var subject = new Operator<object>(a => true, null);

            // act
            var result = subject.Then((a) => { });
            var npe = subject.NextPathElements(null, null);

            // assert
            Assert.AreEqual(3, npe.Count());
            Assert.IsNull(npe.ElementAt(0));
            Assert.NotNull(npe.ElementAt(1));
            Assert.IsNull(npe.ElementAt(2));
        }

        [Test]
        public void ConditionIsTrue_Test1()
        {
            // arrange
            var subject = new Operator<object>(a => true, null);

            // act
            // assert
            Assert.IsTrue(subject.ConditionIsTrue(null, new Mocks()));
        }

        [Test]
        public void ConditionIsTrue_Test2()
        {
            // arrange
            var subject = new Operator<object>(a => false, null);

            // act
            // assert
            Assert.IsFalse(subject.ConditionIsTrue(null, new Mocks()));
        }

        [Test]
        public void RequirementFailed_Test()
        {
            // arrange
            var subject = new Operator<object>(a => true, null);

            // act
            var result = subject.RequirementFailed;
            var npe = subject.NextPathElements(null, null);

            // assert
            Assert.AreEqual(3, npe.Count());
            Assert.AreEqual(result, npe.ElementAt(0));
            Assert.IsNull(npe.ElementAt(1));
            Assert.IsNull(npe.ElementAt(2));
        }

    }
}