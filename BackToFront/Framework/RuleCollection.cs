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
    internal class RuleCollection<TEntity> : IValidate
    {
        private readonly List<Rule<TEntity>> _Rules = new List<Rule<TEntity>>();

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

        public void AddRule(Rule<TEntity> rule)
        {
            _Rules.Add(rule);
        }
    }
}
