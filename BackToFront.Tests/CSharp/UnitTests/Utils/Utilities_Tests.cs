using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using U = BackToFront.Utilities;
using NUnit.Framework;

namespace BackToFront.Tests.CSharp.UnitTests.Utils
{
    [TestFixture]
    public class Utilities_Tests : Base.TestBase
    {
        public class TestClass
        {
            private readonly int HashCode;
            private readonly Func<TestClass, bool> Eq;

            public TestClass(int hashCode, Func<TestClass, bool> eq)
            {
                HashCode = hashCode;
                Eq = eq;
            }

            public override int GetHashCode()
            {
                return HashCode;
            }

            public override bool Equals(object obj)
            {
                return Eq == null ? base.Equals(obj) : Eq((TestClass)obj);
            }
        }

        [Test]
        public void BindOperatorToEquals_Test_BothNull() 
        {
            // arrange
            // act
            // assert
            Assert.IsTrue(U.Utils.BindOperatorToEquals<object>(null, null));
        }
        
        [Test]
        public void BindOperatorToEquals_Test_1Null() 
        {
            // arrange
            // act
            // assert
            Assert.IsFalse(U.Utils.BindOperatorToEquals<object>(null, new object()));
        }

        [Test]
        public void BindOperatorToEquals_Test_NoNulls_AreEqual()
        {
            const int hash = 23423;

            // arrange
            var i2 = new TestClass(hash, null);
            var i1 = new TestClass(hash, a => a == i2);

            // act
            // assert
            Assert.IsTrue(U.Utils.BindOperatorToEquals<TestClass>(i1, i2));
        }

        [Test]
        public void BindOperatorToEquals_Test_NoNulls_NotEqual()
        {
            const int hash = 23423;

            // arrange
            var i2 = new TestClass(hash, null);
            var i1 = new TestClass(hash, a => a != i2);

            // act
            // assert
            Assert.IsFalse(U.Utils.BindOperatorToEquals<TestClass>(i1, i2));
        }

        [Test]
        public void BindOperatorToEquals_Test_NoNulls_InvalidHash()
        {
            const int hash1 = 23423;
            const int hash2 = 4567456;

            // arrange
            var i2 = new TestClass(hash2, null);
            var i1 = new TestClass(hash1, a => a == i2);

            // act
            // assert
            Assert.IsFalse(U.Utils.BindOperatorToEquals<TestClass>(i1, i2));
        }
    }
}
