using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using NUnit.Framework;
using T = NUnit.Framework;
using BackToFront.Utils;

namespace BackToFront.UnitTests.Tests.Utils
{
    [TestFixture]
    public class OperatorsTest
    {
        public class TestClass
        {
            public TestClass(object nullableVal)
            {
                NullableVal = nullableVal;
            }

            public TestClass(int val1, int val2)
            {
                Val1 = val1;
                Val2 = val2;
            }

            public int Val1 { get; set; }
            public int Val2 { get; set; }

            public object NullableVal { get; set; }
        }

        [Test]
        public void EqTest_True()
        {
            // arrange
            var subject = new TestClass(1, 1);

            // act
            // assert
            Assert.IsTrue(Operators.Eq(subject, a => a.Val1, a => a.Val2));
        }

        [Test]
        public void EqTest_False()
        {
            // arrange
            var subject = new TestClass(1, 2);

            // act
            // assert
            Assert.IsFalse(Operators.Eq(subject, a => a.Val1, a => a.Val2));
        }

        [Test]
        public static void NEqTest_true()
        {
            // arrange
            var subject = new TestClass(1, 2);

            // act
            // assert
            Assert.IsTrue(Operators.NEq(subject, a => a.Val1, a => a.Val2));
        }

        [Test]
        public static void NEqTest_false()
        {
            // arrange
            var subject = new TestClass(1, 1);

            // act
            // assert
            Assert.IsFalse(Operators.NEq(subject, a => a.Val1, a => a.Val2));
        }

        [Test]
        public void GrTest_True()
        {
            // arrange
            var subject = new TestClass(2, 1);

            // act
            // assert
            Assert.IsTrue(Operators.Gr(subject, a => a.Val1, a => a.Val2));
        }

        [Test]
        public void GrTest_False_Eq()
        {
            // arrange
            var subject = new TestClass(1, 1);

            // act
            // assert
            Assert.IsFalse(Operators.Gr(subject, a => a.Val1, a => a.Val2));
        }

        [Test]
        public void GrTest_False()
        {
            // arrange
            var subject = new TestClass(1, 2);

            // act
            // assert
            Assert.IsFalse(Operators.Gr(subject, a => a.Val1, a => a.Val2));
        }

        [Test]
        public void LeTest_True()
        {
            // arrange
            var subject = new TestClass(1, 2);

            // act
            // assert
            Assert.IsTrue(Operators.Le(subject, a => a.Val1, a => a.Val2));
        }

        [Test]
        public void LeTest_False_Eq()
        {
            // arrange
            var subject = new TestClass(1, 1);

            // act
            // assert
            Assert.IsFalse(Operators.Le(subject, a => a.Val1, a => a.Val2));
        }

        [Test]
        public void LeTest_False()
        {
            // arrange
            var subject = new TestClass(2, 1);

            // act
            // assert
            Assert.IsFalse(Operators.Le(subject, a => a.Val1, a => a.Val2));
        }

        [Test]
        public void GrEqTest_True()
        {
            // arrange
            var subject = new TestClass(2, 1);

            // act
            // assert
            Assert.IsTrue(Operators.GrEq(subject, a => a.Val1, a => a.Val2));
        }

        [Test]
        public void GrEqTest_True_Eq()
        {
            // arrange
            var subject = new TestClass(1, 1);

            // act
            // assert
            Assert.IsTrue(Operators.GrEq(subject, a => a.Val1, a => a.Val2));
        }

        [Test]
        public void GrEqTest_False()
        {
            // arrange
            var subject = new TestClass(1, 2);

            // act
            // assert
            Assert.IsFalse(Operators.GrEq(subject, a => a.Val1, a => a.Val2));
        }

        [Test]
        public void LeEqTest_True()
        {
            // arrange
            var subject = new TestClass(1, 2);

            // act
            // assert
            Assert.IsTrue(Operators.LeEq(subject, a => a.Val1, a => a.Val2));
        }

        [Test]
        public void LeEqTest_True_Eq()
        {
            // arrange
            var subject = new TestClass(1, 1);

            // act
            // assert
            Assert.IsTrue(Operators.LeEq(subject, a => a.Val1, a => a.Val2));
        }

        [Test]
        public void LeEqTest_False()
        {
            // arrange
            var subject = new TestClass(2, 1);

            // act
            // assert
            Assert.IsFalse(Operators.LeEq(subject, a => a.Val1, a => a.Val2));
        }

        [Test]
        public void NullTest_true()
        {
            // arrange
            var subject = new TestClass(null);

            // act
            // assert
            Assert.IsTrue(Operators.Null(subject, a => a.NullableVal, null));
        }

        [Test]
        public void NullTest_false()
        {
            // arrange
            var subject = new TestClass(new object());

            // act
            // assert
            Assert.IsFalse(Operators.Null(subject, a => a.NullableVal, null));
        }
    }
}
