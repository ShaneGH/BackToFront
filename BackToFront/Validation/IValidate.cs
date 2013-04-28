﻿using BackToFront.Framework.Base;
using BackToFront.Utilities;
using System.Collections.Generic;
using BackToFront.Meta;
using BackToFront.Dependency;
using BackToFront.Expressions.Visitors;
using System;
using BackToFront.Framework;

namespace BackToFront.Validation
{

    /// <summary>
    /// Specifies a rule element that can be validated
    /// </summary>
    /// <typeparam name="TEntity">Type of the entity to validate</typeparam>
    public interface IValidate<TEntity>
    {
        /// <summary>
        /// All of the members which are touched by this object and its children
        /// </summary>
        IEnumerable<AffectedMembers> AffectedMembers { get; }

        /// <summary>
        /// Gets the function of this element
        /// </summary>
        bool PropertyRequirement { get; }

        /// <summary>
        /// Metadata on the element to validate
        /// </summary>
        PathElementMeta Meta { get; }


        Action<TEntity, ValidationContextX> NewCompile(SwapPropVisitor visitor);
    }
}
