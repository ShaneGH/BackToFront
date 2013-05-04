using BackToFront.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BackToFront.Framework
{
    public class ValidationContext
    {
        public readonly bool BreakOnFirstError;
        public readonly IList<IViolation> Violations = new List<IViolation>();
        public readonly object[] Mocks;
        public readonly IDictionary<string, object> Dependencies;

        public ValidationContext(bool breakOnFirstError, object[] mocks, IDictionary<string, object> dependencies)
        {
            BreakOnFirstError = breakOnFirstError;
            Mocks = mocks;
            Dependencies = dependencies;
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
