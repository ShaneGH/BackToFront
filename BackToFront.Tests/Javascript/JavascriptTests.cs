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
        public const string BTF = "BackToFront";

        [Test]
        public void Run()
        {
            var dir = System.Environment.CurrentDirectory.Substring(0, System.Environment.CurrentDirectory.IndexOf(BTF) + BTF.Length + 1);

            Assert.Fail(string.Format(@"{0}Tools\Chutzpah\chutzpah.console.exe", dir));

            Process testRunner = new Process();

            testRunner.StartInfo = new ProcessStartInfo(string.Format(@"{0}Tools\Chutzpah\chutzpah.console.exe", dir), string.Format(@"/path {0}BackToFront.Tests\Javascript\UnitTests /testMode TypeScript /junit {1}", dir, TestResultsFile));
            testRunner.StartInfo.UseShellExecute = false;
            testRunner.StartInfo.RedirectStandardOutput = true;
            testRunner.StartInfo.WindowStyle = ProcessWindowStyle.Hidden;
            testRunner.Start();

            testRunner.WaitForExit();
            testRunner.Close();

            Assert.IsTrue(File.Exists(TestResultsFile));
        }
    }
}
