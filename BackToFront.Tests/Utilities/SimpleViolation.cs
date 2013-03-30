using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;
using System.Threading.Tasks;

using BackToFront.Logic;

namespace BackToFront.Tests.Utilities
{
    [DebuggerDisplay("{UserMessage}")]
    public class SimpleViolation : IViolation
    {
        private readonly string _UserMessage;
        public SimpleViolation()
            : this("error")
        {
        }

        public SimpleViolation(string userMessage)
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
    }
}