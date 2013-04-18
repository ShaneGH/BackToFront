﻿using BackToFront.DataAnnotations;
using NUnit.Framework;
using System;
using System.Linq;
using M = Moq;
using BackToFront.Dependency;
using System.Collections.Generic;
using DA = System.ComponentModel.DataAnnotations;

namespace BackToFront.Tests.UnitTests.DataAnnotations
{
    [TestFixture]
    public class ValidateMemberAttribute_Tests : Base.TestBase
    {
        class Class1
        {
            public Class2 Prop1;
        }

        class Class2
        {
            public Class3[] Prop2 { get; set; }
        }

        class Class3
        {
            public int Prop3;
        }

        [Test]
        public void ProcessValidationContext_Test_NoKey()
        {
            // arrange
            var ctxt = new DA.ValidationContext(new object());

            // act
            var actual = ValidateMemberAttribute.ProcessValidationContext(ctxt);

            // assert
            Assert.NotNull(actual);
            Assert.AreEqual(actual, ctxt.Items[ValidateMemberAttribute.BackToFrontValidationContext]);

        }

        [Test]
        public void ProcessValidationContext_Test_HasKey()
        {
            // arrange
            var ctxt = new DA.ValidationContext(new object());
            var expected = new BTFValidationContext(ctxt);
            ctxt.Items.Add(ValidateMemberAttribute.BackToFrontValidationContext, expected);

            // act
            var actual = ValidateMemberAttribute.ProcessValidationContext(ctxt);

            // assert
            Assert.AreEqual(expected, actual);
            Assert.AreEqual(actual, ctxt.Items[ValidateMemberAttribute.BackToFrontValidationContext]);
        }

        [Test]
        [ExpectedException(typeof(InvalidOperationException))]
        public void ProcessValidationContext_Test_InvalidKey()
        {
            // arrange
            var ctxt = new DA.ValidationContext(new object());
            ctxt.Items.Add(ValidateMemberAttribute.BackToFrontValidationContext, true);

            // act
            // assert
            var actual = ValidateMemberAttribute.ProcessValidationContext(ctxt);
        }
        
        [Test]
        public void Create_Test_WithIndex_OK()
        {
            // arrange
            var baseType = typeof(Class1);
            var p1 = "Prop1";
            var p2 = "Prop2";
            var p3 = "Prop3";
            var index = 98745;

            // act
            var actual = ValidateMemberAttribute.Create(baseType, string.Join(".", p1, p2 + "[" + index + "]", p3));

            // assert
            Assert.AreEqual(baseType, actual.Member);
            Assert.IsNull(actual.Index);

            Assert.AreEqual(typeof(Class1).GetField(p1), actual.NextItem.Member);
            Assert.IsNull(actual.NextItem.Index);

            Assert.AreEqual(typeof(Class2).GetProperty(p2), actual.NextItem.NextItem.Member);
            Assert.AreEqual(index, actual.NextItem.NextItem.Index.Value);

            Assert.AreEqual(typeof(Class3).GetField(p3), actual.NextItem.NextItem.NextItem.Member);
            Assert.IsNull(actual.NextItem.NextItem.NextItem.Index);

            Assert.IsNull(actual.NextItem.NextItem.NextItem.NextItem);
        }
    }
}