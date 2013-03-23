using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using BackToFront.Extensions.Reflection;

using NUnit.Framework;
using System.Reflection;
using BackToFront.Utils;

namespace BackToFront.UnitTests.Tests.Extensions
{
    [TestFixture]
    public class Reflection_Tests
    {
        class TestClass1 
        {
            public string Prop { get; set; } 
            public string Field; 
        }

        class TestClass2 : TestClass1 { }

        [Test]
        public void Is_Test()
        {
            // arrange            
            // act
            // assert
            Assert.IsTrue(typeof(TestClass1).Is(typeof(TestClass1)));
            Assert.IsTrue(typeof(TestClass2).Is(typeof(TestClass1)));
            Assert.IsFalse(typeof(string).Is(typeof(TestClass1)));
        }

        [Test]
        public void Get_Property()
        {
            var p = typeof(TestClass1).GetProperty("Prop");
            var subject = new TestClass1 { Prop = "Hello" };

            Assert.AreEqual(subject.Prop, Reflection.Get(p, subject));
        }

        [Test]
        public void Get_Field()
        {
            var f = typeof(TestClass1).GetField("Field");
            var subject = new TestClass1 { Field = "Hello" };

            Assert.AreEqual(subject.Field, Reflection.Get(f, subject));
        }

        [Test]
        [ExpectedException(typeof(EX))]
        public void Get_Method()
        {
            var m = typeof(TestClass1).GetMethod("ToString");
            var subject = new TestClass1 { Field = "Hello" };

            Reflection.Get(m, subject);
        }

        [Test]
        public void Set_Property()
        {
            string s = "Hello";
            var p = typeof(TestClass1).GetProperty("Prop");
            var subject = new TestClass1();

            Reflection.Set(p, subject, s);

            Assert.AreEqual(s, subject.Prop);
        }

        [Test]
        public void Set_Field()
        {
            string s = "Hello";
            var f = typeof(TestClass1).GetField("Field");
            var subject = new TestClass1();

            Reflection.Set(f, subject, s);
            Assert.AreEqual(s, subject.Field);
        }

        [Test]
        [ExpectedException(typeof(EX))]
        public void Set_Method()
        {
            var m = typeof(TestClass1).GetMethod("ToString");
            var subject = new TestClass1();

            Reflection.Set(m, subject, "asdsadsad");
        }
    }
}
