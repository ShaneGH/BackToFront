﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using BackToFront.Enum;
using BackToFront.Expressions;
using BackToFront.Framework.Base;
using BackToFront.Meta;
using BackToFront.Utilities;
using System.Runtime.Serialization;
using BackToFront.Expressions.Visitors;
using BackToFront.Extensions.IEnumerable;

namespace BackToFront.Framework
{
    /// <summary>
    /// End of a pathway, Throw violation
    /// </summary>
    /// <typeparam name="TEntity"></typeparam>
    public class ThrowViolation<TEntity> : PathElement<TEntity>
    {
        private readonly IEnumerable<MemberChainItem> _violatedMembers;
        private readonly Func<TEntity, IViolation> _violation;
        //TODO: pass in affected members and pass to copile method
        public ThrowViolation(Func<TEntity, IViolation> violation, Rule<TEntity> parentRule, IEnumerable<MemberChainItem> violatedMembers)
            : base(parentRule)
        {
            if (violation == null)
                throw new ArgumentNullException("##6");

            _violation = violation;
            _violatedMembers = (violatedMembers ?? Enumerable.Empty<MemberChainItem>()).ToArray();
        }

        public override IEnumerable<PathElement<TEntity>> AllPossiblePaths
        {
            get
            {
                yield break;
            }
        }

        public override IEnumerable<AffectedMembers> AffectedMembers
        {
            get
            {
                yield break;
            }
        }

        public override bool PropertyRequirement
        {
            get { return false; }
        }

        #region Meta

        private PathElementMeta _Meta;
        public override PathElementMeta Meta
        {
            get
            {
                return _Meta ?? (_Meta = new PathElementMeta(AllPossiblePaths.Where(a => a != null).Select(a => a.Meta), null, PathElementType.ThrowViolation));
            }
        }

        #endregion

        protected override Expression _NewCompile(SwapPropVisitor visitor)
        {
            Action<TEntity, ValidationContext> block = (entity, context) =>
            {
                var violation = _violation(entity);
                violation.ViolatedEntity = entity;
                violation.Violated = _violatedMembers.ToArray();
                context.Violations.Add(violation);
            };
            
            return Expression.Invoke(Expression.Constant(block), visitor.EntityParameter, visitor.ContextParameter);
        }
    }
}
