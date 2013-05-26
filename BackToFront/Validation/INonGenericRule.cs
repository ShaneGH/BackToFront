using BackToFront.Dependency;
using BackToFront.Expressions.Visitors;
using BackToFront.Framework;
using BackToFront.Framework.Base;
using BackToFront.Meta;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using BackToFront.Utilities;

namespace BackToFront.Validation
{
    public interface INonGenericRule
    {
        Type RuleType { get; }

        IEnumerable<MemberChainItem> ValidatableMembers { get; }
        IEnumerable<MemberChainItem> RequiredForValidationMembers { get; }

        List<DependencyWrapper> Dependencies { get; }

        bool PropertyRequirement { get; }
        
        Action<object, ValidationContext> Compile(SwapPropVisitor visitor);

        RuleMeta Meta { get; }

        Expression PreCompiled { get; }
    }
}
