using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BackToFront
{
    /// <summary>
    /// Represents a business rule violation.
    /// </summary>
    public interface IViolation
    {
        /// <summary>
        /// The message to display to a user
        /// </summary>
        string UserMessage { get; }
    }
}
