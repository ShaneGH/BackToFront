using BackToFront.Dependency;
using BackToFront.Expressions.Visitors;
using BackToFront.Framework;
using BackToFront.Framework.Base;
using BackToFront.Meta;
using System;
using System.Collections.Generic;

namespace BackToFront.Validation
{
    public interface INonGenericRule
    {
        Type RuleType { get; }

        IEnumerable<AffectedMembers> AffectedMembers { get; }

        List<DependencyWrapper> Dependencies { get; }

        bool PropertyRequirement { get; }
        
        Action<object, ValidationContext> Compile(SwapPropVisitor visitor);

        ExpressionMeta Meta { get; }
    }
}
