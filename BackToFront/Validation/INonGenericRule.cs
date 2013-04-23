using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BackToFront.Dependency;
using BackToFront.Framework.Base;

namespace BackToFront.Validation
{
    public interface INonGenericRule : IValidate
    {
        IEnumerable<AffectedMembers> AffectedMembers { get; }

        List<DependencyWrapper> Dependencies { get; }

        bool PropertyRequirement { get; }
    }
}
