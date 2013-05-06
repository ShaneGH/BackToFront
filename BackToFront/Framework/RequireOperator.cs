using System;
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
using BackToFront.Extensions.IEnumerable;

namespace BackToFront.Framework
{
    public class RequireOperator<TEntity> : PathElement<TEntity>, IConditionSatisfied<TEntity>
    {
        public RequireOperator(Rule<TEntity> rule)
            : base(rule)
        {
        }
        
        public override IEnumerable<PathElement<TEntity>> AllPossiblePaths
        {
            get
            {
                yield return _Then;
                yield return _RequireThat;
                yield return _RequirementFailed;
            }
        }

        private RequirementFailed<TEntity> _RequirementFailed;
        public IModelViolation<TEntity> RequirementFailed
        {
            get
            {
                return Do(() => _RequirementFailed = new RequirementFailed<TEntity>(a => false, ParentRule));
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
                return _Meta;// ?? (_Meta = new PathElementMeta(AllPossiblePaths.Where(a => a != null).Select(a => a.Meta), Descriptor.Meta, PathElementType.RequireOperator));
            }
        }

        protected override Expression _Compile(Expressions.Visitors.SwapPropVisitor visitor)
        {
            var next = AllPossiblePaths.SingleOrDefault(a => a != null);
            return next != null ? next.Compile(visitor) : Expression.Empty();
        }

        public override IEnumerable<AffectedMembers> AffectedMembers
        {
            get { yield break; }
        }
    }
}
