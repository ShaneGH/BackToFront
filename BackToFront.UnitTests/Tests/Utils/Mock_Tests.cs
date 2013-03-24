using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

using NUnit.Framework;
using BackToFront.Utils;

namespace BackToFront.UnitTests.Tests.Utils
{
    [TestFixture]
    public class Mock_Tests : Base.TestBase
    {
        public class TestClass
        {
            public bool Prop { get; set; }
            public bool Func()
            {
                throw new NotImplementedException();
            }
        }

        [Test]
        public void CanSetTest_true()
        {
            // arrange
            Expression<Func<TestClass, bool>> exp = a => a.Prop;
            var subject = new Mock(exp.Body, null, typeof(TestClass));

            // act
            // assert
            Assert.IsTrue(subject.CanSet);
        }

        [Test]
        public void CanSetTest_false()
        {
            // arrange
            Expression<Func<TestClass, bool>> exp = a => a.Func();
            var subject = new Mock(exp.Body, null, typeof(TestClass));

            // act
            // assert
            Assert.IsFalse(subject.CanSet);
        }

        [Test]
        public void Set_success()
        {
            // arrange
            Expression<Func<TestClass, bool>> exp = a => a.Prop;
            var subject = new Mock(exp.Body, true, typeof(TestClass));
            var test = new TestClass { Prop = false };

            // act
            subject.SetValue(test);

            // assert
            Assert.IsTrue(test.Prop);
        }

        [Test]
        [ExpectedException(typeof(InvalidOperationException))]
        public void Set_fail()
        {
            // arrange
            Expression<Func<TestClass, bool>> exp = a => a.Func();
            var subject = new Mock(exp.Body, true, typeof(TestClass));
            var test = new TestClass { Prop = false };

            // act
            // assert
            subject.SetValue(test);
        }
    }
}
