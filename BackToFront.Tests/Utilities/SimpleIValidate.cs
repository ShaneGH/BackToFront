using BackToFront.Framework.Base;
using BackToFront.Meta;
using System;
using System.Collections.Generic;

namespace BackToFront.Tests.Utilities
{
    public class SimpleIValidate : PathElement<object>
    {
        public IViolation Violation;

        public SimpleIValidate()
            : base(null) { }

        public override bool PropertyRequirement
        {
            get { throw new NotImplementedException(); }
        }

        public override IEnumerable<PathElement<object>> AllPossiblePaths
        {
            get
            {
                yield break;
            }
        }

        protected override System.Linq.Expressions.Expression _Compile(Expressions.Visitors.SwapPropVisitor visitor)
        {
            throw new NotImplementedException();
        }

        public override IEnumerable<BackToFront.Utilities.MemberChainItem> ValidatableMembers
        {
            get { throw new NotImplementedException(); }
        }

        public override IEnumerable<BackToFront.Utilities.MemberChainItem> RequiredForValidationMembers
        {
            get { throw new NotImplementedException(); }
        }
    }
}
