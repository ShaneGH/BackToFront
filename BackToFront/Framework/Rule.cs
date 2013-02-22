using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using BackToFront.Extensions.IEnumerable;
using BackToFront.Framework.Base;
using BackToFront.Framework.Condition;
using BackToFront.Logic;

namespace BackToFront.Framework
{
    internal class Rule<TEntity> : PropertyElement<TEntity>, IRule<TEntity>, IValidate, IValidate<TEntity>
    {
        public Rule()
            : base(PathElement<TEntity>.IgnorePointer)
        { }

        private readonly List<Operators<TEntity>> _If = new List<Operators<TEntity>>();
        public IOperators<TEntity> If(Func<TEntity, object> property)
        {
            var @if = new Operators<TEntity>(property, this);
            _If.Add(@if);
            return @if;
        }

        public IViolation ValidateEntity(object subject)
        {
            // TODO: catch cast exception
            return ValidateEntity((TEntity)subject);
        }

        public IEnumerable<IViolation> FullyValidateEntity(object subject)
        {
            // TODO: catch cast exception
            var list = new List<IViolation>();
            FullyValidateEntity((TEntity)subject, list);
            return list.ToArray();
        }

        public IViolation ValidateEntity(TEntity subject)
        {
            foreach (var i in _If)
            {
                var violation = i.ValidateEntity(subject);
                if (violation != null)
                    return violation;
            }

            return null;
        }

        public void FullyValidateEntity(TEntity subject, IList<IViolation> violationList)
        {
            _If.Each(i => i.FullyValidateEntity(subject, violationList));
        }

        /// <summary>
        /// Rule path traversal is not linear, can only be the first element in the chain (and doesn't use NextOption)
        /// </summary>
        public Logic.Base.IPathElement<TEntity> NextOption
        {
            get { return null; }
        }
    }
}
