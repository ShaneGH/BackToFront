using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using BackToFront.Extensions.IEnumerable;

using NUnit.Framework;

namespace BackToFront.UnitTests.Tests.Extensions
{
    [TestFixture]
    public class IEnumerable_Tests
    {
        [Test]
        public void Each_Test()
        {
            const int number = 4;

            // arrange
            IEnumerable<List<int>> subject = new[] 
            {
                new List<int>(),
                new List<int>(),
                new List<int>(),
                new List<int>(),
                new List<int>(),
                new List<int>()
            };

            // act
            subject.Each(a => a.Add(number));

            // assert
            Assert.IsTrue(subject.All(a => a.Count == 1 && a[0] == number));
        }

        [Test]
        public void Each_Test2()
        {
            // arrange
            IEnumerable<List<int>> subject = new[] 
            {
                new List<int>(),
                new List<int>(),
                new List<int>(),
                new List<int>(),
                new List<int>(),
                new List<int>()
            };

            // act
            subject.Each((a, b) => a.Add(b));

            // assert
            for (int i = 0; i < subject.Count(); i++)
            {
                Assert.AreEqual(1, subject.ElementAt(i).Count);
                Assert.AreEqual(i, subject.ElementAt(i)[0]);
            }
        }

        [Test]
        public void AddRange_Test()
        {
            // arange
            var range = new int[] { 4,5,6,7,8 };
            IList<int> subject = new List<int>();

            // act
            subject.AddRange(range);

            // assert
            Assert.AreEqual(range.Length, subject.Count);
            for (int i = 0; i < subject.Count; i++)
            {
                Assert.AreEqual(range[i], subject[i]);
            }
        }
    }
}
