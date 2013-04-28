using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

using BackToFront.Utilities;
using BackToFront.Expressions;
using BackToFront.Extensions.IEnumerable;
using BackToFront.Logic;
using BackToFront.Framework.Base;
using BackToFront.Validation;
using BackToFront.Meta;
using BackToFront.Enum;
using System.Runtime.Serialization;
using BackToFront.Dependency;
using BackToFront.Expressions.Visitors;

namespace BackToFront.Framework
{
    public class RuleCollection<TEntity> : IValidate, IValidate<TEntity>
    {
        private readonly List<IValidate<TEntity>> _Rules = new List<IValidate<TEntity>>();
        public IEnumerable<IValidate<TEntity>> Rules
        {
            get
            {
                // TODO: cache
                return _Rules.ToArray();
            }
        }

        public IViolation ValidateEntity(object subject, SwapPropVisitor visitor)
        {
            // TODO: catch cast exception/change to as
            var sub = (TEntity)subject;
            return ValidateEntity(sub, new ValidationContext { ExpressionModifier = visitor });
        }

        public IEnumerable<IViolation> FullyValidateEntity(object subject, SwapPropVisitor visitor)
        {
            // TODO: catch cast exception/change to as
            var sub = (TEntity)subject;
            IList<IViolation> violationList = new List<IViolation>();
            FullyValidateEntity(sub, violationList, new ValidationContext { ExpressionModifier = visitor });
            return violationList.ToArray();
        }

        public void AddRule(IValidate<TEntity> rule)
        {
            _Rules.Add(rule);
        }

        public Action<TEntity, ValidationContextX> NewCompile(SwapPropVisitor visitor)
        {
            var compiled = Rules.Select(r => r.NewCompile(visitor));

            return (a, b) => compiled.Each(c => c(a, b));
        }

        public IViolation ValidateEntity(TEntity subject, ValidationContext context)
        {
            IViolation violation;
            foreach (var rule in _Rules)
                if ((violation = rule.ValidateEntity(subject, context.Copy())) != null)
                    return violation;

            return null;
        }

        public void FullyValidateEntity(TEntity subject, IList<IViolation> violationList, ValidationContext context)
        {
            _Rules.Each(r => r.FullyValidateEntity(subject, violationList, context.Copy()));
        }

        public IEnumerable<AffectedMembers> AffectedMembers
        {
            get { return _Rules.Select(r => r.AffectedMembers).Aggregate(); }
        }

        public bool PropertyRequirement
        {
            get { return false; }
        }

        private PathElementMeta _Meta;
        public PathElementMeta Meta
        {
            get
            {
                return _Meta ?? (_Meta = new PathElementMeta(Rules.Select(a => a.Meta), null, PathElementType.RuleCollection));
            }
        }
    }
}
