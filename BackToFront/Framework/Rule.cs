using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

using BackToFront.Extensions.IEnumerable;
using BackToFront.Framework.Base;
using BackToFront.Framework.Condition;
using BackToFront.Logic;

namespace BackToFront.Framework
{
    internal class Rule<TEntity> : IRule<TEntity>, IValidate<TEntity>
    {
        private readonly HashSet<PropertyElement<TEntity>> _registeredElements = new HashSet<PropertyElement<TEntity>>();

        //TODO: cache?
        public bool HasValidChain
        {
            get
            {
                return _registeredElements.All(a => a.HasValidChain);
            }
        }

        public Rule()
        { }

        private readonly List<Operators<TEntity>> _If = new List<Operators<TEntity>>();
        public IOperators<TEntity> ElseIf(Expression<Func<TEntity, object>> property)
        {
            var @if = new Operators<TEntity>(property, this);
            _If.Add(@if);
            return @if;
        }

        public IViolation ValidateEntity(TEntity subject)
        {
            IViolation violation = null;
            _If.Any(a => a.ValidateIfCondition(subject, out violation));
            return violation;
        }

        public void FullyValidateEntity(TEntity subject, IList<IViolation> violationList)
        {
            _If.Any(i => i.FullyValidateIfCondition(subject, violationList));
        }

        public Logic.Compilations.IConditionSatisfied<TEntity> Else
        {
            get { return ElseIf(a => true).IsTrue(); }
        }

        public void Register(PropertyElement<TEntity> element)
        {
            _registeredElements.Add(element);
        }
    }
}
