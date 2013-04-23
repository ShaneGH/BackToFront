using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using BackToFront.Enum;
using BackToFront.Expressions;
using BackToFront.Framework.Base;
using BackToFront.Framework.Meta;
using BackToFront.Logic;
using BackToFront.Logic.Compilations;
using BackToFront.Utilities;

namespace BackToFront.Framework
{
    public class RequireOperator<TEntity> : ExpressionElement<TEntity, bool>, IRequirementFailed<TEntity>
    {
        public RequireOperator(Expression<Func<TEntity, bool>> descriptor, Rule<TEntity> rule)
            : base(descriptor, rule)
        {
        }

        public override IEnumerable<PathElement<TEntity>> NextPathElements(TEntity subject, ValidationContext context)
        {
            return NextPathElements();
        }

        public IEnumerable<PathElement<TEntity>> NextPathElements()
        {
            yield return _Then;
            yield return _RequireThat;
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

        private MetaData _Meta;
        public override IMetaElement Meta
        {
            get { return _Meta ?? (_Meta = new MetaData(this)); }
        }

        private class MetaData : IMetaElement
        {
            private readonly RequireOperator<TEntity> _Owner;

            public MetaData(RequireOperator<TEntity> owner)
            {
                _Owner = owner;
            }

            public IEnumerable<IMetaElement> Children
            {
                get
                {
                    return _Owner.NextPathElements().Where(a => a != null).Select(a => a.Meta);
                }
            }

            public ExpressionWrapperBase Code
            {
                get { return _Owner.Descriptor; }
            }

            public PathElementType Type
            {
                get { return PathElementType.RequireOperator; }
            }
        }
    }
}
