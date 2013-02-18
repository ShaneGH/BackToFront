using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


using Moq;
using BackToFront.Logic;
using BackToFront.Framework.Base;
using NUnit.Framework;

namespace BackToFront.UnitTests.Tests.Framework.Base
{
    [TestFixture]
    public class OperandTests
    {
        //private class TestClass : OperatorBase<object, Utilities.SimpleViolation>
        //{
        //    public bool ContinueVal;

        //    public Action NextPathCalled;
        //    public Action<bool> ContinueCalled;

        //    public TestClass()
        //        : base(a => a, new BackToFront.Framework.Rule<object,Utilities.SimpleViolation>())
        //    {
        //    }

        //    protected override IEnumerable<IValidatablePathElement<object>> NextPathElement
        //    {
        //        get { NextPathCalled(); yield break; }
        //    }
        //}

        //[Test]
        //[TestCase(true, false)]
        //[TestCase(true, true)]
        //[TestCase(false, false)]
        //[TestCase(false, true)]
        //public void ValidateTest(bool continueResult, bool operatorResult)
        //{
        //    // arrange
        //    var continueCalled = false;
        //    var nextPathCalled = false;
        //    var subject = new TestClass
        //    {
        //        ContinueVal = continueResult,
        //        ContinueCalled = a => { Assert.AreEqual(operatorResult, a); continueCalled = true; },
        //        NextPathCalled = () => nextPathCalled = true
        //    };

        //    // act
        //    subject.Validate(null, (a, b, c) => operatorResult, null);

        //    // assert
        //    Assert.IsTrue(continueCalled);
        //    Assert.AreEqual(subject.ContinueVal, nextPathCalled);
        //}

        //[Test]
        //[TestCase(true, false)]
        //[TestCase(true, true)]
        //[TestCase(false, false)]
        //[TestCase(false, true)]
        //public void ValidateAllTest(bool continueResult, bool operatorResult)
        //{
        //    // arrange
        //    var continueCalled = false;
        //    var nextPathCalled = false;
        //    var subject = new TestClass
        //    {
        //        ContinueVal = continueResult,
        //        ContinueCalled = a => { Assert.AreEqual(operatorResult, a); continueCalled = true; },
        //        NextPathCalled = () => nextPathCalled = true
        //    };

        //    // act
        //    subject.ValidateAll(null, (a, b, c) => operatorResult, null, new List<IViolation>());

        //    // assert
        //    Assert.IsTrue(continueCalled);
        //    Assert.AreEqual(subject.ContinueVal, nextPathCalled);
        //}
    }
}
