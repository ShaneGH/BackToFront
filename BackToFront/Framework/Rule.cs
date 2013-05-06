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
    public class Rule<TEntity> : PathElement<TEntity>, IAdditionalRuleCondition<TEntity>, IRule<TEntity>, IRuleValidation<TEntity>, INonGenericRule
    {
        private static readonly Type _Type = typeof(TEntity);
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

        protected override Expression _NewCompile(SwapPropVisitor visitor)
        {
            var next = AllPossiblePaths.SingleOrDefault(a => a != null);
            if (next != null)
            {
                return next.NewCompile(visitor);
            }
            else
                return Expression.Empty();
        }

        Action<object, ValidationContext> INonGenericRule.NewCompile(SwapPropVisitor visitor)
        {
            var tyt = NewCompile(visitor);
            var rule = Expression.Lambda<Action<TEntity, ValidationContext>>(tyt, visitor.EntityParameter, visitor.ContextParameter).Compile();

            return (a, b) =>
            {
                if (!(a is TEntity))
                    throw new InvalidOperationException("##");

                // TODO: as
                rule((TEntity)a, b);
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

            return _Condition.Add(property);
        }


        public IConditionSatisfied<TEntity> Else
        {
            get { return ElseIf(a => true); }
        }

        #endregion

        #region Validate

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

        public Type RuleType
        {
            get { return _Type; }
        }
    }
}