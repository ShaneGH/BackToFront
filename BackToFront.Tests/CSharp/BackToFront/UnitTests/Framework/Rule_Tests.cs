using BackToFront.Framework;
using BackToFront.Framework.Base;
using BackToFront.Tests.Utilities;
using Moq;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using U = BackToFront.Utilities;
using BackToFront.Validation;
using BackToFront.Expressions.Visitors;
using System.Linq.Expressions;

namespace BackToFront.Tests.CSharp.UnitTests.Framework
{
    [TestFixture]
    public class Rule_Tests : BackToFront.Tests.Base.TestBase
    {
        public class Accessor<TEntity> : Rule<TEntity>
            where TEntity : class
        {
            public Accessor()
                : base(null) { }

            public Expression __NewCompile(ExpressionMocker visitor)
            {
                return _Compile(visitor);
            }
        }

        [Test]
        public void AllSubRules_Test()
        {
            // arrange
            var r1 = new Rule<object>();
            var r2 = new Rule<object>(r1);
            var r3 = new Rule<object>(r2);
            var r4 = new Rule<object>(r3);
            var r5 = new Rule<object>(r1);
            var r6 = new Rule<object>(r2);

            // act
            var result = r1.AllAncestorRules.ToArray();

            // assert
            Assert.AreEqual(5, result.Count());
            Assert.IsTrue(result.Contains(r2));
            Assert.IsTrue(result.Contains(r3));
            Assert.IsTrue(result.Contains(r4));
            Assert.IsTrue(result.Contains(r5));
            Assert.IsTrue(result.Contains(r6));
        }

        [Test]
        public void RequireThat_Test()
        {
            // arrange
            var subject = new Accessor<object>();

            // act
            var result = subject.RequireThat(a => true);
            var pe = subject.AllPossiblePaths;

            // assert
            Assert.AreEqual(1, pe.Count(a => a != null));
            Assert.AreEqual(result, pe.First(a => a != null));
        }

        [Test]
        public void If_Test()
        {
            // arrange
            var subject = new Rule<object>();

            // act
            var result = subject.If(a => true);
            var pe = subject.AllPossiblePaths;

            // assert
            Assert.AreEqual(1, pe.Count(a => a != null));
            Assert.AreEqual(result, ((MultiCondition<object>)pe.First(a => a != null)).If.Last().Action);
        }

        [Test]
        public void Else_Test()
        {
            // arrange
            var subject = new Rule<object>();
            var spv = new ExpressionMocker(typeof(object));

            // act
            var result = subject.Else;
            var pe = subject.AllPossiblePaths;

            // assert
            Assert.AreEqual(1, pe.Count(a => a != null));
            Assert.AreEqual(result, ((MultiCondition<object>)pe.First(a => a != null)).If.Last().Action);

            var compiled = Expression.Lambda<Func<object, ValidationContext, bool>>(((MultiCondition<object>)pe.First(a => a != null)).If.Last().Descriptor.WrappedExpression, spv.EntityParameter, spv.ContextParameter);
            Assert.IsTrue(compiled.Compile()(null, null));
        }

        [Test]
        public void ValidatableMembers_Test()
        {
            // arrange
            var subject = new Rule<object>();

            var item1 = new U.MemberChainItem(typeof(string));
            var v1 = new Mock<IValidate<object>>();
            v1.Setup(a => a.ValidationSubjects).Returns(new[] { item1 });
            subject.Register(v1.Object);

            var item2 = new U.MemberChainItem(typeof(int));
            var v2 = new Mock<IValidate<object>>();
            v2.Setup(a => a.ValidationSubjects).Returns(new[] { item2 });
            subject.Register(v2.Object);

            // act
            var actual = subject.ValidationSubjects;

            // assert
            Assert.AreEqual(new[] { item1, item2 }, actual.ToArray());
        }

        [Test]
        public void Meta_SmokeTest()
        {
            // arrange
            var subject = new Rule<object>();
            subject.If(a => a.Equals(true)).RequireThat(a => !a.Equals(4)).WithModelViolation("Hello")
                .ElseIf(a => a.Equals(false)).RequireThat(a => !a.Equals(3)).WithModelViolation("Hello")
                .Else.RequireThat(a => !a.Equals(3)).WithModelViolation("Hello");

            // act
            // assert
            var result = subject.Meta;
        }
    }
}