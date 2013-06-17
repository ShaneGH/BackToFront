using System.Collections.Generic;
using System.Runtime.Serialization;

namespace BackToFront.Meta
{
    [DataContract]
    public class RuleCollectionMeta
    {
        [DataMember]
        public string Entity { get; set; }

        [DataMember]
        public IList<RuleMeta> Rules { get; set; }
    }
}
