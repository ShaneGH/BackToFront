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

            public int[] Array { get; set; }

            public int[] Array2 { get; set; }
        }

        #region Non indexed

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
            subject.NextItem = new MemberChainItem(intThing);
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
            subject.NextItem = new MemberChainItem(stringThing);

            // assert
            Assert.AreEqual(objectThing, subject.Member);
            Assert.AreEqual(stringThing, subject.NextItem.Member);
        }

        [Test]
        public void Equals_Test_referenceEquals_true()
        {
            // arrange
            var subject = new MemberChainItem(typeof(TestClass).GetProperty("Array"));

            // act
            // assert
            Assert.IsTrue(subject.Equals(subject));
        }

        [Test]
        public void Equals_Test_valueEquals_true()
        {
            // arrange
            var subject1 = new MemberChainItem(typeof(TestClass).GetProperty("Array"), new MemberIndex(4));
            subject1.NextItem = new MemberChainItem(typeof(int).GetMethod("GetHashCode"));

            var subject2 = new MemberChainItem(typeof(TestClass).GetProperty("Array"), new MemberIndex(4));
            subject2.NextItem = new MemberChainItem(typeof(int).GetMethod("GetHashCode"));

            // act
            // assert
            Assert.IsTrue(subject1 == subject2);
        }

        [Test]
        public void Equals_Test_valueEquals_false_Member()
        {
            // arrange
            var subject1 = new MemberChainItem(typeof(TestClass).GetProperty("Array"), new MemberIndex(4));
            subject1.NextItem = new MemberChainItem(typeof(int).GetMethod("GetHashCode"));

            var subject2 = new MemberChainItem(typeof(TestClass).GetProperty("Array2"), new MemberIndex(4));
            subject2.NextItem = new MemberChainItem(typeof(int).GetMethod("GetHashCode"));

            // act
            // assert
            Assert.IsFalse(subject1.Equals(subject2));
        }

        [Test]
        public void Equals_Test_valueEquals_false_next()
        {
            // arrange
            var subject1 = new MemberChainItem(typeof(TestClass).GetProperty("Array"), new MemberIndex(4));
            subject1.NextItem = new MemberChainItem(typeof(int).GetMethod("ToString", new Type[0]));

            var subject2 = new MemberChainItem(typeof(TestClass).GetProperty("Array"), new MemberIndex(4));
            subject2.NextItem = new MemberChainItem(typeof(int).GetMethod("GetHashCode"));

            // act
            // assert
            Assert.IsFalse(subject1.Equals(subject2));
        }

        [Test]
        public void Equals_Test_valueEquals_false_index()
        {
            // arrange
            var subject1 = new MemberChainItem(typeof(TestClass).GetProperty("Array"), new MemberIndex(4));
            subject1.NextItem = new MemberChainItem(typeof(int).GetMethod("GetHashCode"));

            var subject2 = new MemberChainItem(typeof(TestClass).GetProperty("Array"), new MemberIndex(3));
            subject2.NextItem = new MemberChainItem(typeof(int).GetMethod("GetHashCode"));

            // act
            // assert
            Assert.IsFalse(subject1 == subject2);
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

        #endregion

        #region indexed

        [Test]
        [ExpectedException(typeof(InvalidOperationException))]
        public void Constructor_Test_IndexedType()
        {
            // arrange
            // act
            // assert
            var subject = new MemberChainItem(typeof(TestClass), new MemberIndex(2));
        }

        [Test]
        [ExpectedException(typeof(InvalidOperationException))]
        public void Constructor_Test_IndexedNonEnumerable()
        {
            // arrange
            // act
            // assert
            new MemberChainItem(typeof(TestClass).GetProperty("Prop"), new MemberIndex(2));
        }

        [Test]
        public void Constructor_Test_IndexedEnumerable()
        {
            // arrange
            var property = typeof(TestClass).GetProperty("Array");
            var index = 4;

            // act
            var subject = new MemberChainItem(property, new MemberIndex(index));

            // assert
            Assert.AreEqual(property, subject.Member);
            Assert.AreEqual(index, subject.Index.Index);
            Assert.AreEqual(typeof(int), subject.IndexedType);
        }

        [Test]
        [ExpectedException(typeof(InvalidOperationException))]
        public void NextItem_Test_Indexed_InvalidProperty()
        {
            // arrange
            var property = typeof(TestClass).GetProperty("Array");
            var index = 4;
            var baseMember = new MemberChainItem(property, new MemberIndex(index));

            // act
            // assert
            baseMember.NextItem = new MemberChainItem(typeof(int[]).GetProperty("Length"));
        }

        [Test]
        public void NextItem_Test_Indexed_ValidProperty()
        {
            // arrange
            var property = typeof(TestClass).GetProperty("Array");
            var index = 4;
            var baseMember = new MemberChainItem(property, new MemberIndex(index));
            var toInsert = new MemberChainItem(typeof(int).GetMethod("GetHashCode"));

            // act
            baseMember.NextItem = toInsert;

            // assert
            Assert.AreEqual(toInsert, baseMember.NextItem);
        }

        #endregion
    }
}