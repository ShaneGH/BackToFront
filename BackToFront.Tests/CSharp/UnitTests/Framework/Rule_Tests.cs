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

            public Expression __NewCompile(SwapPropVisitor visitor)
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
            var spv = new SwapPropVisitor(typeof(object));

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
        public void AffectedMembers_Test()
        {
            // arrange
            var subject = new Rule<object>();

            var item1 = new AffectedMembers { Member = new U.MemberChainItem(typeof(string))};
            var v1 = new Mock<IValidate<object>>();
            v1.Setup(a => a.AffectedMembers).Returns(new[] { item1 });
            subject.Register(v1.Object);

            var item2 = new AffectedMembers { Member = new U.MemberChainItem(typeof(string)) };
            var v2 = new Mock<IValidate<object>>();
            v2.Setup(a => a.AffectedMembers).Returns(new[] { item2 });
            subject.Register(v2.Object);

            // act
            var actual = subject.AffectedMembers;

            // assert
            Assert.IsTrue(AreKindOfEqual(new[] { item1, item2 }, actual));
        }

        //[Test]
        //public void _NewCompileTest()
        //{
        //    // arrange
        //    var subject = new Accessor<object>();
        //    subject.RequireThat(a => true);

        //    // act
        //    var result = subject.__NewCompile(new SwapPropVisitor());

        //    // assert
        //    Assert.AreNotEqual(PathElement<object>.DoNothing, result);
        //}

        //[Test]
        //public void _NewCompileTest_NoNextElement()
        //{
        //    // arrange
        //    var subject = new Accessor<object>();

        //    // act
        //    var result = subject.__NewCompile(new SwapPropVisitor());

        //    // assert
        //    Assert.AreEqual(PathElement<object>.DoNothing, result);
        //}
    }
}
