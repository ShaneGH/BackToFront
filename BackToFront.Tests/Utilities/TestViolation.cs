using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;
using System.Threading.Tasks;

using BackToFront.Logic;
using BackToFront.Utilities;

namespace BackToFront.Tests.Utilities
{
    [DebuggerDisplay("{UserMessage}")]
    public class TestViolation : IViolation
    {
        private readonly string _UserMessage;
        public TestViolation()
            : this("error")
        {
        }

        public TestViolation(string userMessage)
        {
            _UserMessage = userMessage;
        }

        public string UserMessage
        {
            get { return _UserMessage; }
        }

        public override string ToString()
        {
            return UserMessage;
        }

        public object ViolatedEntity
        {
            get;
            set;
        }

        public IEnumerable<MemberChainItem> Violated
        {
            get;
            set;
        }
    }
}