using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using BackToFront.Enum;
using BackToFront.Expressions;
using BackToFront.Extensions.IEnumerable;
using BackToFront.Framework.Base;
using BackToFront.Meta;
using BackToFront.Logic;
using BackToFront.Logic.Compilations;
using BackToFront.Utilities;
using System.Runtime.Serialization;
using BackToFront.Expressions.Visitors;

namespace BackToFront.Framework
{
    public class RequirementFailed<TEntity> : ExpressionElement<TEntity, bool>, IModelViolation<TEntity>
    {
        public override IEnumerable<PathElement<TEntity>> AllPossiblePaths
        {
            get
            {
                yield return Violation;
            }
        }

        public RequirementFailed(Expression<Func<TEntity, bool>> property, Rule<TEntity> rule)
            : base(property, rule)
        {
        }

        protected ThrowViolation<TEntity> Violation;
        public IAdditionalRuleCondition<TEntity> WithModelViolation(Func<IViolation> violation)
        {
            return WithModelViolation(a => violation());
        }

        public IAdditionalRuleCondition<TEntity> WithModelViolation(string violation)
        {
            return WithModelViolation(a => new SimpleViolation(violation));
        }

        public IAdditionalRuleCondition<TEntity> WithModelViolation(Func<TEntity, IViolation> violation)
        {
            Do(() => { Violation = new ThrowViolation<TEntity>(violation, ParentRule, AffectedMembers.Select(a => a.Member)); });
            return ParentRule;
        }

        public override bool PropertyRequirement
        {
            get { return false; }
        }

        private PathElementMeta _Meta;
        public override PathElementMeta Meta
        {
            get
            {
                return _Meta ?? (_Meta = new PathElementMeta(AllPossiblePaths.Where(a => a != null).Select(a => a.Meta), Descriptor.Meta, PathElementType.RequirementFailed));
            }
        }

        protected override Expression _NewCompile(SwapPropVisitor visitor)
        {
            var next = AllPossiblePaths.SingleOrDefault(a => a != null);            
            if (next != null)
            {
                using (visitor.WithEntityParameter(EntityParameter))
                {
                    var des = visitor.Visit(Descriptor.WrappedExpression);
                    var nxt = next.NewCompile(visitor);
                    return Expression.IfThen(Expression.Not(des), nxt);
                }
            }

            return Expression.Empty();
        }
    }
}
