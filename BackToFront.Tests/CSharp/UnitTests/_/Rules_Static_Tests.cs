using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using NUnit.Framework;
using BackToFront.Utilities;
using BackToFront.Framework;

namespace BackToFront.Tests.CSharp.UnitTests
{
    public partial class Rules_Tests
    {
        [Test]
        public void GetRules_Test()
        {
            // arrange
            Rules<TestClass2>.AddRule(a => { });
            
            // act
            var rules = Rules.GetRules(typeof(TestClass2));

            // assert
            Assert.AreEqual(Rules<TestClass2>.Repository.Registered, rules);
        }
    }
}
