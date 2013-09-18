using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

using BackToFront.Utilities;
using BackToFront.Tests.Utilities;
using NUnit.Framework;
using BackToFront.Web.Serialization;
using System.IO;

namespace BackToFront.Tests.CSharp.UnitTests.Web
{
    [TestFixture]
    public class OneWayJsonSerializer_Tests : Base.TestBase
    {
        public class TestClass1
        {
            public int Something { get; set; }
            public string SomethingElse { get; set; }
        }

        public class TestClass2
        {
            public TestClass1 TestClass1 { get; set; }
        }

        [Test]
        public void Testt()
        {
            // Arrange
            OneWayJsonSerializer<TestClass2> subject = new OneWayJsonSerializer<TestClass2>(typeof(TestClass1));

            // Act
            using (var strm = new MemoryStream())
            {
                using (var w = new StreamWriter(strm))
                {
                    subject.WriteObject(w, new TestClass2 { TestClass1 = new TestClass1 { Something = 3, SomethingElse = "asdsad" } });
                    w.Flush();

                strm.Position = 0;
                using (var r = new StreamReader(strm))
                {
                    var xxx = r.ReadToEnd();
                }
                }
            }

            // Assert
        }
    }
}
