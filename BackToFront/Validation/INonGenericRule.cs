﻿using BackToFront.Dependency;
using BackToFront.Expressions.Visitors;
using BackToFront.Framework;
using BackToFront.Framework.Base;
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

        Meta.PathElementMeta Meta { get; }
        
        Action<object, ValidationContext> NewCompile(SwapPropVisitor visitor);
    }
}
