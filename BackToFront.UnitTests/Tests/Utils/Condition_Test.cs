using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using BackToFront.UnitTests.Utilities;
using BackToFront.Utils;
using NUnit.Framework;

namespace BackToFront.UnitTests.Tests.Utils
{
    [TestFixture]
    public class Condition_Test
    {
        public class TestClass
        {
            public bool Flag1 { get; set; }
            public bool Flag2 { get; set; }
            public bool Flag3 { get; set; }
            public bool Flag4 { get; set; }
        }

        public static IEnumerable<Tuple<bool, bool, bool, bool, bool, bool, bool>> TestOperation()
        {
            return BackToFront.UnitTests.Tests.Framework.Condition.OperatorsTest.TestOperation();
        }

        public static Dictionary<bool, Dictionary<bool, Dictionary<bool, Func<bool, bool, bool, bool, bool>>>> Functions
        {
            get
            {
                return BackToFront.UnitTests.Tests.Framework.Condition.OperatorsTest.Functions;
            }
        }

        [Test]
        [TestCaseSource("TestOperation")]
        public void GiantConditionTest(Tuple<bool, bool, bool, bool, bool, bool, bool> input)
        {
            // And = true, Or = false
            bool evaluation1 = input.Item1,
                evaluation2 = input.Item2,
                evaluation3 = input.Item3,
                evaluation4 = input.Item4,
                operator1 = input.Item5,
                operator2 = input.Item6,
                operator3 = input.Item7;
            
            TestClass test = new TestClass
            {
                Flag1 = evaluation1,
                Flag2 = evaluation2,
                Flag3 = evaluation3,
                Flag4 = evaluation4
            };

            var violation = new SimpleViolation("Something");
            Func<bool, string> opString = a => a ? " AND " : " OR ";
            Func<bool, Enum.LogicalOperator> opOp = a => a ? Enum.LogicalOperator.And : Enum.LogicalOperator.Or;
            var log = evaluation1 + opString(operator1) + evaluation2 + opString(operator2) + evaluation3 + opString(operator3) + evaluation4;

            var subject = new Condition<TestClass>();
            subject.Add(Enum.LogicalOperator.And, a => a.Flag1, Operators.Eq, a => true);
            subject.Add(opOp(operator1), a => a.Flag2, Operators.Eq, a => true);
            subject.Add(opOp(operator2), a => a.Flag3, Operators.Eq, a => true);
            subject.Add(opOp(operator3), a => a.Flag4, Operators.Eq, a => true);

            var result = subject.CompiledCondition(test);

            Assert.AreEqual(Functions[operator1][operator2][operator3](evaluation1, evaluation2, evaluation3, evaluation4), result, log);
        }
    }
}
