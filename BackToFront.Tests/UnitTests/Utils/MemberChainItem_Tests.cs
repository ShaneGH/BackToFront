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
        public class TestClass
        {
            public int Prop { get; set; }
        }

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
        public void SetNext_Test_Invalid()
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
        public void SetNext_Test_Valid()
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

        [Test]
        public void Equals_Test_referenceEquals_true()
        {
            // arrange
            var subject = new MemberChainItem(typeof(string));

            // act
            // assert
            Assert.IsTrue(subject.Equals(subject));
        }

        [Test]
        public void Equals_Test_valueEquals_true()
        {
            // arrange
            var subject1 = new MemberChainItem(typeof(string));
            subject1.SetNext(typeof(string).GetMethod("GetHashCode"));

            var subject2 = new MemberChainItem(typeof(string));
            subject2.SetNext(typeof(string).GetMethod("GetHashCode"));

            // act
            // assert
            Assert.IsTrue(subject1 == subject2);
        }

        [Test]
        public void Equals_Test_valueEquals_false_Member()
        {
            // arrange
            var subject1 = new MemberChainItem(typeof(string));
            subject1.SetNext(typeof(string).GetMethod("GetHashCode"));

            var subject2 = new MemberChainItem(typeof(object));
            subject2.SetNext(typeof(object).GetMethod("GetHashCode"));

            // act
            // assert
            Assert.IsFalse(subject1.Equals(subject2));
        }

        [Test]
        public void Equals_Test_valueEquals_false_next()
        {
            // arrange
            var subject1 = new MemberChainItem(typeof(object));
            subject1.SetNext(typeof(object).GetMethod("ToString"));

            var subject2 = new MemberChainItem(typeof(object));
            subject2.SetNext(typeof(object).GetMethod("GetHashCode"));

            // act
            // assert
            Assert.IsFalse(subject1.Equals(subject2));
        }

        [Test]
        public void UltimateMemberTest()
        {
            // arrange
            var subject = new MemberChainItem(typeof(TestClass));
            subject.NextItem = new MemberChainItem(typeof(TestClass).GetProperty("Prop"));
            MemberChainItem current = subject.NextItem;
            for (var i = 0; i < 10; i++)
            {
                current.NextItem = new MemberChainItem(typeof(int).GetMethod("GetHashCode"));
                current = current.NextItem;
            }
            
            // act
            // assert
            Assert.AreEqual(typeof(int).GetMethod("GetHashCode"), subject.UltimateMember);
        }
    }
}