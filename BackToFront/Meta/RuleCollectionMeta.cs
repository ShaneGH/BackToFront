using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

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
