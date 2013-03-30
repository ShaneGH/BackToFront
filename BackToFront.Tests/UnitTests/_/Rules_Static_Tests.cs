using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using NUnit.Framework;
using BackToFront.Utils;
using BackToFront.Framework;

namespace BackToFront.Tests.UnitTests
{
    public partial class Rules_Tests
    {
        [Test]
        public void GetRules_Test()
        {
            // arrange
            Rules<TestClass>.AddRule(a => { });
            
            // act
            var rules = Rules.GetRules(typeof(TestClass));

            // assert
            Assert.AreEqual(Rules<TestClass>.Repository.Registered, rules);
        }
    }
}
