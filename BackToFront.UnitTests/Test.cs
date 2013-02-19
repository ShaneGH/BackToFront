using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using BackToFront;
using BackToFront.Extensions;
using NUnit.Framework;

namespace BackToFront.UnitTests
{
    /*
     * Violation, Require, Then, And
     */
    [TestFixture]
    public class Test
    {
        public class ViolationClass : IViolation
        {
            private readonly string _UserMessage;
            public ViolationClass(string userMessage)
            {
                _UserMessage = userMessage;
            }

            public string UserMessage
            {
                get { return _UserMessage; }
            }
        }

        public static void Testtt()
        {
            Rules.Add<Something>(trunk => trunk
                .If(b => b.Value1).IsEqualTo(0).ModelViolationIs(new ViolationClass("Invalid"))
                .If(b => b.Value1).IsEqualTo(2).And(b => b.Value2).IsEqualTo(6)
                    .Then(branch1 => 
                    {
                        branch1.RequireThat(c => c.Value3).IsEqualTo(c => c.Value4).OrModelViolationIs(new ViolationClass("Invalid"));

                        branch1.If(c => c.Value4).IsEqualTo(1).RequireThat(c => c.Value5).IsEqualTo(8).OrModelViolationIs(new ViolationClass("Invalid"))
                            .If(c => c.Value4).IsEqualTo(0).RequireThat(c => c.Value5).IsEqualTo(8).OrModelViolationIs(new ViolationClass("Invalid"));
                    })
                .If(b => b.Value4).IsEqualTo(0).RequireThat(b => b.Value5).IsEqualTo(8).OrModelViolationIs(new ViolationClass("Invalid")));


            new Something().Validate();
        }

        [Test]
        public void TestX()
        {
            Func<object, object> i1 = a => a;
            Func<object, object> i2 = a => a;

            Assert.IsFalse(i1 == i2);
            Assert.IsFalse(i1.Equals(i2));
        }

        [Test]
        public void TestY()
        {
            Func<object, object> i1 = a => a;
            Func<object, object> i2 = a => a;

            Assert.IsTrue(null == null);
        }

        [Test]
        public void TestZ()
        {
            // 1 false
            Assert.IsTrue(false && true || true);
            Assert.IsTrue(false || true && true);

            Assert.IsTrue(true && false || true);
            Assert.IsTrue(true || false && true);

            Assert.IsTrue(true && true || false);
            Assert.IsTrue(true || true && false);

            // 2 false
            Assert.IsFalse(true && false || false);
            Assert.IsTrue(true || false && false);

            Assert.IsFalse(false && true || false);
            Assert.IsFalse(false || true && false);

            Assert.IsTrue(false && false || true);
            Assert.IsFalse(false || false && true);
        }
    }

    public class Something
    {
        public object Value1 { get; set; }
        public object Value2 { get; set; }
        public object Value3 { get; set; }
        public object Value4 { get; set; }
        public object Value5 { get; set; }
    }

    public class SomethingElse
    {
        public int Value { get; set; }
    }
}