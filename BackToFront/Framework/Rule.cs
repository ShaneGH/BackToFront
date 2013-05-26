using BackToFront.Dependency;
using BackToFront.Enum;
using BackToFront.Expressions.Visitors;
using BackToFront.Extensions.IEnumerable;
using BackToFront.Framework.Base;
using BackToFront.Logic;
using BackToFront.Logic.Compilations;
using BackToFront.Meta;
using BackToFront.Validation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using BackToFront.Expressions;
using BackToFront.Utilities;

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

        protected override Expression _Compile(SwapPropVisitor visitor)
        {
            PathElement<TEntity> next;
            if ((next = AllPossiblePaths.SingleOrDefault(a => a != null)) != null)
                return next.Compile(visitor);
            else
                return Expression.Empty();
        }

        Action<object, ValidationContext> INonGenericRule.Compile(SwapPropVisitor visitor)
        {
            return Compile(visitor, Compile(visitor));
        }

        private Action<object, ValidationContext> Compile(SwapPropVisitor visitor, Expression logic)
        {
            var rule = Expression.Lambda<Action<TEntity, ValidationContext>>(logic, visitor.EntityParameter, visitor.ContextParameter).Compile();

            return (entity, validationContext) =>
            {
                if (!(entity is TEntity))
                    throw new InvalidOperationException("##");

                // TODO: as
                rule((TEntity)entity, validationContext);
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
        
        private Expression _PreCompiled;
        public Expression PreCompiled
        {
            get
            {
                return _PreCompiled ?? (_PreCompiled = _Compile(new SwapPropVisitor(typeof(TEntity))));
            }
        }

        #endregion

        #region Meta

        private IPreCompiledRule _Meta;
        public IPreCompiledRule Meta
        {
            get 
            {
                return _Meta ?? (_Meta = new PreCompiledRule(this));
            }
        }

        public override IEnumerable<MemberChainItem> ValidationSubjects
        {
            // TODO: cache???
            get { return RegisteredItems.Select(a => a.ValidationSubjects).Aggregate(); }
        }

        public override IEnumerable<MemberChainItem> RequiredForValidation
        {
            // TODO: cache???
            get { return RegisteredItems.Select(a => a.RequiredForValidation).Aggregate(); }
        }

        public List<DependencyWrapper> Dependencies
        {
            get { return _Dependencies; }
        }

        public override bool PropertyRequirement
        {
            get { return false; }
        }

        public Type RuleType
        {
            get { return _Type; }
        }

        #endregion

        /// <summary>
        /// TODO: this is a bad class, no error checking. Maybe have as private
        /// </summary>
        public class PreCompiledRule : IPreCompiledRule
        {
            public readonly RuleMeta Meta;
            public readonly Expression Descriptor;
            public readonly Action<object, ValidationContext> Worker;
            public readonly ParameterExpression Entity;
            public readonly ParameterExpression Context;

            public PreCompiledRule(Rule<TEntity> rule)
            {
                var visitor = new SwapPropVisitor(typeof(TEntity));

                Meta = new RuleMeta(rule);
                Descriptor = rule.Compile(visitor);
                Worker = rule.Compile(visitor, Descriptor);
                Entity = visitor.EntityParameter;
                Context = visitor.ContextParameter;
            }

            RuleMeta IPreCompiledRule.Meta
            {
                get { return Meta; }
            }

            Expression IPreCompiledRule.Descriptor
            {
                get { return Descriptor; }
            }

            Action<object, ValidationContext> IPreCompiledRule.Worker
            {
                get { return Worker; }
            }

            ParameterExpression IPreCompiledRule.Entity
            {
                get { return Entity; }
            }

            ParameterExpression IPreCompiledRule.Context
            {
                get { return Context; }
            }
        }

    }
}