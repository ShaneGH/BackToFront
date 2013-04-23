using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using BackToFront.Enum;

namespace BackToFront.Meta
{
    [DataContract]
    public abstract class ExpressionElementMeta
    {
        [DataMember]
        public abstract object Descriptor { get; }

        [DataMember]
        public abstract IEnumerable<ExpressionElementMeta> Elements { get; }

        [DataMember]
        public abstract ExpressionWrapperType ExpressionType { get; }

        [DataMember]
        public abstract Type Type { get; }

        [DataMember]
        public abstract ExpressionElementMeta Base { get; }
    }
}
