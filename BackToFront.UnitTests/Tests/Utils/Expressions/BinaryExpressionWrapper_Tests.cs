using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

using BackToFront.UnitTests.Utilities;
using BackToFront.Utils.Expressions;
using NUnit.Framework;

namespace BackToFront.UnitTests.Tests.Utils.Expressions
{
    [TestFixture]
    public class BinaryExpressionWrapper_Tests : Base.TestBase
    {
        [Test]
        public void AllAreImplemented()
        {
            var ignore = new ExpressionType[] 
            {
                ExpressionType.AddAssign,
                ExpressionType.AddAssignChecked,
                ExpressionType.AddChecked,
                ExpressionType.And,
                ExpressionType.AndAssign,
                ExpressionType.ArrayLength,
                ExpressionType.Assign,
                ExpressionType.Block,
                ExpressionType.Call,
                ExpressionType.Conditional,
                ExpressionType.Constant,
                ExpressionType.Convert,
                ExpressionType.ConvertChecked,
                ExpressionType.DebugInfo,
                ExpressionType.Decrement,
                ExpressionType.Default,
                ExpressionType.DivideAssign,
                ExpressionType.Dynamic,
                ExpressionType.ExclusiveOrAssign,
                ExpressionType.Extension,
                ExpressionType.Goto,
                ExpressionType.Increment,
                ExpressionType.Index,
                ExpressionType.Invoke,
                ExpressionType.IsFalse,
                ExpressionType.IsTrue,
                ExpressionType.Label,
                ExpressionType.Lambda,
                ExpressionType.LeftShift,
                ExpressionType.LeftShiftAssign,
                ExpressionType.ListInit,
                ExpressionType.Loop,
                ExpressionType.MemberAccess,
                ExpressionType.MemberInit,
                ExpressionType.ModuloAssign,
                ExpressionType.MultiplyAssign,
                ExpressionType.MultiplyAssignChecked,
                ExpressionType.MultiplyChecked,
                ExpressionType.Negate,
                ExpressionType.NegateChecked,
                ExpressionType.New,
                ExpressionType.NewArrayBounds,
                ExpressionType.NewArrayInit,
                ExpressionType.Not,
                ExpressionType.OnesComplement,
                ExpressionType.Or,
                ExpressionType.OrAssign,
                ExpressionType.Parameter,
                ExpressionType.PostDecrementAssign,
                ExpressionType.PostIncrementAssign,
                ExpressionType.PowerAssign,
                ExpressionType.PreDecrementAssign,
                ExpressionType.PreIncrementAssign,
                ExpressionType.Quote,
                ExpressionType.RightShift,
                ExpressionType.RightShiftAssign,
                ExpressionType.RuntimeVariables,
                ExpressionType.SubtractAssign,
                ExpressionType.SubtractAssignChecked,
                ExpressionType.SubtractChecked,
                ExpressionType.Switch,
                ExpressionType.Throw,
                ExpressionType.Try,
                ExpressionType.TypeAs,
                ExpressionType.TypeEqual,
                ExpressionType.TypeIs,
                ExpressionType.UnaryPlus,
                ExpressionType.Unbox,
            };

            foreach (var en in System.Enum.GetValues(typeof(ExpressionType)).Cast<ExpressionType>())
            {
                if (ignore.Contains(en))
                {
                    Assert.IsFalse(BinaryExpressionWrapper.Evaluations.ContainsKey(en), string.Format("There is an implementation of {0}. Remove it from the ignore list", en));
                }
                else
                {
                    Assert.IsTrue(BinaryExpressionWrapper.Evaluations.ContainsKey(en));
                }
            }
        }

        [Test]
        public void IsSameExpression_Test()
        {
            // arrange
            Expression<Func<int, bool>> func1 = a => a == 4;
            Expression<Func<int, bool>> func2 = a => a == 4;
            Expression<Func<int, bool>> func3 = a => a == 5;
            Expression<Func<int, bool>> func4 = a => a != 4;
            var subject = new FuncExpressionWrapper<int, bool>(func1).Body as BinaryExpressionWrapper;

            // act
            // assert
            Assert.IsTrue(subject.IsSameExpression(new FuncExpressionWrapper<int, bool>(func1).Body as BinaryExpressionWrapper));
            Assert.IsTrue(subject.IsSameExpression(new FuncExpressionWrapper<int, bool>(func2).Body as BinaryExpressionWrapper));
            Assert.IsFalse(subject.IsSameExpression(new FuncExpressionWrapper<int, bool>(func3).Body as BinaryExpressionWrapper));
            Assert.IsFalse(subject.IsSameExpression(new FuncExpressionWrapper<int, bool>(func4).Body as BinaryExpressionWrapper));
        }

        [Test]
        public void EvaluateTest()
        {
            // arange
            var subject = new FuncExpressionWrapper<int, bool>(a => a == 0);
            var ex = new Tuple<Expression<Func<int, bool>>, bool>(a => a == 0, true);

            // act
            // assert            
            Assert.IsTrue((bool)subject.Evaluate(new object[] { 0 }));
            Assert.IsFalse((bool)subject.Evaluate(new object[] { 1 }));

            Assert.IsTrue((bool)subject.Evaluate(new object[] { 0 }, new []{ ex }));
            Assert.IsTrue((bool)subject.Evaluate(new object[] { 1 }, new[] { ex }));
        }

        [Test]
        public void Deep_EvaluateTest()
        {
            // arange
            var subject = new FuncExpressionWrapper<int, string>(a => (a == 0).ToString());
            var ex = new Tuple<Expression<Func<int, bool>>, bool>(a => a == 0, true);

            // act
            // assert            
            Assert.AreEqual(true.ToString(), subject.Evaluate(new object[] { 0 }));
            Assert.AreEqual(false.ToString(), subject.Evaluate(new object[] { 1 }));

            Assert.AreEqual(true.ToString(), subject.Evaluate(new object[] { 0 }, new[] { ex }));
            Assert.AreEqual(true.ToString(), subject.Evaluate(new object[] { 1 }, new[] { ex }));
        }

        #region operators

        [Test]
        [TestCase(true, true, true)]
        [TestCase(true, false, false)]
        [TestCase(false, true, false)]
        [TestCase(false, false, false)]
        public void OperatorTest_AndAlso(bool param1, bool param2, bool result)
        {
            var subject = new FuncExpressionWrapper<bool, bool, bool>((a, b) => a && b).Body as BinaryExpressionWrapper;

            Assert.AreEqual(result, subject.Evaluate(new object[] { param1, param2 }));
        }

        [Test]
        [TestCase(true, true, true)]
        [TestCase(true, false, true)]
        [TestCase(false, true, true)]
        [TestCase(false, false, false)]
        public void OperatorTest_OrElse(bool param1, bool param2, bool result)
        {
            var subject = new FuncExpressionWrapper<bool, bool, bool>((a, b) => a || b).Body as BinaryExpressionWrapper;

            Assert.AreEqual(result, subject.Evaluate(new object[] { param1, param2 }));
        }

        [Test]
        [TestCase(true, true, true)]
        [TestCase(true, false, true)]
        [TestCase(false, true, true)]
        [TestCase(false, false, false)]
        public void OperatorTest_Or(bool param1, bool param2, bool result)
        {
            var subject = new FuncExpressionWrapper<bool, bool, bool>((a, b) => a || b).Body as BinaryExpressionWrapper;

            Assert.AreEqual(result, subject.Evaluate(new object[] { param1, param2 }));
        }

        [Test]
        [TestCase(true, true, true)]
        [TestCase(true, false, false)]
        [TestCase(false, true, false)]
        [TestCase(false, false, true)]
        public void OperatorTest_Equal(bool param1, bool param2, bool result)
        {
            var subject = new FuncExpressionWrapper<bool, bool, bool>((a, b) => a == b).Body as BinaryExpressionWrapper;

            Assert.AreEqual(result, subject.Evaluate(new object[] { param1, param2 }));
        }

        [Test]
        [TestCase(true, true, false)]
        [TestCase(true, false, true)]
        [TestCase(false, true, true)]
        [TestCase(false, false, false)]
        public void OperatorTest_NotEqual(bool param1, bool param2, bool result)
        {
            var subject = new FuncExpressionWrapper<bool, bool, bool>((a, b) => a != b).Body as BinaryExpressionWrapper;

            Assert.AreEqual(result, subject.Evaluate(new object[] { param1, param2 }));
        }

        [Test]
        public void OperatorTest_Add()
        {
            var subject = new FuncExpressionWrapper<int, int, int>((a, b) => a + b).Body as BinaryExpressionWrapper;

            Assert.AreEqual(5, subject.Evaluate(new object[] { 3, 2 }));
        }

        [Test]
        public void OperatorTest_Subtract()
        {
            var subject = new FuncExpressionWrapper<int, int, int>((a, b) => a - b).Body as BinaryExpressionWrapper;

            Assert.AreEqual(1, subject.Evaluate(new object[] { 3, 2 }));
        }

        [Test]
        public void OperatorTest_Multiply()
        {
            var subject = new FuncExpressionWrapper<int, int, int>((a, b) => a * b).Body as BinaryExpressionWrapper;

            Assert.AreEqual(6, subject.Evaluate(new object[] { 3, 2 }));
        }

        [Test]
        public void OperatorTest_Divide()
        {
            var subject = new FuncExpressionWrapper<int, int, int>((a, b) => a / b).Body as BinaryExpressionWrapper;

            Assert.AreEqual(3, subject.Evaluate(new object[] { 6, 2 }));
        }

        [Test]
        public void OperatorTest_ArrayIndex()
        {
            var subject = new FuncExpressionWrapper<int[], int, int>((a, b) => a[b]).Body as BinaryExpressionWrapper;

            Assert.AreEqual(6, subject.Evaluate(new object[] { new[] { 3, 6 }, 1 }));
        }

        [Test]
        [TestCase(1, 2, false)]
        [TestCase(2, 2, false)]
        [TestCase(3, 2, true)]
        public void OperatorTest_GreaterThan(int param1, int param2, bool result)
        {
            var subject = new FuncExpressionWrapper<int, int, bool>((a, b) => a > b).Body as BinaryExpressionWrapper;

            Assert.AreEqual(result, subject.Evaluate(new object[] { param1, param2 }));
        }

        [Test]
        [TestCase(1, 2, true)]
        [TestCase(2, 2, false)]
        [TestCase(3, 2, false)]
        public void OperatorTest_LessThan(int param1, int param2, bool result)
        {
            var subject = new FuncExpressionWrapper<int, int, bool>((a, b) => a < b).Body as BinaryExpressionWrapper;

            Assert.AreEqual(result, subject.Evaluate(new object[] { param1, param2 }));
        }

        [Test]
        [TestCase(1, 2, false)]
        [TestCase(2, 2, true)]
        [TestCase(3, 2, true)]
        public void OperatorTest_GreaterThanEqualTo(int param1, int param2, bool result)
        {
            var subject = new FuncExpressionWrapper<int, int, bool>((a, b) => a >= b).Body as BinaryExpressionWrapper;

            Assert.AreEqual(result, subject.Evaluate(new object[] { param1, param2 }));
        }

        [Test]
        [TestCase(1, 2, true)]
        [TestCase(2, 2, true)]
        [TestCase(3, 2, false)]
        public void OperatorTest_LessThanEqualTo(int param1, int param2, bool result)
        {
            var subject = new FuncExpressionWrapper<int, int, bool>((a, b) => a <= b).Body as BinaryExpressionWrapper;

            Assert.AreEqual(result, subject.Evaluate(new object[] { param1, param2 }));
        }

        [Test]
        public void OperatorTest_Modulo()
        {
            var subject = new FuncExpressionWrapper<int, int>(a => a % 3).Body as BinaryExpressionWrapper;

            Assert.AreEqual(2, subject.Evaluate(new object[] { 5 }));
        }

        [Test]
        public void OperatorTest_Coalesce()
        {
            string string1 = "DLFNLKDNF";
            string string2 = "fw45FS";

            var subject = new FuncExpressionWrapper<string, string>(a => a ?? string2).Body as BinaryExpressionWrapper;

            Assert.AreEqual(string1, subject.Evaluate(new object[] { string1 }));
            Assert.AreEqual(string2, subject.Evaluate(new object[] { null }));
        }

        [Test]
        [TestCase(true, true, false)]
        [TestCase(true, false, true)]
        [TestCase(false, true, true)]
        [TestCase(false, false, false)]
        public void OperatorTest_ExclusiveOr(bool param1, bool param2, bool result)
        {
            var subject = new FuncExpressionWrapper<bool, bool, bool>((a, b) => a ^ b).Body as BinaryExpressionWrapper;

            Assert.AreEqual(result, subject.Evaluate(new object[] { param1, param2 }));
        }

        #endregion
    }
}
