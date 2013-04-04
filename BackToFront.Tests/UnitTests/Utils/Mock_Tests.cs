using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

using NUnit.Framework;
using BackToFront.Utils;
using BackToFront.Expressions;

namespace BackToFront.Tests.UnitTests.Utils
{
    [TestFixture]
    public class Mock_Tests : Base.TestBase
    {
        public class TestClass1
        {
            public bool Prop { get; set; }
            public bool Func()
            {
                throw new NotImplementedException();
            }


            public TestClass2 Prop2 { get; set; }
        }

        public class TestClass2
        {
            public int Prop { get; set; }
            public bool Func(int arg)
            {
                throw new NotImplementedException();
            }
        }

        [Test]
        public void CanSetTest_true()
        {
            // arrange
            Expression<Func<TestClass1, bool>> exp = a => a.Prop;
            var subject = new Mock(exp.Body, null, typeof(TestClass1));

            // act
            // assert
            Assert.IsTrue(subject.CanSet);
        }

        [Test]
        public void CanSetTest_false()
        {
            // arrange
            Expression<Func<TestClass1, bool>> exp = a => a.Func();
            var subject = new Mock(exp.Body, null, typeof(TestClass1));

            // act
            // assert
            Assert.IsFalse(subject.CanSet);
        }

        [Test]
        public void Set_success()
        {
            // arrange
            Expression<Func<TestClass1, bool>> exp = a => a.Prop;
            var subject = new Mock(exp.Body, true, typeof(TestClass1));
            var test = new TestClass1 { Prop = false };

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
            Expression<Func<TestClass1, bool>> exp = a => a.Func();
            var subject = new Mock(exp.Body, true, typeof(TestClass1));
            var test = new TestClass1 { Prop = false };

            // act
            // assert
            subject.SetValue(test);
        }

        [Test]
        [Ignore]
        public void ForChild_Test_Success()
        {
            // arrange
            var subject = Mock.Create<TestClass1, int>(a => a.Prop2.Prop, 1);

            // act
            var result = subject.ForChild<TestClass1, TestClass2>(a => a.Prop2);

            // assert
            Assert.IsTrue(result.Expression.IsSameExpression(ExpressionWrapperBase.ToWrapper<TestClass2, int>(a => a.Prop)));
        }

        [Test]
        [Ignore]
        public void ForChild_Test_Success_MultiPath_Param()
        {
            // arrange
            var subject = Mock.Create<TestClass1, bool>(a => a.Prop2.Func(a.Prop2.GetHashCode()), true);

            // act
            var result = subject.ForChild<TestClass1, TestClass2>(a => a.Prop2);

            // assert
            Assert.IsTrue(result.Expression.IsSameExpression(ExpressionWrapperBase.ToWrapper<TestClass2, bool>(a => a.Func(a.GetHashCode()))));
        }

        [Test]
        [Ignore]
        public void ForChild_Test_Success_MultiPath_Const()
        {
            // arrange
            var subject = Mock.Create<TestClass1, bool>(a => a.Prop2.Func(5), true);

            // act
            var result = subject.ForChild<TestClass1, TestClass2>(a => a.Prop2);

            // assert
            Assert.IsTrue(result.Expression.IsSameExpression(ExpressionWrapperBase.ToWrapper<TestClass2, bool>(a => a.Func(5))));
        }

        [Test]
        [Ignore]
        [ExpectedException(typeof(InvalidOperationException))]
        public void ForChild_Test_Fail_InvalidChildBaseType()
        {
            // arrange
            var subject = Mock.Create<TestClass1, int>(a => a.Prop2.Prop, 1);

            // act
            // assert
            var result = subject.ForChild<TestClass2, int>(a => a.Prop);
        }

        [Test]
        [Ignore]
        [ExpectedException(typeof(InvalidOperationException))]
        public void ForChild_Test_Fail_ReferencesOtherPath()
        {
            // arrange
            var subject = Mock.Create<TestClass1, bool>(a => a.Prop, true);

            // act
            // assert
            var result = subject.ForChild<TestClass1, TestClass2>(a => a.Prop2);
        }

        [Test]
        [Ignore]
        [ExpectedException(typeof(InvalidOperationException))]
        public void ForChild_Test_Fail_PartiallyReferencesOtherPath()
        {
            // arrange
            var subject = Mock.Create<TestClass1, bool>(a => a.Prop2.Func(a.Prop.GetHashCode()), true);

            // act
            // assert
            var result = subject.ForChild<TestClass1, TestClass2>(a => a.Prop2);
        }

        [Test]
        [Ignore]
        public void TryForChild_Test_Success()
        {
            // arrange
            var subject = Mock.Create<TestClass1, int>(a => a.Prop2.Prop, 1);

            // act
            Mock result;
            var success = subject.TryForChild<TestClass1, TestClass2>(a => a.Prop2, out result);

            // assert
            Assert.IsTrue(success);
            Assert.IsTrue(result.Expression.IsSameExpression(ExpressionWrapperBase.ToWrapper<TestClass2, int>(a => a.Prop)));
        }

        [Test]
        [Ignore]
        public void TryForChild_Test_Failure()
        {
            // arrange
            var subject = Mock.Create<TestClass1, int>(a => a.Prop2.Prop, 1);

            // act
            Mock dummy;
            var success = subject.TryForChild<TestClass1, bool>(a => a.Prop, out dummy);

            // assert
            Assert.IsFalse(success);
        }
    }
}
