using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

using BackToFront;
using BackToFront.Logic;
using BackToFront.Validate;
using NUnit.Framework;

namespace BackToFront.UnitTests
{
    [TestFixture]
    public class Testpad
    {
        [Test]
        public void TestExpressionEquality()
        {
            Expression<Func<object, object>> i1 = a => a;
            Expression<Func<object, object>> i2= a => a;

            Assert.IsFalse(i1 == i2);
            Assert.IsFalse(i1.Equals(i2));
        }

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

        public static void SetupTestpad()
        {
            Rules.Add<Something>(trunk => trunk
                .If(b => b.Value1).IsEqualTo(0).ModelViolationIs(new ViolationClass("Invalid")));

            Rules.Add<Something>(trunk => trunk
                .If(b => b.Value1).IsEqualTo(2).And(b => b.Value2).IsEqualTo(6)
                    .Then(branch1 => 
                    {
                        branch1.RequireThat(c => c.Value3).IsEqualTo(c => c.Value4).OrModelViolationIs(new ViolationClass("Invalid"));

                        branch1.If(c => c.Value4).IsEqualTo(1).RequireThat(c => c.Value5).IsEqualTo(8).Or(c => c.Value5).IsEqualTo(8).OrModelViolationIs(new ViolationClass("Invalid"))
                            .If(c => c.Value4).IsEqualTo(0).RequireThat(c => c.Value5).IsEqualTo(8).OrModelViolationIs(new ViolationClass("Invalid"));
                    }));

            Rules.Add<Something>(trunk => trunk
                .If(b => b.Value4).IsEqualTo(0).RequireThat(b => b.Value5).IsEqualTo(8).OrModelViolationIs(new ViolationClass("Invalid")));

            Rules.Add<Something>(trunk => trunk
                .If(b => b.Value4).IsEqualTo(1).NestedAnd(u => u.Value(b => b.Value5).IsEqualTo(3).NestedAnd(a => a.Value(g => g.Value5).IsEqualTo(7))).And(c => c.Value1).IsEqualTo(7));


            new Something().Validate();
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
}