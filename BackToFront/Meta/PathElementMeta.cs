using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using BackToFront.Enum;
using BackToFront.Expressions;

namespace BackToFront.Meta
{
    [DataContract]
    public abstract class PathElementMeta
    {
        [DataMember]
        public abstract IEnumerable<PathElementMeta> Children { get; }

        [DataMember]
        public abstract PathElementType Type { get; }

        [DataMember]
        public abstract ExpressionElementMeta Code { get; }
    }
}
