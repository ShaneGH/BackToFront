using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

using BackToFront.Extensions;

using BackToFront.Utilities;
using BackToFront.Logic;
using BackToFront.Framework.Base;
using NUnit.Framework;

using M = Moq;
using Moq.Protected;

using BackToFront.Tests.Utilities;
using BackToFront.Framework;
using BackToFront.Expressions.Visitors;
using System.Linq.Expressions;

namespace BackToFront.Tests.CSharp.UnitTests.Framework.Base
{
    [TestFixture]
    public class PathElementTests
    {
        public abstract class PathElement_Test<T> : PathElement<T>
        {
            public PathElement_Test()
                : base(null)
            { }

            public TOutput _Do<TOutput>(Func<TOutput> action)
            {
                return Do(action);
            }

            public void _Do(Action action)
            {
                Do(action);
            }
        }

        [Test]
        public void Do_Action()
        {
            // arrange
            var subject = new M.Mock<PathElement_Test<object>>();
            object val = new object();
            object newVal = new object();

            // act
            subject.Object._Do(() => { val = newVal; });

            // assert
            Assert.AreEqual(val, newVal);
        }

        [Test]
        public void Do_Func()
        {
            // arrange
            var subject = new M.Mock<PathElement_Test<object>>();
            object val = new object();

            // act
            var result = subject.Object._Do(() => val);

            // assert
            Assert.AreEqual(val, result);
        }

        [Test]
        public void Do_Locked()
        {
            // arrange
            var subject = new M.Mock<PathElement_Test<object>>();

            // act
            subject.Object._Do(() => { });

            // assert
            Assert.Throws<InvalidOperationException>(() => { subject.Object._Do(() => { }); });
        }

        [Test]
        public void NewCompileTest()
        {
            // arrange
            Expression<Action> pass = () => Assert.Pass();
            var subject = new M.Mock<PathElement<object>>(null) { CallBase = true };
            ExpressionMocker visitor = new ExpressionMocker(typeof(object));
            subject.Protected().Setup<Expression>("_Compile", ItExpr.Is<ExpressionMocker>(a => a == visitor)).Returns(pass.Body);

            // act
            var compiled = subject.Object.Compile(visitor);
            Expression.Lambda<Action<object, ValidationContext>>(compiled, visitor.EntityParameter, visitor.ContextParameter).Compile()(new object(), new ValidationContext(false, null, null));

            // assert
            Assert.Fail("Did not hit pass statement");
        }

        [Test]
        public void NewCompileTest_Break()
        {
            // arrange
            Expression<Action> fail = () => Assert.Fail();
            var subject = new M.Mock<PathElement<object>>(null) { CallBase = true };
            ExpressionMocker visitor = new ExpressionMocker(typeof(object));
            subject.Protected().Setup<Expression>("_Compile", ItExpr.Is<ExpressionMocker>(a => a == visitor)).Returns(fail);
            var ctxt = new ValidationContext(true, null, null);
            ctxt.Violations.Add(null);

            // act
            var compiled = subject.Object.Compile(visitor);
            Expression.Lambda<Action<object, ValidationContext>>(compiled, visitor.EntityParameter, visitor.ContextParameter).Compile()(new object(), new ValidationContext(false, null, null));

            // assert
            Assert.Pass("Avoid previous fail");
        }
    }
}
