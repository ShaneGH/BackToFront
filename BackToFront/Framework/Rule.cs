using BackToFront.Dependency;
using BackToFront.Extensions.IEnumerable;
using BackToFront.Framework.Base;
using BackToFront.Logic;
using BackToFront.Logic.Compilations;
using BackToFront.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using BackToFront.Validation;
using BackToFront.Meta;
using BackToFront.Expressions;
using BackToFront.Enum;
using System.Runtime.Serialization;
using BackToFront.Expressions.Visitors;

namespace BackToFront.Framework
{
    public class Rule<TEntity> : PathElement<TEntity>, IAdditionalRuleCondition<TEntity>, IRule<TEntity>, IValidate<TEntity>, IValidate, IRuleValidation<TEntity>, INonGenericRule 
    {
        private readonly HashSet<IValidate<TEntity>> RegisteredItems = new HashSet<IValidate<TEntity>>();
        public readonly List<DependencyWrapper> _Dependencies = new List<DependencyWrapper>();
        private readonly HashSet<Rule<TEntity>> SubRules = new HashSet<Rule<TEntity>>();

        public Rule()
            : this(null)
        { }

        public Rule(Rule<TEntity> parentRule)
            : base(parentRule)
        {
            if (ParentRule != null)
                ParentRule.SubRules.Add(this);
        }

        protected override Action<TEntity, ValidationContextX> _NewCompile(SwapPropVisitor visitor)
        {
            var next = AllPossiblePaths.SingleOrDefault(a => a != null);
            if (next != null)
            {
                var r = next.NewCompile(visitor);
                return (a, b) => r(a, b);
            }
            else
                return DoNothing;
        }

        Action<object, ValidationContextX> INonGenericRule.NewCompile(SwapPropVisitor visitor)
        {
            var tmp = base.NewCompile(visitor);
            return (a, b) => 
            {
                if (!(a is TEntity))
                    throw new InvalidOperationException("##");

                // TODO: as
                tmp((TEntity)a, b);
            };
        }

        #region hierarchy

        public IEnumerable<Rule<TEntity>> AllAncestorRules
        {
            get
            {
                return SubRules.Select(sr => new[] { sr }.Concat(sr.AllAncestorRules)).Aggregate();
            }
        }

        public override IEnumerable<PathElement<TEntity>> NextPathElements(TEntity subject, ValidationContext context)
        {
            return AllPossiblePaths;
        }
        
        public override IEnumerable<PathElement<TEntity>> AllPossiblePaths
        {
            get
            {
                yield return _Condition;
                yield return _RequireThat;
            }
        }

        public void Register(IValidate<TEntity> element)
        {
            RegisteredItems.Add(element);
        }

        #endregion

        #region ruleElements

        private RequirementFailed<TEntity> _RequireThat;
        public IModelViolation<TEntity> RequireThat(Expression<Func<TEntity, bool>> property)
        {
            return Do(() => _RequireThat = new RequirementFailed<TEntity>(property, this));
        }

        MultiCondition<TEntity> _Condition;
        public IConditionSatisfied<TEntity> If(Expression<Func<TEntity, bool>> property)
        {
            return Do(() =>
            {
                _Condition = new MultiCondition<TEntity>(this);
                return ElseIf(property);
            });
        }

        public IConditionSatisfied<TEntity> ElseIf(Expression<Func<TEntity, bool>> property)
        {
            if (_Condition == null)
                return If(property);

            var @if = new Operator<TEntity>(property, this);
            _Condition.If.Add(@if);
            return @if;
        }

        public IConditionSatisfied<TEntity> Else
        {
            get { return ElseIf(a => true); }
        }

        #endregion 

        #region Validate

        IViolation IValidate.ValidateEntity(object subject, SwapPropVisitor visitor)
        {
            if (subject is TEntity)
            {
                //TODO: change cast to as
                return ValidateEntity((TEntity)subject, new ValidationContext { ExpressionModifier = visitor });
            }

            throw new InvalidOperationException("##");
        }

        IEnumerable<IViolation> IValidate.FullyValidateEntity(object subject, SwapPropVisitor visitor)
        {
            if (subject is TEntity)
            {
                List<IViolation> violations = new List<IViolation>();
                //TODO: change cast to as
                FullyValidateEntity((TEntity)subject, violations, new ValidationContext { ExpressionModifier = visitor });
                return violations.ToArray();
            }

            throw new InvalidOperationException("##");
        }

        #endregion

        #region Meta

        public override IEnumerable<AffectedMembers> AffectedMembers
        {
            // TODO: cache???
            get { return RegisteredItems.Select(a => a.AffectedMembers).Aggregate(); }
        }

        public List<DependencyWrapper> Dependencies
        {
            get { return _Dependencies; }
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
                return _Meta ?? (_Meta = new PathElementMeta(AllPossiblePaths.Where(a => a != null).Select(a => a.Meta), null, PathElementType.Rule));
            }
        }

        #endregion
    }
}
