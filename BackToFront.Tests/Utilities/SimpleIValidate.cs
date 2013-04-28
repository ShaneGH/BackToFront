using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BackToFront.Framework.Base;
using BackToFront.Meta;
using BackToFront.Utilities;

namespace BackToFront.Tests.Utilities
{
    public class SimpleIValidate : PathElement<object>
    {
        public IViolation Violation;

        public SimpleIValidate()
            : base(null) { }

        public override IEnumerable<AffectedMembers> AffectedMembers
        {
            get { throw new NotImplementedException(); }
        }

        public override bool PropertyRequirement
        {
            get { throw new NotImplementedException(); }
        }

        public override PathElementMeta Meta
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

        protected override System.Linq.Expressions.Expression _NewCompile(Expressions.Visitors.SwapPropVisitor visitor, System.Linq.Expressions.ParameterExpression entity, System.Linq.Expressions.ParameterExpression context)
        {
            throw new NotImplementedException();
        }
    }
}
