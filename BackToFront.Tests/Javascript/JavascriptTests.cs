using System;
using System.IO;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NUnit.Framework;

namespace BackToFront.Tests.Javascript
{
    [TestFixture]
    public class JavascriptTests
    {
        public const string TestResultsFile = "unitTestResults.xml";

        [Test]
        public void Run()
        {
            // Start the child process.
            Process testRunner = new Process();

            testRunner.StartInfo = new ProcessStartInfo(@"..\..\..\Tools\Chutzpah\chutzpah.console.exe", @"/path ..\..\BackToFront.Tests\Javascript\UnitTests /testMode JavaScript /junit " + TestResultsFile);
            testRunner.StartInfo.UseShellExecute = false;
            testRunner.StartInfo.RedirectStandardOutput = true;
            testRunner.StartInfo.WindowStyle = ProcessWindowStyle.Hidden;
            testRunner.Start();

            testRunner.StandardOutput.ReadToEnd();
            testRunner.WaitForExit();
            testRunner.Close();

            Assert.IsTrue(File.Exists(TestResultsFile));
        }
    }
}
