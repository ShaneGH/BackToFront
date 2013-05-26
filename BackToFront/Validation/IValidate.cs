﻿using BackToFront.Framework.Base;
using BackToFront.Utilities;
using System.Collections.Generic;
using BackToFront.Meta;
using BackToFront.Dependency;
using BackToFront.Expressions.Visitors;
using System;
using BackToFront.Framework;
using System.Linq.Expressions;

namespace BackToFront.Validation
{

    /// <summary>
    /// Specifies a rule element that can be validated
    /// </summary>
    /// <typeparam name="TEntity">Type of the entity to validate</typeparam>
    public interface IValidate<TEntity>
    {
        IEnumerable<MemberChainItem> ValidationSubjects { get; }

        IEnumerable<MemberChainItem> RequiredForValidation { get; }

        /// <summary>
        /// Gets the function of this element
        /// </summary>
        bool PropertyRequirement { get; }

        Expression Compile(SwapPropVisitor visitor);
    }
}
