using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace BackToFront.Utilities
{
    public class ValidationContext
    {
        public Mocks Mocks { get; set; }
        public readonly IList<MemberChainItem> ViolatedMembers = new List<MemberChainItem>();

        public ValidationContext()
        {
            Mocks = new Mocks();
        }

        /// <summary>
        /// Copies all properties of the validation context down to 1 level
        /// </summary>
        /// <returns></returns>
        public ValidationContext Copy()
        {
            return new ValidationContext { Mocks = new Utilities.Mocks(Mocks) };
        }
    }
}
