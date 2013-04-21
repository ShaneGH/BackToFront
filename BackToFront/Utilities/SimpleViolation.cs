using System.Collections.Generic;

namespace BackToFront.Utilities
{
    public class SimpleViolation : IViolation
    {
        private readonly string _UserMessage;

        public SimpleViolation(string userMessage)
        {
            _UserMessage = userMessage;
        }

        public string UserMessage
        {
            get { return _UserMessage; }
        }

        public object ViolatedEntity { get; set; }

        public IEnumerable<MemberChainItem> Violated { get; set; }
    }
}
