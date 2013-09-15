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

        public override IEnumerable<PathElement<object>> AllPossiblePaths
        {
            get
            {
                yield break;
            }
        }

        protected override System.Linq.Expressions.Expression _Compile(Expressions.Visitors.ExpressionMocker visitor)
        {
            throw new NotImplementedException();
        }

        public override IEnumerable<BackToFront.Utilities.MemberChainItem> ValidationSubjects
        {
            get { throw new NotImplementedException(); }
        }

        public override IEnumerable<BackToFront.Utilities.MemberChainItem> RequiredForValidation
        {
            get { throw new NotImplementedException(); }
        }
    }
}
