using System;
using System.Linq;
using System.Linq.Expressions;
using BackToFront.Expressions;
using BackToFront.Meta;
using BackToFront.Tests.Base;
using NUnit.Framework;
using M = Moq;

namespace BackToFront.Tests.CSharp.UnitTests.Meta
{
    [TestFixture]
    public class ExpressionMeta_Tests : MetaTestBase<ExpressionMeta>
    {
        public class Accessor : ExpressionMeta
        {
            public Accessor(ExpressionWrapperBase expression)
                : base(expression)
            { }

            public Accessor()
                : base()
            { }

            public override Enum.ExpressionWrapperType ExpressionType
            {
                get { return Enum.ExpressionWrapperType.Conditional; }
            }
        }

        [Test]
        public void MetaTypesTest()
        {
            var type = typeof(ExpressionMeta);
            var types = type.Assembly.GetTypes().Where(t => t != type && type.IsAssignableFrom(t)).ToArray();

            Assert.AreEqual(types, ExpressionMeta.MetaTypes);
        }

        public override ExpressionMeta Create()
        {
            var wrapper = new M.Mock<ExpressionWrapperBase>();
            wrapper.Setup(a => a.WrappedExpression).Returns(Expression.Empty());

            return new Accessor(wrapper.Object);
        }

        public override void Test(ExpressionMeta item1, ExpressionMeta item2)
        {
            Assert.AreEqual(item1.NodeType, item2.NodeType);
        }

        public override Type[] KnownTypes
        {
            get
            {
                return new[] { typeof(Accessor) };
            }
        }
    }
}
