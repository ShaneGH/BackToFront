﻿using System;
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
        /*<?xml version="1.0" encoding="UTF-8" ?>
<testsuites>
  <testsuite name="C:\Dev\Apps\BackToFront\BackToFront.Tests\Javascript\UnitTests\BackToFront.ts" tests="1" failures="0" time="41">
    <testcase name="Hello" />
  </testsuite>
</testsuites>
*/

        public const string TestResultsFile = "JSTestResults.xml";

        [Test]
        public void Run()
        {
            using (var process = new Process { StartInfo = new ProcessStartInfo() })
            {
                process.StartInfo.WindowStyle = System.Diagnostics.ProcessWindowStyle.Hidden;
                process.StartInfo.FileName = @"Chutzpah\chutzpah.console.exe";
                process.StartInfo.Arguments = @"/path Javascript\UnitTests /testMode JavaScript /junit JSTestResults.xml";
                process.Start();
                process.WaitForExit();
            }

            using (var file = new FileStream(TestResultsFile, FileMode.Open))
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

        public override void TestFixtureSetUp()
        {
            base.TestFixtureSetUp();
        }
    }
}