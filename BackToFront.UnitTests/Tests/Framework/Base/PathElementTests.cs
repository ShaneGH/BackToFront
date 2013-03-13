using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

using BackToFront.Extensions;

using BackToFront.Utils;
using BackToFront.Logic;
using BackToFront.Framework.Base;
using NUnit.Framework;

using BackToFront.UnitTests.Utilities;

namespace BackToFront.UnitTests.Tests.Framework.Base
{
    [TestFixture]
    public class PathElementTests
    {
        private class TestClass : PathElement<object>
        {
            public TestClass()
                : base(null)
            { }

            public readonly IList<SimpleIValidate> Els = new List<SimpleIValidate>();

            protected override IEnumerable<PathElement<object>> NextPathElements(object subject, IEnumerable<BackToFront.Utils.Mock> mocks)
            {
                return Els;
            }

            public IViolation ValNext(object subject)
            {
                return ValidateNext(subject, null);
            }

            public void ValAllNext(object subject, IList<IViolation> list)
            {
                ValidateAllNext(subject, list, null);
            }

            public override IViolation ValidateEntity(object subject, IEnumerable<Mock> mocks)
            {
                throw new InvalidOperationException();
            }

            public override void FullyValidateEntity(object subject, IList<IViolation> violationList, IEnumerable<Mock> mocks)
            {
                throw new InvalidOperationException();
            }
        }

        [Test]
        public void ValidateTest_violation()
        {
            // arrange
            var v = new SimpleViolation("violation");

            var test = new TestClass();
            test.Els.Add(null);
            test.Els.Add(null);
            test.Els.Add(new SimpleIValidate { Violation = v });
            test.Els.Add(null);

            // act
            var result = test.ValNext(null);

            // assert
            Assert.AreEqual(v, result);
        }

        [Test]
        public void ValidateAllTest_violation()
        {
            // arrange
            var v = new SimpleViolation("violation");

            var test = new TestClass();
            test.Els.Add(null);
            test.Els.Add(null);
            test.Els.Add(new SimpleIValidate { Violation = v });
            test.Els.Add(null);
            var violations = new List<IViolation>();

            // act
            test.ValAllNext(null, violations);

            // assert
            Assert.AreEqual(1, violations.Count);
            Assert.AreEqual(v, violations.ElementAt(0));
        }

        [Test]
        [ExpectedException(typeof(InvalidOperationException))]
        public void ValidateTest_moreThan1Path()
        {
            // arrange
            var v = new SimpleViolation("violation");

            var test = new TestClass();
            test.Els.Add(new SimpleIValidate { Violation = v });
            test.Els.Add(new SimpleIValidate { Violation = v });

            // act
            var result = test.ValNext(null);
        }

        [Test]
        public void ValidateTest_Noviolation()
        {
            // arrange
            var test = new TestClass();
            test.Els.Add(new SimpleIValidate());

            // act
            var result = test.ValNext(null);

            // assert
            Assert.IsNull(result);
        }

        [Test]
        public void ValidateTest_noNextPath()
        {
            // arrange
            var test = new TestClass();

            // act
            var result = test.ValNext(null);

            // assert
            Assert.IsNull(result);
        }
    }
}
