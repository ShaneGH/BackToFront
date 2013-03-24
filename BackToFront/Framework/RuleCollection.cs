using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

using BackToFront.Utils;
using BackToFront.Expressions;
using BackToFront.Extensions.IEnumerable;
using BackToFront.Logic;

namespace BackToFront.Framework
{
    public class RuleCollection<TEntity> : IValidate
    {
        private readonly List<IValidate<TEntity>> _Rules = new List<IValidate<TEntity>>();

        public IViolation ValidateEntity(object subject, Mocks mocks)
        {
            IViolation violation;
            // TODO: catch cast exception
            var sub = (TEntity)subject;
            foreach (var rule in _Rules)
            {
                if ((violation = rule.ValidateEntity(sub, mocks)) != null)
                    return violation;
            }

            return null;
        }

        public IEnumerable<IViolation> FullyValidateEntity(object subject, Mocks mocks)
        {
            // TODO: catch cast exception
            var sub = (TEntity)subject;
            IList<IViolation> violationList = new List<IViolation>();
            _Rules.Each(i => i.FullyValidateEntity(sub, violationList, mocks));
            return violationList.ToArray();
        }

        public void AddRule(IValidate<TEntity> rule)
        {
            _Rules.Add(rule);
        }
    }
}
