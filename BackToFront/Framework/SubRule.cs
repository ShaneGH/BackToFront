using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

using BackToFront.Logic;
using BackToFront.Framework.Base;
using BackToFront.Framework.Condition;
using BackToFront.Framework.Requirement;

namespace BackToFront.Framework
{
    internal class SubRule<TEntity> : PathElement<TEntity>, ISubRule<TEntity>
    {
        private readonly List<PathElement<TEntity>> _next = new List<PathElement<TEntity>>();

        public SubRule(Rule<TEntity> rule)
            : base(PathElement<TEntity>.IgnorePointer, rule)
        {
        }

        /// <summary>
        /// Sub rule is last element in chain
        /// </summary>
        protected override IEnumerable<PathElement<TEntity>> NextPathElements
        {
            get { yield break; }
        }

        public override void ValidateEntity(TEntity subject, out IViolation violation)
        {
            //TODO: Threadding??
            violation = null;
            foreach (var next in _next)
            {
                next.ValidateEntity(subject, out violation);
                if (violation != null)
                {
                    break;
                }
            }
        }

        public override void FullyValidateEntity(TEntity subject, IList<IViolation> violationList)
        {
            //TODO: Threadding??
            _next.ForEach(a => a.FullyValidateEntity(subject, violationList));
        }

        public IOperators<TEntity> If(Expression<Func<TEntity, object>> property)
        {
            var op = new Operators<TEntity>(property, ParentRule);
            _next.Add(op);
            return op;
        }

        public IRequireOperators<TEntity> RequireThat(Expression<Func<TEntity, object>> property)
        {
            var op = new RequireOperators<TEntity>(property, ParentRule);
            _next.Add(op);
            return op;
        }
    }
}
