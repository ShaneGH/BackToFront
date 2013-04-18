//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using System.Threading.Tasks;

//using BackToFront.Attributes;

//using NUnit.Framework;

//namespace BackToFront.UnitTests.Tests.Attributes
//{
//    [TestFixture]
//    public class AffectsMembersAttribute_Tests : UnitTests.Base.TestBase
//    {
//        public class TestClass1
//        {
//            public TestClass2 Member { get; set; }
//            public string Hi;
//        }

//        public class TestClass2
//        {
//            public TestClass3 Member;
//        }

//        public class TestClass3
//        {
//            public string Hi;
//        }

//        [Test]
//        public void ConstructorTest_Pass()
//        {
//            // arrange            
//            // act
//            // assert
//            var subject = new AffectsMembersAttribute("var.var.var.va", "var", "var.terer.asd.wf.asere");
//        }

//        [Test]
//        public void ConstructorTest_Fail()
//        {
//            // arrange
//            // act
//            // assert
//            Assert.Throws(typeof(InvalidOperationException), () => new AffectsMembersAttribute("as.cas.ase.rased()"));
//            Assert.Throws(typeof(InvalidOperationException), () => new AffectsMembersAttribute("as.cas.ase."));
//            Assert.Throws(typeof(InvalidOperationException), () => new AffectsMembersAttribute("as.cas.4ase.rased"));
//            Assert.Throws(typeof(InvalidOperationException), () => new AffectsMembersAttribute("as.cas.a$se.rased"));
//            Assert.Throws(typeof(InvalidOperationException), () => new AffectsMembersAttribute("as.cas.as[e.rased"));
//        }

//        [Test]
//        public void GetMembersTest_Pass()
//        {
//            // arrange
//            var subject = new AffectsMembersAttribute("Member.Member.Hi", "Hi");

//            // act
//            var members = subject.GetMembers(typeof(TestClass1));

//            // assert
//            Assert.AreEqual(4, members.Count());
//            Assert.IsTrue(members.Contains(typeof(TestClass1).GetMember("Hi").Single()));
//            Assert.IsTrue(members.Contains(typeof(TestClass1).GetMember("Member").Single()));
//            Assert.IsTrue(members.Contains(typeof(TestClass2).GetMember("Member").Single()));
//            Assert.IsTrue(members.Contains(typeof(TestClass3).GetMember("Hi").Single()));
//        }

//        [Test]
//        public void GetMembersTest_Cache()
//        {
//            // arrange
//            var subject = new AffectsMembersAttribute("Member.Member.Hi", "Hi");

//            // act
//            var members1 = subject.GetMembers(typeof(TestClass1));
//            var members2 = subject.GetMembers(typeof(TestClass1));

//            // assert
//            Assert.AreSame(members1, members2);
//        }

//        [Test]
//        [ExpectedException(typeof(InvalidOperationException))]
//        public void GetMembersTest_InvalidPropertyName()
//        {
//            // arrange
//            var subject = new AffectsMembersAttribute("Member.MemberX.Hi", "Hi");

//            // act
//            var members = subject.GetMembers(typeof(TestClass1));
//        }
//    }
//}
