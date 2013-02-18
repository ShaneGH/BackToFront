using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using NUnit.Framework;

namespace BackToFront.UnitTests.Base
{
    public class TestBase
    {
        [SetUp]
        public virtual void Setup() { }

        [TearDown]
        public virtual void TearDown() { }

        [TestFixtureSetUp]
        public virtual void TestFixtureSetUp() { }

        [TestFixtureTearDown]
        public virtual void TestFixtureTearDown() { }
    }
}
