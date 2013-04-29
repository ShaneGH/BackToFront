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
using BackToFront.Logic;
using BackToFront.Logic.Compilations;
using BackToFront.Utilities;
using System.Runtime.Serialization;

namespace BackToFront.Framework
{
    public class RequireOperator<TEntity> : ExpressionElement<TEntity, bool>, IRequirementFailed<TEntity>
    {
        public RequireOperator(Expression<Func<TEntity, bool>> descriptor, Rule<TEntity> rule)
            : base(descriptor, rule)
        {
        }
        
        public override IEnumerable<PathElement<TEntity>> AllPossiblePaths
        {
            get
            {
                yield return _Then;
                yield return _RequireThat;
            }
        }

        private RequirementFailed<TEntity> _RequireThat = null;
        public IModelViolation<TEntity> RequireThat(Expression<Func<TEntity, bool>> condition)
        {
            return Do(() => _RequireThat = new RequirementFailed<TEntity>(condition, ParentRule));
        }

        private SubRuleCollection<TEntity> _Then = null;
        public IAdditionalRuleCondition<TEntity> Then(Action<IRule<TEntity>> action)
        {
            Do(() => _Then = new SubRuleCollection<TEntity>(ParentRule));

            // run action on new sub rule
            action(_Then);

            // present rule to begin process again
            return ParentRule;
        }

        public override bool PropertyRequirement
        {
            get { return true; }
        }

        private PathElementMeta _Meta;
        public override PathElementMeta Meta
        {
            get
            {
                return _Meta ?? (_Meta = new PathElementMeta(AllPossiblePaths.Where(a => a != null).Select(a => a.Meta), Descriptor.Meta, PathElementType.RequireOperator));
            }
        }

        protected override Expression _NewCompile(Expressions.Visitors.SwapPropVisitor visitor)
        {
            var next = AllPossiblePaths.SingleOrDefault(a => a != null);
            if (next != null)
            {
                //using (visitor.WithEntityParameter(EntityParameter))
                //Expression.Lambda<Action<TEntity, ValidationContextX>>(visitor.Visit(Descriptor.WrappedExpression), entity, context).Compile();

                using (visitor.WithEntityParameter(EntityParameter))
                {
                    var des = visitor.Visit(Descriptor.WrappedExpression);
                    var nxt = next.NewCompile(visitor);
                    return Expression.IfThen(des, nxt);
                }
            }
            else
                return Expression.Empty();
        }
    }
}
