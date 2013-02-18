using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using BackToFront.Framework;
using BackToFront.Framework.Base;

using BackToFront.UnitTests.Base;
using BackToFront.UnitTests.Utilities;

using NUnit.Framework;

using BackToFront.Logic;
namespace BackToFront.UnitTests.Tests.Framework
{
    [TestFixture]
    public class OperatorsTest : TestBase
    {
        private static readonly Dictionary<bool, Dictionary<bool, Dictionary<bool, Func<bool, bool, bool, bool, bool>>>> Functions = new Dictionary<bool, Dictionary<bool, Dictionary<bool, Func<bool, bool, bool, bool, bool>>>>();
        static OperatorsTest()
        {
            Functions[false] = new Dictionary<bool, Dictionary<bool, Func<bool, bool, bool, bool, bool>>>();
            Functions[true] = new Dictionary<bool, Dictionary<bool, Func<bool, bool, bool, bool, bool>>>();
            Functions[false][false] = new Dictionary<bool, Func<bool, bool, bool, bool, bool>>();
            Functions[true][false] = new Dictionary<bool, Func<bool, bool, bool, bool, bool>>();
            Functions[false][true] = new Dictionary<bool, Func<bool, bool, bool, bool, bool>>();
            Functions[true][true] = new Dictionary<bool, Func<bool, bool, bool, bool, bool>>();

            Functions[false][false][false] = (a, b, c, d) => a || b || c || d;
            Functions[false][false][true] = (a, b, c, d) => a || b || c && d;
            Functions[false][true][false] = (a, b, c, d) => a || b && c || d;
            Functions[false][true][true] = (a, b, c, d) => a || b && c && d;
            Functions[true][false][false] = (a, b, c, d) => a && b || c || d;
            Functions[true][false][true] = (a, b, c, d) => a && b || c && d;
            Functions[true][true][false] = (a, b, c, d) => a && b && c || d;
            Functions[true][true][true] = (a, b, c, d) => a && b && c && d;
        }

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

        public static IEnumerable<Tuple<bool, bool, bool, bool, bool, bool, bool>> TestOperation()
        {
            bool v1 = false,
                v2 = false,
                v3 = false,
                v4 = false,
                v5 = false,
                v6 = false,
                v7 = false;
            
            do
            {
                do
                {
                    do
                    {
                        do
                        {
                            do
                            {
                                do
                                {
                                    do
                                    {
                                        yield return new Tuple<bool, bool, bool, bool, bool, bool, bool>(v1, v2, v3, v4, v5, v6, v7);
                                        v4 = !v4;
                                    } while (v4);
                                    v7 = !v7;
                                } while (v7);
                                v3 = !v3;
                            } while (v3);
                            v6 = !v6;
                        } while (v6);
                        v2 = !v2;
                    } while (v2);
                    v5 = !v5;
                } while (v5);
                v1 = !v1;
            } while (v1);
        }
        
        [Test]
        [TestCaseSource("TestOperation")]
        public void GiantOperatorTest(Tuple<bool, bool, bool, bool, bool, bool, bool> input)
        {
            // And = true, Or = false

            bool evaluation1 = input.Item1,
                evaluation2 = input.Item2,
                evaluation3 = input.Item3,
                evaluation4 = input.Item4,
                operator1 = input.Item5,
                operator2 = input.Item6,
                operator3 = input.Item7;
            
            var violation = new SimpleViolation("Something");
            Func<bool, string> op = a => a ? " AND " : " OR ";
            var log = evaluation1 + op(operator1) + evaluation2 + op(operator2) + evaluation3 + op(operator3) + evaluation4;

            var c1 = new If<object, SimpleViolation>(a => evaluation1, null);
            var c2 = operator1 ? c1.IsTrue().And(a => evaluation2) : c1.IsTrue().Or(a => evaluation2);
            var c3 = operator2 ? c2.IsTrue().And(a => evaluation3) : c2.IsTrue().Or(a => evaluation3);
            var c4 = operator3 ? c3.IsTrue().And(a => evaluation4) : c3.IsTrue().Or(a => evaluation4);
            c4.IsTrue().ModelViolationIs(violation);

            var result = c1.Validate(new object());
            if (result != null)
                Assert.AreEqual(violation, result);

            //Assert.Fail(log + " returns " + Functions[operator1][operator2][operator3](evaluation1, evaluation2, evaluation3, evaluation4));
            Assert.AreEqual(Functions[operator1][operator2][operator3](evaluation1, evaluation2, evaluation3, evaluation4), result != null, log);
        }

        [Test]
        public void EqTest_True()
        {
            // arrange
            var subject = new TestClass(1, 1);

            // act
            // assert
            Assert.IsTrue(If<TestClass, SimpleViolation>.Eq(subject, a => a.Val1, a => a.Val2));
        }

        [Test]
        public void EqTest_False()
        {
            // arrange
            var subject = new TestClass(1, 2);

            // act
            // assert
            Assert.IsFalse(If<TestClass, SimpleViolation>.Eq(subject, a => a.Val1, a => a.Val2));
        }

        [Test]
        public static void NEqTest_true()
        {
            // arrange
            var subject = new TestClass(1, 2);

            // act
            // assert
            Assert.IsTrue(If<TestClass, SimpleViolation>.NEq(subject, a => a.Val1, a => a.Val2));
        }

        [Test]
        public static void NEqTest_false()
        {
            // arrange
            var subject = new TestClass(1, 1);

            // act
            // assert
            Assert.IsFalse(If<TestClass, SimpleViolation>.NEq(subject, a => a.Val1, a => a.Val2));
        }

        [Test]
        public void GrTest_True()
        {
            // arrange
            var subject = new TestClass(2, 1);

            // act
            // assert
            Assert.IsTrue(If<TestClass, SimpleViolation>.Gr(subject, a => a.Val1, a => a.Val2));
        }

        [Test]
        public void XXXX()
        {
            IComparable x = 8;
            Assert.IsTrue(x.CompareTo(8) == 0);
            Assert.IsTrue(x.CompareTo(7) == 1);
            Assert.IsTrue(x.CompareTo(9) == -1);
        }

        [Test]
        public void GrTest_False_Eq()
        {
            // arrange
            var subject = new TestClass(1, 1);

            // act
            // assert
            Assert.IsFalse(If<TestClass, SimpleViolation>.Gr(subject, a => a.Val1, a => a.Val2));
        }

        [Test]
        public void GrTest_False()
        {
            // arrange
            var subject = new TestClass(1, 2);

            // act
            // assert
            Assert.IsFalse(If<TestClass, SimpleViolation>.Gr(subject, a => a.Val1, a => a.Val2));
        }

        [Test]
        public void LeTest_True()
        {
            // arrange
            var subject = new TestClass(1, 2);

            // act
            // assert
            Assert.IsTrue(If<TestClass, SimpleViolation>.Le(subject, a => a.Val1, a => a.Val2));
        }

        [Test]
        public void LeTest_False_Eq()
        {
            // arrange
            var subject = new TestClass(1, 1);

            // act
            // assert
            Assert.IsFalse(If<TestClass, SimpleViolation>.Le(subject, a => a.Val1, a => a.Val2));
        }

        [Test]
        public void LeTest_False()
        {
            // arrange
            var subject = new TestClass(2, 1);

            // act
            // assert
            Assert.IsFalse(If<TestClass, SimpleViolation>.Le(subject, a => a.Val1, a => a.Val2));
        }

        [Test]
        public void GrEqTest_True()
        {
            // arrange
            var subject = new TestClass(2, 1);

            // act
            // assert
            Assert.IsTrue(If<TestClass, SimpleViolation>.GrEq(subject, a => a.Val1, a => a.Val2));
        }

        [Test]
        public void GrEqTest_True_Eq()
        {
            // arrange
            var subject = new TestClass(1, 1);

            // act
            // assert
            Assert.IsTrue(If<TestClass, SimpleViolation>.GrEq(subject, a => a.Val1, a => a.Val2));
        }

        [Test]
        public void GrEqTest_False()
        {
            // arrange
            var subject = new TestClass(1, 2);

            // act
            // assert
            Assert.IsFalse(If<TestClass, SimpleViolation>.GrEq(subject, a => a.Val1, a => a.Val2));
        }

        [Test]
        public void LeEqTest_True()
        {
            // arrange
            var subject = new TestClass(1, 2);

            // act
            // assert
            Assert.IsTrue(If<TestClass, SimpleViolation>.LeEq(subject, a => a.Val1, a => a.Val2));
        }

        [Test]
        public void LeEqTest_True_Eq()
        {
            // arrange
            var subject = new TestClass(1, 1);

            // act
            // assert
            Assert.IsTrue(If<TestClass, SimpleViolation>.LeEq(subject, a => a.Val1, a => a.Val2));
        }

        [Test]
        public void LeEqTest_False()
        {
            // arrange
            var subject = new TestClass(2, 1);

            // act
            // assert
            Assert.IsFalse(If<TestClass, SimpleViolation>.LeEq(subject, a => a.Val1, a => a.Val2));
        }

        [Test]
        public void NullTest_true()
        {
            // arrange
            var subject = new TestClass(null);

            // act
            // assert
            Assert.IsTrue(If<TestClass, SimpleViolation>.Null(subject, a => a.NullableVal, null));
        }

        [Test]
        public void NullTest_false()
        {
            // arrange
            var subject = new TestClass(new object());

            // act
            // assert
            Assert.IsFalse(If<TestClass, SimpleViolation>.Null(subject, a => a.NullableVal, null));
        }
    }
}
