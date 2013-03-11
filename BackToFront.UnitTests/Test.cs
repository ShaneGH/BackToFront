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
                .If(b => b.Value1 == 0).ModelViolationIs(new ViolationClass("Invalid")));

            Rules.Add<Something>(trunk => trunk
                .If(b => b.Value1 == 2 && b.Value2 == 6)
                    .Then(branch1 =>
                    {
                        branch1.RequireThat(c => c.Value3 == c.Value4).OrModelViolationIs(new ViolationClass("Invalid"));
                        branch1.If(c => c.Value4 == 1).SmartRequireThat(c => c.Value5 == 8 || c.Value5 == 8).OrModelViolationIs(new ViolationClass("Invalid"));
                        branch1.If(c => c.Value4 == 0).SmartRequireThat(c => c.Value5 == 8).OrModelViolationIs(new ViolationClass("Invalid"));
                    }));

            Rules.Add<Something>(trunk => trunk
                .If(b => b.Value4 == 0).SmartRequireThat(b => b.Value5 == 8).OrModelViolationIs(new ViolationClass("Invalid")));

            Rules.Add<Something>(trunk => trunk
                .If(b => b.Value4 == 1 && (b.Value5 == 3 && b.Value5 == 7) && b.Value1 == 7));

            new Something().Validate();
        }
    }

    public class Something
    {
        public int Value1 { get; set; }
        public int Value2 { get; set; }
        public int Value3 { get; set; }
        public int Value4 { get; set; }
        public int Value5 { get; set; }
    }
}