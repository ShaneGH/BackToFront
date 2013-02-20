﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using BackToFront.Framework.Condition;
using BackToFront.Framework.Base;

using BackToFront.UnitTests.Base;
using BackToFront.UnitTests.Utilities;

using NUnit.Framework;

using BackToFront.Logic;
namespace BackToFront.UnitTests.Tests.Framework.Condition
{
    [TestFixture]
    public class OperatorsTest : TestBase
    {
        private static readonly Dictionary<bool, Dictionary<bool, Dictionary<bool, Func<bool, bool, bool, bool, bool>>>> Functions = new Dictionary<bool, Dictionary<bool, Dictionary<bool, Func<bool, bool, bool, bool, bool>>>>();
        static OperatorsTest()
        {
            Functions[false] = new Dictionary<bool, Dictionary<bool, Func<bool, bool, bool, bool, bool>>>();
            Functions[true] = new Dictionary<bool, Dictionary<bool, Func<bool, bool, bool, bool, bool>>>();
            Functions[false][false] = new Dictionary<bool, Func<bool, bool, bool, bool, bool>>();
            Functions[true][false] = new Dictionary<bool, Func<bool, bool, bool, bool, bool>>();
            Functions[false][true] = new Dictionary<bool, Func<bool, bool, bool, bool, bool>>();
            Functions[true][true] = new Dictionary<bool, Func<bool, bool, bool, bool, bool>>();

            Functions[false][false][false] = (a, b, c, d) => a || b || c || d;
            Functions[false][false][true] = (a, b, c, d) => a || b || c && d;
            Functions[false][true][false] = (a, b, c, d) => a || b && c || d;
            Functions[false][true][true] = (a, b, c, d) => a || b && c && d;
            Functions[true][false][false] = (a, b, c, d) => a && b || c || d;
            Functions[true][false][true] = (a, b, c, d) => a && b || c && d;
            Functions[true][true][false] = (a, b, c, d) => a && b && c || d;
            Functions[true][true][true] = (a, b, c, d) => a && b && c && d;
        }

        public static IEnumerable<Tuple<bool, bool, bool, bool, bool, bool, bool>> TestOperation()
        {
            bool v1 = false,
                v2 = false,
                v3 = false,
                v4 = false,
                v5 = false,
                v6 = false,
                v7 = false;
            
            do
            {
                do
                {
                    do
                    {
                        do
                        {
                            do
                            {
                                do
                                {
                                    do
                                    {
                                        yield return new Tuple<bool, bool, bool, bool, bool, bool, bool>(v1, v2, v3, v4, v5, v6, v7);
                                        v4 = !v4;
                                    } while (v4);
                                    v7 = !v7;
                                } while (v7);
                                v3 = !v3;
                            } while (v3);
                            v6 = !v6;
                        } while (v6);
                        v2 = !v2;
                    } while (v2);
                    v5 = !v5;
                } while (v5);
                v1 = !v1;
            } while (v1);
        }
        
        [Test]
        [TestCaseSource("TestOperation")]
        public void GiantOperatorTest(Tuple<bool, bool, bool, bool, bool, bool, bool> input)
        {
            // And = true, Or = false

            bool evaluation1 = input.Item1,
                evaluation2 = input.Item2,
                evaluation3 = input.Item3,
                evaluation4 = input.Item4,
                operator1 = input.Item5,
                operator2 = input.Item6,
                operator3 = input.Item7;
            
            var violation = new SimpleViolation("Something");
            Func<bool, string> op = a => a ? " AND " : " OR ";
            var log = evaluation1 + op(operator1) + evaluation2 + op(operator2) + evaluation3 + op(operator3) + evaluation4;

            var c1 = new Operators<object>(a => evaluation1, null);
            var c2 = operator1 ? c1.IsTrue().And(a => evaluation2) : c1.IsTrue().Or(a => evaluation2);
            var c3 = operator2 ? c2.IsTrue().And(a => evaluation3) : c2.IsTrue().Or(a => evaluation3);
            var c4 = operator3 ? c3.IsTrue().And(a => evaluation4) : c3.IsTrue().Or(a => evaluation4);
            c4.IsTrue().ModelViolationIs(violation);

            var result = c1.ValidateEntity(new object());
            if (result != null)
                Assert.AreEqual(violation, result);

            Assert.AreEqual(Functions[operator1][operator2][operator3](evaluation1, evaluation2, evaluation3, evaluation4), result != null, log);
        }
    }
}
