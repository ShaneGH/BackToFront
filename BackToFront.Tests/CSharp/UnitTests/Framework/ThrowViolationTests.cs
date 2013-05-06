using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BackToFront.Tests.Base;
using M = Moq;
using NUnit.Framework;


using BackToFront.Framework;
using BackToFront.Tests.Utilities;
using BackToFront.Expressions.Visitors;
using BackToFront.Utilities;
using System.Linq.Expressions;
namespace BackToFront.Tests.CSharp.UnitTests.Framework
{
    [TestFixture]
    public class ThrowViolationTests : TestBase
    {
        public class Violation : IViolation
        {
            public string UserMessage
            {
                get { throw new NotImplementedException(); }
            }

            public object ViolatedEntity { get; set; }
            public IEnumerable<MemberChainItem> Violated { get; set; }
        }

        public class Accessor : ThrowViolation<object>
        {
            public Accessor(Func<object, IViolation> violation, IEnumerable<MemberChainItem> items)
                : base(violation, null, items) { }

            public Expression __NewCompile(SwapPropVisitor visitor)
            {
                return _Compile(visitor);
            }
        }

        public class TestClass : TestViolation
        {
            public TestClass(string message)
                : base(message)
            { }

            public object Property { get; set; }
        }

        //[Test]
        //public void _NewCompileTest()
        //{
        //    // arrange
        //    var item = new object();
        //    var violation = new Violation();

        //    MemberChainItem mci = new MemberChainItem(typeof(int));
        //    var subject = new Accessor(a => violation, new []{ mci });
        //    SwapPropVisitor visitor = new SwapPropVisitor();
        //    var ctxt = new ValidationContextX(true, null, null);

        //    // act
        //    subject.__NewCompile(new SwapPropVisitor())(item, ctxt);

        //    // assert
        //    Assert.AreEqual(1, ctxt.Violations.Count());
        //    Assert.AreEqual(violation, ctxt.Violations.First());

        //    Assert.AreEqual(item, violation.ViolatedEntity);
        //    Assert.AreEqual(1, violation.Violated.Count());
        //    Assert.AreEqual(mci, violation.Violated.First());
        //}
    }
}
