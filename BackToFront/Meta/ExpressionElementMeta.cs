using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using BackToFront.Enum;

namespace BackToFront.Meta
{
    [DataContract]
    public class ExpressionElementMeta
    {
        public ExpressionElementMeta() { }

        public ExpressionElementMeta(object descriptor, IEnumerable<ExpressionElementMeta> elements, ExpressionWrapperType expressionType, Type type, ExpressionElementMeta _base)
        {
            Descriptor = descriptor;
            Elements = elements ?? new ExpressionElementMeta[0];
            ExpressionType = expressionType;
            Type = type;
            Base = _base;
        }

        [DataMember]
        public object Descriptor { get; private set; }

        [DataMember]
        public IEnumerable<ExpressionElementMeta> Elements { get; private set; }

        [DataMember]
        public ExpressionWrapperType ExpressionType { get; private set; }

        //TODO: this
        //[DataMember]
        public Type Type { get; private set; }

        [DataMember]
        public ExpressionElementMeta Base { get; private set; }
    }
}
