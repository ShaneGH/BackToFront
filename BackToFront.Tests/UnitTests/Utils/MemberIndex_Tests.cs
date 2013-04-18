using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using BackToFront.Utils;
using NUnit.Framework;

namespace BackToFront.Tests.UnitTests.Utils
{
    [TestFixture]
    public class MemberIndex_Tests
    {
        [Test]
        public void EqualityTest()
        {
            Action<MemberIndex, MemberIndex, bool> test = (item1, item2, areEqual) =>
            {
                if (areEqual)
                {
                    Assert.IsTrue(item1 == item2);
                    Assert.IsFalse(item1 != item2);
                }
                else
                {
                    Assert.IsFalse(item1 == item2);
                    Assert.IsTrue(item1 != item2);
                }
            };

            var tmp = new MemberIndex();
            test(tmp, tmp, true);
            test(new MemberIndex(), new MemberIndex(), true);
            test(new MemberIndex(), new MemberIndex(2), false);
            test(new MemberIndex(2), new MemberIndex(2), true);
            test(new MemberIndex(3), new MemberIndex(2), false);
        }
    }
}
