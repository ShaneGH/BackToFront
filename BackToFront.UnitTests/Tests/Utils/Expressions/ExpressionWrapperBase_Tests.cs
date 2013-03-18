using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

using BackToFront.Extensions.Reflection;
using BackToFront.UnitTests.Utilities;
using BackToFront.Utils.Expressions;
using NUnit.Framework;
using System.Collections.ObjectModel;

namespace BackToFront.UnitTests.Tests.Utils.Expressions
{
    class ExpressionWrapperBase_Tests : Base.TestBase
    {
        [Test]
        public void AllExpressionWrappersAreImplemented()
        {
            var ignore = new Type[] 
            { 
                typeof(Expression),
                typeof(Expression<>),
                typeof(BlockExpression),
                typeof(ConditionalExpression),                
                typeof(DebugInfoExpression),                
                typeof(DefaultExpression),                
                typeof(DynamicExpression),                
                typeof(GotoExpression),                
                typeof(IndexExpression),     
                // important: invocation of lambda
                typeof(InvocationExpression),  
                typeof(LambdaExpression),              
                typeof(LabelExpression),                
                typeof(ListInitExpression),                
                typeof(LoopExpression),                
                typeof(MemberInitExpression),                              
                typeof(NewArrayExpression),                
                typeof(NewExpression),                
                typeof(RuntimeVariablesExpression),                
                typeof(SwitchExpression),                
                typeof(TryExpression),                
                typeof(TypeBinaryExpression)              
            };

            foreach (var expType in typeof(Expression).Assembly.GetTypes().Where(t => t.Is<Expression>() && t.IsPublic))
            {
                if (ignore.Contains(expType))
                    Assert.IsFalse(ExpressionWrapperBase.Constructors.ContainsKey(expType), string.Format("{0} is implemented, remove from ignore list", expType));
                else
                    Assert.IsTrue(ExpressionWrapperBase.Constructors.ContainsKey(expType), string.Format("{0} is not implemented", expType));
            }
        }


        #region operators

        [Test]
        public void AllExpressionTypesAreImplemented()
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
                    Assert.IsFalse(ExpressionWrapperBase.Evaluations.ContainsKey(en), string.Format("There is an implementation of {0}. Remove it from the ignore list", en));
                }
                else
                {
                    Assert.IsTrue(ExpressionWrapperBase.Evaluations.ContainsKey(en));
                }
            }
        }

        [Test]
        [TestCase(true, true, true)]
        [TestCase(true, false, false)]
        [TestCase(false, true, false)]
        [TestCase(false, false, false)]
        public void OperatorTest_AndAlso(bool param1, bool param2, bool result)
        {
            ReadOnlyCollection<ParameterExpression> parameters;
            var subject = ExpressionWrapperBase.ToWrapper<bool, bool, bool>((a, b) => a && b, out parameters) as BinaryExpressionWrapper;

            Assert.AreEqual(result, subject.CompileAndCall<bool, bool, bool>(parameters, param1, param2));
        }

        [Test]
        [TestCase(true, false)]
        [TestCase(false, true)]
        public void OperatorTest_Not(bool param, bool result)
        {
            ReadOnlyCollection<ParameterExpression> parameters;
            var subject = ExpressionWrapperBase.ToWrapper<bool, bool>((a) => !a, out parameters) as UnaryExpressionWrapper;

            Assert.AreEqual(result, subject.CompileAndCall<bool, bool>(parameters, param));
        }

        [Test]
        public void OperatorTest_Not_IsIntegralType()
        {
            ReadOnlyCollection<ParameterExpression> parameters;
            var subject = ExpressionWrapperBase.ToWrapper<int, int>((a) => ~a, out parameters) as UnaryExpressionWrapper;

            Assert.AreEqual(-9, subject.CompileAndCall<int, int>(parameters, 8));
        }

        [Test]
        public void OperatorTest_Convert()
        {
            ReadOnlyCollection<ParameterExpression> parameters;
            var subject = ExpressionWrapperBase.ToWrapper<int, double>((a) => (double)a, out parameters) as UnaryExpressionWrapper;

            Assert.AreEqual(8D, subject.CompileAndCall<int, double>(parameters, 8));
            Assert.IsInstanceOf<double>(subject.CompileAndCall<int, double>(parameters, 8));
        }

        [Test]
        [TestCase(true, true, true)]
        [TestCase(true, false, true)]
        [TestCase(false, true, true)]
        [TestCase(false, false, false)]
        public void OperatorTest_OrElse(bool param1, bool param2, bool result)
        {
            ReadOnlyCollection<ParameterExpression> parameters;
            var subject = ExpressionWrapperBase.ToWrapper<bool, bool, bool>((a, b) => a || b, out parameters) as BinaryExpressionWrapper;

            Assert.AreEqual(result, subject.CompileAndCall<bool, bool, bool>(parameters, param1, param2));
        }

        [Test]
        [TestCase(true, true, true)]
        [TestCase(true, false, true)]
        [TestCase(false, true, true)]
        [TestCase(false, false, false)]
        public void OperatorTest_Or(bool param1, bool param2, bool result)
        {
            ReadOnlyCollection<ParameterExpression> parameters;
            var subject = ExpressionWrapperBase.ToWrapper<bool, bool, bool>((a, b) => a || b, out parameters) as BinaryExpressionWrapper;

            Assert.AreEqual(result, subject.CompileAndCall<bool, bool, bool>(parameters, param1, param2));
        }

        [Test]
        [TestCase(true, true, true)]
        [TestCase(true, false, false)]
        [TestCase(false, true, false)]
        [TestCase(false, false, true)]
        public void OperatorTest_Equal(bool param1, bool param2, bool result)
        {
            ReadOnlyCollection<ParameterExpression> parameters;
            var subject = ExpressionWrapperBase.ToWrapper<bool, bool, bool>((a, b) => a == b, out parameters) as BinaryExpressionWrapper;

            Assert.AreEqual(result, subject.CompileAndCall<bool, bool, bool>(parameters, param1, param2));
        }

        [Test]
        [TestCase(true, true, false)]
        [TestCase(true, false, true)]
        [TestCase(false, true, true)]
        [TestCase(false, false, false)]
        public void OperatorTest_NotEqual(bool param1, bool param2, bool result)
        {
            ReadOnlyCollection<ParameterExpression> parameters;
            var subject = ExpressionWrapperBase.ToWrapper<bool, bool, bool>((a, b) => a != b, out parameters) as BinaryExpressionWrapper;

            Assert.AreEqual(result, subject.CompileAndCall<bool, bool, bool>(parameters, param1, param2));
        }

        [Test]
        public void OperatorTest_Add()
        {
            ReadOnlyCollection<ParameterExpression> parameters;
            var subject = ExpressionWrapperBase.ToWrapper<int, int, int>((a, b) => a + b, out parameters) as BinaryExpressionWrapper;

            Assert.AreEqual(5, subject.CompileAndCall<int, int, int>(parameters, 3, 2));
        }

        [Test]
        public void OperatorTest_Subtract()
        {
            ReadOnlyCollection<ParameterExpression> parameters;
            var subject = ExpressionWrapperBase.ToWrapper<int, int, int>((a, b) => a - b, out parameters) as BinaryExpressionWrapper;

            Assert.AreEqual(1, subject.CompileAndCall<int, int, int>(parameters, 3, 2));
        }

        [Test]
        public void OperatorTest_Multiply()
        {
            ReadOnlyCollection<ParameterExpression> parameters;
            var subject = ExpressionWrapperBase.ToWrapper<int, int, int>((a, b) => a * b, out parameters) as BinaryExpressionWrapper;

            Assert.AreEqual(6, subject.CompileAndCall<int, int, int>(parameters, 3, 2));
        }

        [Test]
        public void OperatorTest_Divide()
        {
            ReadOnlyCollection<ParameterExpression> parameters;
            var subject = ExpressionWrapperBase.ToWrapper<int, int, int>((a, b) => a / b, out parameters) as BinaryExpressionWrapper;

            Assert.AreEqual(3, subject.CompileAndCall<int, int, int>(parameters, 6, 2));
        }

        [Test]
        public void OperatorTest_ArrayIndex()
        {
            ReadOnlyCollection<ParameterExpression> parameters;
            var subject = ExpressionWrapperBase.ToWrapper<int[], int, int>((a, b) => a[b], out parameters) as BinaryExpressionWrapper;

            Assert.AreEqual(6, subject.CompileAndCall<int[], int, int>(parameters, new[] { 3, 6 }, 1));
        }

        [Test]
        [TestCase(1, 2, false)]
        [TestCase(2, 2, false)]
        [TestCase(3, 2, true)]
        public void OperatorTest_GreaterThan(int param1, int param2, bool result)
        {
            ReadOnlyCollection<ParameterExpression> parameters;
            var subject = ExpressionWrapperBase.ToWrapper<int, int, bool>((a, b) => a > b, out parameters) as BinaryExpressionWrapper;

            Assert.AreEqual(result, subject.CompileAndCall<int, int, bool>(parameters, param1, param2));
        }

        [Test]
        [TestCase(1, 2, true)]
        [TestCase(2, 2, false)]
        [TestCase(3, 2, false)]
        public void OperatorTest_LessThan(int param1, int param2, bool result)
        {
            ReadOnlyCollection<ParameterExpression> parameters;
            var subject = ExpressionWrapperBase.ToWrapper<int, int, bool>((a, b) => a < b, out parameters) as BinaryExpressionWrapper;

            Assert.AreEqual(result, subject.CompileAndCall<int, int, bool>(parameters, param1, param2));
        }

        [Test]
        [TestCase(1, 2, false)]
        [TestCase(2, 2, true)]
        [TestCase(3, 2, true)]
        public void OperatorTest_GreaterThanEqualTo(int param1, int param2, bool result)
        {
            ReadOnlyCollection<ParameterExpression> parameters;
            var subject = ExpressionWrapperBase.ToWrapper<int, int, bool>((a, b) => a >= b, out parameters) as BinaryExpressionWrapper;

            Assert.AreEqual(result, subject.CompileAndCall<int, int, bool>(parameters, param1, param2));
        }

        [Test]
        [TestCase(1, 2, true)]
        [TestCase(2, 2, true)]
        [TestCase(3, 2, false)]
        public void OperatorTest_LessThanEqualTo(int param1, int param2, bool result)
        {
            ReadOnlyCollection<ParameterExpression> parameters;
            var subject = ExpressionWrapperBase.ToWrapper<int, int, bool>((a, b) => a <= b, out parameters) as BinaryExpressionWrapper;

            Assert.AreEqual(result, subject.CompileAndCall<int, int, bool>(parameters, param1, param2));
        }

        [Test]
        public void OperatorTest_Modulo()
        {
            ReadOnlyCollection<ParameterExpression> parameters;
            var subject = ExpressionWrapperBase.ToWrapper<int, int>(a => a % 3, out parameters) as BinaryExpressionWrapper;

            Assert.AreEqual(2, subject.CompileAndCall<int, int>(parameters, 5));
        }

        [Test]
        public void OperatorTest_Coalesce()
        {
            ReadOnlyCollection<ParameterExpression> parameters;
            string string1 = "DLFNLKDNF";
            string string2 = "fw45FS";

            var subject = ExpressionWrapperBase.ToWrapper<string, string>(a => a ?? string2, out parameters) as BinaryExpressionWrapper;

            Assert.AreEqual(string1, subject.CompileAndCall<string, string>(parameters, string1));
            Assert.AreEqual(string2, subject.CompileAndCall<string, string>(parameters, null));
        }

        [Test]
        [TestCase(true, true, false)]
        [TestCase(true, false, true)]
        [TestCase(false, true, true)]
        [TestCase(false, false, false)]
        public void OperatorTest_ExclusiveOr(bool param1, bool param2, bool result)
        {
            ReadOnlyCollection<ParameterExpression> parameters;
            var subject = ExpressionWrapperBase.ToWrapper<bool, bool, bool>((a, b) => a ^ b, out parameters) as BinaryExpressionWrapper;

            Assert.AreEqual(result, subject.CompileAndCall<bool, bool, bool>(parameters, param1, param2));
        }

        #endregion
    }
}
