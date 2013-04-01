using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

using M = Moq;

using NUnit.Framework;
using BackToFront.Utils;
using BackToFront.Framework;
using BackToFront.Framework.Base;

namespace BackToFront.Tests.UnitTests.Utils
{
    [TestFixture]
    public class MemberChainItem_Tests : Base.TestBase
    {
        [Test]
        [ExpectedException(typeof(ArgumentNullException))]
        public void Constructor_Test_Null_Member()
        {
            // arrange
            // act
            // assert
            new MemberChainItem(null);
        }

        [Test]
        [ExpectedException(typeof(InvalidOperationException))]
        public void Constructor_Test_SetNext()
        {
            // arrange
            var intThing = typeof(int).GetMethod("CompareTo", new[] { typeof(int) });
            Assert.NotNull(intThing);

            // act
            var subject = new MemberChainItem(typeof(object).GetMethod("ToString"));

            // assert
            subject.SetNext(intThing);
        }

        [Test]
        public void Constructor_Test_SetNext_Valid()
        {
            // arrange
            var objectThing = typeof(object).GetMethod("ToString");
            var stringThing = typeof(string).GetProperty("Length");
            Assert.NotNull(objectThing);
            Assert.NotNull(stringThing);

            // act
            var subject = new MemberChainItem(objectThing);
            subject.SetNext(stringThing);

            // assert
            Assert.AreEqual(objectThing, subject.Member);
            Assert.AreEqual(stringThing, subject.NextItem.Member);
        }
    }
}