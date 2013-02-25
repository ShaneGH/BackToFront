using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

using BackToFront.Extensions.IEnumerable;
using BackToFront.Logic;

namespace BackToFront.Framework
{
    internal class RuleCollection<TEntity> : IRuleCollection<TEntity>, IValidate
    {
        private readonly List<Rule<TEntity>> _Rules = new List<Rule<TEntity>>();
        private bool? _hasValidChain = null;
        bool HasValidChain
        {
            get
            {
                return _Rules.All(a => a.HasValidChain);
            }
        }

        public IOperators<TEntity> If(Expression<Func<TEntity, object>> property)
        {
            var rule = new Rule<TEntity>();
            _Rules.Add(rule);
            return rule.ElseIf(property);
        }

        public IViolation ValidateEntity(object subject)
        {
            IViolation violation;
            // TODO: catch cast exception
            foreach (var rule in _Rules)
            {
                if ((violation = rule.ValidateEntity((TEntity)subject)) != null)
                    return violation;
            }

            return null;
        }

        public IEnumerable<IViolation> FullyValidateEntity(object subject)
        {
            // TODO: catch cast exception
            IList<IViolation> violationList = new List<IViolation>();
            _Rules.Each(i => i.FullyValidateEntity((TEntity)subject, violationList));
            return violationList.ToArray();
        }
    }
}
