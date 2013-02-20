using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BackToFront.UnitTests
{
    public class Program
    {
        public static void Main(string[] args)
        {
            RunTest<Tests.Logic.TestPass3Test>(a => a.If_Or(true, false));
        }

        public static void RunTest<TTestClass>(Action<TTestClass> runSingleTest)
            where TTestClass : Base.TestBase, new()
        {
            var test = new TTestClass();

            try
            {
                test.TestFixtureSetUp();
                runSingleTest(test);
            }
            finally
            {
                test.TestFixtureTearDown();
            }
        }
    }
}
