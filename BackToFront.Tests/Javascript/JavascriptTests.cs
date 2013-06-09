using System;
using System.IO;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Xml.Linq;
using System.Text;
using System.Xml.Serialization;
using System.Threading.Tasks;
using NUnit.Framework;

namespace BackToFront.Tests.Javascript
{
    [TestFixture]
    public class JavascriptTests : Base.TestBase
    {
        public const string BackToFrontResultsX = "BTFJSTestResults.xml";
        public const string WebExpressionsResultsX = "WEJSTestResults.xml";

        public void Run(string path, string resultsFile)
        {
            using (var process = new Process { StartInfo = new ProcessStartInfo() })
            {
                process.StartInfo.WindowStyle = System.Diagnostics.ProcessWindowStyle.Hidden;
                process.StartInfo.FileName = @"Chutzpah\chutzpah.console.exe";
                process.StartInfo.Arguments = @"/path " + path + " /testMode JavaScript /junit " + resultsFile;
                process.Start();
                process.WaitForExit();
            }

            using (var file = new FileStream(resultsFile, FileMode.Open))
            {
                var results = XDocument.Load(file).Elements()
                    .Where(e => e.Name == "testsuites");

                foreach (var result in results.Elements().Where(e => e.Name == "testsuite"))
                {
                    if (int.Parse(result.Attributes().First(a => a.Name == "failures").Value) != 0)
                    {
                        file.Position = 0;
                        using (var reader = new StreamReader(file))
                        {
                            Assert.Fail(reader.ReadToEnd());
                        }
                    }
                }
            }
        }

        [Test]
        public void RunBackToFrontTests()
        {
            Run(@"Javascript\BackToFront\UnitTests", BackToFrontResultsX);
        }

        [Test]
        public void RunWebExpressionsTests()
        {
            Run(@"Javascript\WebExpressions\UnitTests", WebExpressionsResultsX);
        }
    }
}