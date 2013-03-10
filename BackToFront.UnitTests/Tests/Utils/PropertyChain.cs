using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

using NUnit.Framework;

using BackToFront.Attributes;
using BackToFront.Utils;

namespace BackToFront.UnitTests.Tests.Utils
{
    [TestFixture]
    public class PropertyChain_Test
    {
        class TestClass
        {
            [AffectsMembersAttribute]
            public string Property { get; set; }
            public string Field;

            public InnerTestClass Inner { get; set; }
        }

        class InnerTestClass
        {
            public string Field;
        }

        [Test]
        public void Constructor_Test_Constructor1()
        {
            // arrange            
            // act
            var test = new PropertyChain<TestClass>(new MemberInfo[] { typeof(TestClass).GetField("Field"), typeof(string).GetProperty("Length") });

            // assert
            Assert.AreEqual(typeof(TestClass).GetField("Field"), test.Members.ElementAt(0));
            Assert.AreEqual(typeof(string).GetProperty("Length"), test.Members.ElementAt(1));
        }

        [Test]
        public void Constructor_Test_Constructor2()
        {
            // arrange
            // act
            var test = new PropertyChain<TestClass>(a => a.Field.Length);

            // assert
            Assert.AreEqual(2, test.Members.Count());
            Assert.AreEqual(typeof(TestClass).GetField("Field"), test.Members.ElementAt(0));
            Assert.AreEqual(typeof(string).GetProperty("Length"), test.Members.ElementAt(1));
        }

        [Test]
        [ExpectedException(typeof(EX))]
        public void Constructor_Test_ContainsMethod()
        {
            // arrange            
            // act
            var test = new PropertyChain<TestClass>(a => a.Field.Count());
        }

        [Test]
        [ExpectedException(typeof(EX))]
        public void Constructor_Test_ContainsAffectsMembersAttribute()
        {
            // arrange            
            // act
            var test = new PropertyChain<TestClass>(a => a.Property);
        }

        [Test]
        [ExpectedException(typeof(EX))]
        public void Constructor_Test_InvalidOrdering()
        {
            // arrange            
            // act
            var test = new PropertyChain<TestClass>(new MemberInfo[] { typeof(string).GetProperty("Length"), typeof(TestClass).GetField("Field") });
        }

        [Test]
        public void Equality_Test()
        {
            // arrange
            // act
            // assert
            Assert.IsTrue(new PropertyChain<TestClass>(a => a.Field.Length) == new PropertyChain<TestClass>(a => a.Field.Length));
            Assert.IsTrue(new PropertyChain<TestClass>(a => a.Field.Length).Equals(new PropertyChain<TestClass>(a => a.Field.Length)));
        }

        #region Get/set

        [Test]
        public void Set_Test_OK()
        {
            // arrange
            var value = "Hello";
            var tc = new TestClass { Inner = new InnerTestClass() };
            var subject = new PropertyChain<TestClass>(a => a.Inner.Field);

            // act
            subject.Set(tc, value);

            // assert
            Assert.AreEqual(value, tc.Inner.Field);
        }

        [Test]
        [ExpectedException(typeof(EX))]
        public void Set_Test_NullProperty()
        {
            // arrange
            var value = "Hello";
            var tc = new TestClass();
            var subject = new PropertyChain<TestClass>(a => a.Inner.Field);

            // act
            // assert
            subject.Set(tc, value);
        }

        [Test]
        public void Get_Test_OK()
        {
            // arrange
            var tc = new TestClass { Inner = new InnerTestClass { Field = "Hello" } };
            var subject = new PropertyChain<TestClass>(a => a.Inner.Field);

            // act
            var value = (string)subject.Get(tc);

            // assert
            Assert.AreEqual(value, tc.Inner.Field);
        }

        [Test]
        [ExpectedException(typeof(EX))]
        public void Get_Test_NullProperty()
        {
            // arrange
            var tc = new TestClass();
            var subject = new PropertyChain<TestClass>(a => a.Inner.Field);

            // act
            // assert
            var value = (string)subject.Get(tc);
        }

        [Test]
        public void TrySet_Test_OK()
        {
            // arrange
            var value = "Hello";
            var tc = new TestClass { Inner = new InnerTestClass() };
            var subject = new PropertyChain<TestClass>(a => a.Inner.Field);

            // act
            var result = subject.TrySet(tc, value);

            // assert
            Assert.IsTrue(result);
            Assert.AreEqual(value, tc.Inner.Field);
        }

        [Test]
        public void TrySet_Test_NullProperty()
        {
            // arrange
            var value = "Hello";
            var tc = new TestClass();
            var subject = new PropertyChain<TestClass>(a => a.Inner.Field);

            // act
            var result = subject.TrySet(tc, value);

            // assert
            Assert.IsFalse(result);
        }

        [Test]
        public void TryGet_Test_OK()
        {
            // arrange
            var tc = new TestClass { Inner = new InnerTestClass { Field = "Hello" } };
            var subject = new PropertyChain<TestClass>(a => a.Inner.Field);
            object value;

            // act
            var result = subject.TryGet(tc, out value);

            // assert
            Assert.IsTrue(result);
            Assert.AreEqual((string)value, tc.Inner.Field);
        }

        [Test]
        public void TryGet_Test_NullProperty()
        {
            // arrange
            var tc = new TestClass();
            var subject = new PropertyChain<TestClass>(a => a.Inner.Field);
            object value;

            // act
            var result = subject.TryGet(tc, out value);

            // assert
            Assert.IsFalse(result);
        }

        #endregion
    }
}
