using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using M = Moq;
using NUnit.Framework;
using BackToFront.Framework;
using BackToFront.Framework.Base;
using BackToFront.Extensions.IEnumerable;
using BackToFront.Utilities;
using BackToFront.Expressions.Visitors;
using System.Linq.Expressions;
using BackToFront.Tests.Base;
using BackToFront.Meta;
using BackToFront.Expressions;
using BackToFront.Enum;

namespace BackToFront.Tests.CSharp.UnitTests.Meta
{
    [TestFixture]
    public class BinaryExpressionMeta_Tests : MetaTestBase<BinaryExpressionMeta>
    {
        [Test]
        public void Constructor_Test()
        {
            // arrange
            var wrapper = (BinaryExpressionWrapper)ExpressionWrapperBase.ToWrapper<int, int>(a => a + 4);

            // act
            var subject = new BinaryExpressionMeta(wrapper);

            // assert
            Assert.AreEqual(wrapper.Left.Meta, subject.Left);
            Assert.AreEqual(wrapper.Right.Meta, subject.Right);
        }

        [Test]
        public void ExpressionType_Test()
        {
            Assert.AreEqual(new BinaryExpressionMeta().ExpressionType, ExpressionWrapperType.Binary);
        }

        public override BinaryExpressionMeta Create()
        {
            var wrapper = (BinaryExpressionWrapper)ExpressionWrapperBase.ToWrapper<int, int>(a => a + 4);

            // act
            return new BinaryExpressionMeta(wrapper);
        }

        public override void Test(BinaryExpressionMeta item1, BinaryExpressionMeta item2)
        {
            Assert.AreEqual(((ParameterExpressionMeta)item1.Left).Name, ((ParameterExpressionMeta)item2.Left).Name);
            Assert.AreEqual(((ConstantExpressionMeta)item1.Right).Value, ((ConstantExpressionMeta)item2.Right).Value);
        }
    }
}
