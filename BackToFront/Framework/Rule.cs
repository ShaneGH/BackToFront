using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using BackToFront.Extensions;
using BackToFront.Framework.Base;
using BackToFront.Logic;

namespace BackToFront.Framework
{
    internal class Rule<TEntity> : PropertyElement<TEntity>, IRule<TEntity>, IValidate, IValidate<TEntity>
    {
        public Rule()
            : base(PathElement<TEntity>.IgnorePointer)
        { }

        private readonly List<If<TEntity>> _If = new List<If<TEntity>>();
        public IOperators<TEntity> If(Func<TEntity, object> property)
        {
            var @if = new If<TEntity>(property, this);
            _If.Add(@if);
            return @if;
        }

        public IViolation Validate(object subject)
        {
            // TODO: catch cast exception
            return Validate((TEntity)subject);
        }

        public IEnumerable<IViolation> ValidateAll(object subject)
        {
            // TODO: catch cast exception
            var list = new List<IViolation>();
            ValidateAll((TEntity)subject, list);
            return list.ToArray();
        }

        public IViolation Validate(TEntity subject)
        {
            foreach (var i in _If)
            {
                var violation = i.Validate(subject);
                if (violation != null)
                    return violation;
            }

            return null;
        }

        public void ValidateAll(TEntity subject, IList<IViolation> violationList)
        {
            _If.Each(i => i.ValidateAll(subject, violationList));
        }

        /// <summary>
        /// Rule path traversal is not linear, can only be the first element in the chain (and doesn't use NextOption)
        /// </summary>
        public Logic.Base.IPathElement NextOption
        {
            get { return null; }
        }
    }
}
