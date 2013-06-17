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
    public interface IPreCompiledRule
    {
        RuleMeta Meta { get; }
        Expression Descriptor { get; }
        Action<object, ValidationContext> Worker { get; }
        ParameterExpression Entity { get; }
        ParameterExpression Context { get; }
    }

    public interface INonGenericRule
    {
        Type RuleType { get; }

        IEnumerable<MemberChainItem> ValidationSubjects { get; }
        IEnumerable<MemberChainItem> RequiredForValidation { get; }

        List<DependencyWrapper> Dependencies { get; }
        
        Action<object, ValidationContext> Compile(SwapPropVisitor visitor);

        IPreCompiledRule Meta { get; }
    }
}
