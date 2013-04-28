using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

using NUnit.Framework;
using BackToFront.Utilities;

namespace BackToFront.Tests.CSharp.UnitTests.Utils
{
    [TestFixture]
    public class ReflectionWrapper_Tests : Base.TestBase
    {
        public class TestClass<T>
        {
            public const string Prefix = "9as8ughfd9p8ayf987ga";

            public T Prop { get; set; }
            public T Field;
            public string Method()
            {
                return Prefix + Field.ToString();
            }

            public string Method(string input1)
            {
                return Method() + input1.ToString();
            }

            public string Method(string input1, int input2)
            {
                return Method(input1) + input2.ToString();
            }

            public string Method(string input1, int input2, bool input3)
            {
                return Method(input1, input2) + input3.ToString();
            }
        }

        public override void Setup()
        {
            base.Setup();

            ReflectionWrapper.UseCache = true;
        }

        #region Property

        [Test]
        public void GetPropertyTest()
        {
            // arrange
            var wrapped = new TestClass<string>();
            wrapped.Prop = "saedflkjnae094ut";
            var subject = new ReflectionWrapper(wrapped);

            // act
            var actual = subject.Property<string>("Prop");

            // assert
            Assert.AreEqual(wrapped.Prop, actual);
        }

        [Test]
        public void SetPropertyTest()
        {
            const string expected = "saedflkjnae094ut";

            // arrange
            var wrapped = new TestClass<string>();
            var subject = new ReflectionWrapper(wrapped);

            // act
            subject.Property("Prop", expected);

            // assert
            Assert.AreEqual(expected, wrapped.Prop);
        }

        #endregion

        #region Field

        [Test]
        public void GetFieldTest()
        {
            // arrange
            var wrapped = new TestClass<string>();
            wrapped.Field = "saedflkjnae094ut";
            var subject = new ReflectionWrapper(wrapped);

            // act
            var actual = subject.Field<string>("Field");

            // assert
            Assert.AreEqual(wrapped.Field, actual);
        }

        [Test]
        public void SetFieldTest()
        {
            const string expected = "saedflkjnae094ut";

            // arrange
            var wrapped = new TestClass<string>();
            var subject = new ReflectionWrapper(wrapped);

            // act
            subject.Field("Field", expected);

            // assert
            Assert.AreEqual(expected, wrapped.Field);
        }

        #endregion

        #region Method

        [Test]
        public void InvokeMethodTest_NoArgs()
        {
            CallMethodTest(a => a.Method(), a => a.Method<string>("Method"));
        }

        [Test]
        public void InvokeMethodTest_1Arg()
        {
            const string arg1 = "ILJKB:OIUHG";

            CallMethodTest(a => a.Method(arg1), a => a.Method<string, string>("Method", arg1));
        }

        [Test]
        public void InvokeMethodTest_2Args()
        {
            const string arg1 = "ILJKB:OIUHG";
            const int arg2 = 45543;

            CallMethodTest(a => a.Method(arg1, arg2), a => a.Method<string, string, int>("Method", arg1, arg2));
        }

        [Test]
        public void InvokeMethodTest_3Args()
        {
            const string arg1 = "ILJKB:OIUHG";
            const int arg2 = 45543;
            const bool arg3 = true;

            CallMethodTest(a => a.Method(arg1, arg2, arg3), a => a.Method<string, string, int, bool>("Method", arg1, arg2, arg3));
        }


        private static void CallMethodTest(Func<TestClass<string>, string> getExpected, Func<ReflectionWrapper, string> getActual)
        {
            // arrange
            var wrapped = new TestClass<string>();
            wrapped.Field = "uioho9y098yh";
            var subject = new ReflectionWrapper(wrapped);

            // act
            var actual = getActual(subject);
            var expected = getExpected(wrapped);

            // assert
            Assert.AreEqual(expected, actual);
        }

        #endregion

        #region invalid operation

        [Test]
        [ExpectedException(typeof(InvalidOperationException))]
        public void GetPropertyTest_InvalidProperty()
        {
            // arrange
            var subject = new ReflectionWrapper(new TestClass<string>());

            // act
            // assert
            subject.Property<object>("invalid");
        }

        [Test]
        [ExpectedException(typeof(InvalidOperationException))]
        public void SetPropertyTest_InvalidProperty()
        {
            // arrange
            var subject = new ReflectionWrapper(new TestClass<string>());

            // act
            // assert
            subject.Property<object>("invalid", null);
        }

        [Test]
        [ExpectedException(typeof(InvalidOperationException))]
        public void GetFieldTest_InvalidField()
        {
            // arrange
            var subject = new ReflectionWrapper(new TestClass<string>());

            // act
            // assert
            subject.Field<object>("invalid");
        }

        [Test]
        [ExpectedException(typeof(InvalidOperationException))]
        public void SetFieldTest_InvalidField()
        {
            // arrange
            var subject = new ReflectionWrapper(new TestClass<string>());

            // act
            // assert
            subject.Field<object>("invalid", null);
        }

        [Test]
        [ExpectedException(typeof(InvalidOperationException))]
        public void CallMethodTest_InvalidMethod()
        {
            // arrange
            var subject = new ReflectionWrapper(new TestClass<string>());

            // act
            // assert
            subject.Method<object>("invalid");
        }

        #endregion

        #region MethodComparer

        [TestFixture]
        public class MethodComparerTests
        {
            [Test]
            public void Equals_Test_Success()
            {
                Type type1 = typeof(string);
                string string1 = "sadfse908u-0";
                Type array1_1 = typeof(TaskCanceledException);
                Type array1_2 = typeof(ConstantExpression);

                // arrange
                var subject = new ReflectionWrapper.MethodComparer();
                var tulple1 = new Tuple<Type, string, Type[]>(type1, string1, new[] { array1_1, array1_2 });
                var tulple2 = new Tuple<Type, string, Type[]>(type1, string1, new[] { array1_1, array1_2 });

                // act
                // assert
                Assert.IsTrue(subject.Equals(tulple1, tulple2));
            }

            [Test]
            public void Equals_Test_Success_EqualArrays()
            {
                Type type1 = typeof(string);
                string string1 = "sadfse908u-0";
                Type[] array = new [] { typeof(TaskCanceledException), typeof(ConstantExpression)};

                // arrange
                var subject = new ReflectionWrapper.MethodComparer();
                var tulple1 = new Tuple<Type, string, Type[]>(type1, string1, array);
                var tulple2 = new Tuple<Type, string, Type[]>(type1, string1, array);

                // act
                // assert
                Assert.IsTrue(subject.Equals(tulple1, tulple2));
            }

            [Test]
            public void Equals_Test_Success_2NullArrays()
            {
                Type type1 = typeof(string);
                string string1 = "sadfse908u-0";

                // arrange
                var subject = new ReflectionWrapper.MethodComparer();
                var tulple1 = new Tuple<Type, string, Type[]>(type1, string1, null);
                var tulple2 = new Tuple<Type, string, Type[]>(type1, string1, null);

                // act
                // assert
                Assert.IsTrue(subject.Equals(tulple1, tulple2));
            }

            [Test]
            public void Equals_Test_Fail_InvalidArrayItem2()
            {
                Type type1 = typeof(string);
                string string1 = "sadfse908u-0";
                Type array1_1 = typeof(TaskCanceledException);
                Type array1_2 = typeof(ConstantExpression);
                Type array2_2 = typeof(Guid);

                // arrange
                var subject = new ReflectionWrapper.MethodComparer();
                var tulple1 = new Tuple<Type, string, Type[]>(type1, string1, new[] { array1_1, array1_2 });
                var tulple2 = new Tuple<Type, string, Type[]>(type1, string1, new[] { array1_1, array2_2 });

                // act
                // assert
                Assert.IsFalse(subject.Equals(tulple1, tulple2));
            }

            [Test]
            public void Equals_Test_Fail_InvalidArrayItemCount()
            {
                Type type1 = typeof(string);
                string string1 = "sadfse908u-0";
                Type array1_1 = typeof(TaskCanceledException);
                Type array1_2 = typeof(ConstantExpression);

                // arrange
                var subject = new ReflectionWrapper.MethodComparer();
                var tulple1 = new Tuple<Type, string, Type[]>(type1, string1, new[] { array1_1, array1_2 });
                var tulple2 = new Tuple<Type, string, Type[]>(type1, string1, new[] { array1_1 });

                // act
                // assert
                Assert.IsFalse(subject.Equals(tulple1, tulple2));
            }

            [Test]
            public void Equals_Test_Fail_NullArray2()
            {
                Type type1 = typeof(string);
                string string1 = "sadfse908u-0";
                Type array1_1 = typeof(TaskCanceledException);
                Type array1_2 = typeof(ConstantExpression);

                // arrange
                var subject = new ReflectionWrapper.MethodComparer();
                var tulple1 = new Tuple<Type, string, Type[]>(type1, string1, new[] { array1_1, array1_2 });
                var tulple2 = new Tuple<Type, string, Type[]>(type1, string1, null);

                // act
                // assert
                Assert.IsFalse(subject.Equals(tulple1, tulple2));
            }

            [Test]
            public void Equals_Test_Fail_bad_string()
            {
                Type type1 = typeof(string);
                string string1 = "sadfse908u-0";
                string string2 = "sdroihf908yh";
                Type array1_1 = typeof(TaskCanceledException);
                Type array1_2 = typeof(ConstantExpression);

                // arrange
                var subject = new ReflectionWrapper.MethodComparer();
                var tulple1 = new Tuple<Type, string, Type[]>(type1, string1, new[] { array1_1, array1_2 });
                var tulple2 = new Tuple<Type, string, Type[]>(type1, string2, new[] { array1_1, array1_2 });

                // act
                // assert
                Assert.IsFalse(subject.Equals(tulple1, tulple2));
            }

            [Test]
            public void Equals_Test_Bad_Type1()
            {
                Type type1 = typeof(string);
                Type type2 = typeof(object);
                string string1 = "sadfse908u-0";
                Type array1_1 = typeof(TaskCanceledException);
                Type array1_2 = typeof(ConstantExpression);

                // arrange
                var subject = new ReflectionWrapper.MethodComparer();
                var tulple1 = new Tuple<Type, string, Type[]>(type1, string1, new[] { array1_1, array1_2 });
                var tulple2 = new Tuple<Type, string, Type[]>(type2, string1, new[] { array1_1, array1_2 });

                // act
                // assert
                Assert.IsFalse(subject.Equals(tulple1, tulple2));
            }
        }

        #endregion

        #region Misc

        [Test]
        public void TupleEqualityTest()
        {
            // arrange
            var obj1 = new object();
            var obj2 = new object();
            var obj3 = new object();

            var tuple1 = new Tuple<object, object>(obj1, obj2);
            var tuple2 = new Tuple<object, object>(obj1, obj2);
            var tuple3 = new Tuple<object, object>(obj1, obj3);

            // act
            // assert
            Assert.IsTrue(tuple1.Equals(tuple2));
            Assert.IsFalse(tuple1.Equals(tuple3));
        }

        #endregion
    }
}