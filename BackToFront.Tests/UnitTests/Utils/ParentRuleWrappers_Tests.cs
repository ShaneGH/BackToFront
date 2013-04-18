using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

using M = Moq;

using NUnit.Framework;
using BackToFront.Utilities;
using BackToFront.Framework;

namespace BackToFront.Tests.UnitTests.Utils
{
    [TestFixture]
    public class ParentRuleWrappers_Tests : Base.TestBase
    {
        public static bool first = true;
        public class TestClass { }

        public override void Setup()
        {
            base.Setup();

            if (first)
                Rules<TestClass>.Repository.Add((rule) => { });

            // added to static dictionary
            first = false;
        }

        [Test]
        [ExpectedException(typeof(InvalidOperationException))]
        public void Constructor_InvalidType()
        {
            // arrange
            // act
            // assert
            new ParentRuleWrappers<TestClass>(typeof(string));
        }

        [Test]
        public void Dependencies_Test()
        {
            // arrange
            // act
            var subject = new ParentRuleWrappers<TestClass>(typeof(TestClass));

            // assert
            // rule added in setup
            Assert.AreEqual(1, subject.Count());
        }
    }
}