﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BackToFront.Enum
{
    public enum PathElementType
    {
        Rule,
        DeadEnd,
        MultiCondition,
        Condition,
        RequireOperator,
        ThrowViolation,
        RuleCollection,
        RequirementFailed,
        SubRuleCollection
    }
}
