using BackToFront.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.Serialization;

namespace BackToFront.Framework
{
    public interface IValidationContext
    {
        [DataMember]
        bool BreakOnFirstError { get; }

        [DataMember]
        IList<IViolation> Violations { get; }

        [DataMember]
        object[] Mocks { get; }

        [DataMember]
        IDictionary<string, object> Dependencies { get; }
    }

    public class ValidationContext : IValidationContext
    {
        public readonly bool BreakOnFirstError;
        public readonly IList<IViolation> Violations = new List<IViolation>();
        public readonly object[] Mocks;
        public readonly IDictionary<string, object> Dependencies;

        public ValidationContext(bool breakOnFirstError, object[] mocks, IDictionary<string, object> dependencies)
        {
            BreakOnFirstError = breakOnFirstError;
            Mocks = mocks ?? new object[0];
            Dependencies = dependencies ?? new Dictionary<string, object>();
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

        bool IValidationContext.BreakOnFirstError
        {
            get { return BreakOnFirstError; }
        }

        IList<IViolation> IValidationContext.Violations
        {
            get { return Violations; }
        }

        object[] IValidationContext.Mocks
        {
            get { return Mocks; }
        }

        IDictionary<string, object> IValidationContext.Dependencies
        {
            get { return Dependencies; }
        }
    }
}
