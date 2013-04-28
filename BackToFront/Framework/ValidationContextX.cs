using BackToFront.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BackToFront.Framework
{
    public class ValidationContextX
    {
        public readonly bool BreakOnFirstError;
        public readonly IList<IViolation> Violations = new List<IViolation>();

        public ValidationContextX(bool breakOnFirstError)
        {
            BreakOnFirstError = breakOnFirstError;
        }

        public bool IsViolated
        {
            get
            {
                return Violations.Any();
            }
        }

        public bool Break
        {
            get
            {
                return BreakOnFirstError && IsViolated;
            }
        }
    }
}
