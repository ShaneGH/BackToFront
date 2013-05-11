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
    public class Lockable
    {
        private bool _Locked = false;

        public void Lock()
        {
            _Locked = true;
        }

        protected void Set<T>(ref T property, T value)
        {
            if(_Locked)
                throw new InvalidOperationException("##");

            property = value;
        }
    }

    [DataContract]
    public sealed class PathElementMeta
    {
        public PathElementMeta() { }

        public PathElementMeta(IEnumerable<PathElementMeta> children, object code, PathElementType type)
            : this()
        {
            Children = children.ToArray();
            Code = code;
            Type = type;
        }

        [DataMember]
        public IEnumerable<PathElementMeta> Children { get; private set; }

        [DataMember]
        public object Code { get; private set; }

        [DataMember]
        public PathElementType Type { get; private set; }
    }
}
