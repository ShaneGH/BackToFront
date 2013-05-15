using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using BackToFront.Meta;
using NUnit.Framework;

namespace BackToFront.Tests.Base
{
    public class MetaTestBase : TestBase
    {
        public static DataContractSerializer DefaultSerializer = new DataContractSerializer(typeof(object), ExpressionMeta.MetaTypes);
    }

    [TestFixture]
    public abstract class MetaTestBase<TMeta> : MetaTestBase
        where TMeta : ExpressionMeta
    {

        [Test]
        public void SerializationTest()
        {
            DataContractSerializer ser = null;
            if (KnownTypes != null && KnownTypes.Any())
            {
                ser = new DataContractSerializer(typeof(object), KnownTypes.Union(ExpressionMeta.MetaTypes));
            }
            else
            {
                ser = DefaultSerializer;
            }

            var item1 = Create();

            using (var stream = new MemoryStream())
            {
                ser.WriteObject(stream, item1);
                stream.Position = 0;
                Test(item1, (TMeta)ser.ReadObject(stream));
            }
        }

        public virtual Type[] KnownTypes { get { return new Type[0]; } }

        public abstract TMeta Create();

        public abstract void Test(TMeta item1, TMeta item2);
    }
}
