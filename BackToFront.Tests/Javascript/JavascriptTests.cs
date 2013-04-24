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
    public class JavascriptTests
    {
        /*<?xml version="1.0" encoding="UTF-8" ?>
<testsuites>
  <testsuite name="C:\Dev\Apps\BackToFront\BackToFront.Tests\Javascript\UnitTests\BackToFront.ts" tests="1" failures="0" time="41">
    <testcase name="Hello" />
  </testsuite>
</testsuites>
*/

        public const string TestResultsFile = "JSTestResults.xml";

        [Test]
        [Explicit]
        public void Run()
        {
            //$(TargetDir)Chutzpah\chutzpah.console.exe /path "$(ProjectDir)Javascript\UnitTests" /testMode TypeScript /junit "$(TargetDir)JSTestResults.xml"

            using (var file = new FileStream(TestResultsFile, FileMode.Open))
            {
                var results = XDocument.Load(file).Elements()
                    .Where(e => e.Name == "testsuites");

                foreach (var result in results.Elements().Where(e => e.Name == "testsuite"))
                {
                    if (int.Parse(result.Attributes().First(a => a.Name == "failures").Value) != 0)
                        Assert.Fail();
                }
            }
        }
    }
}
