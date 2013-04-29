using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using BackToFront.Enum;
using BackToFront.Expressions;
using BackToFront.Framework.Base;
using BackToFront.Meta;
using BackToFront.Utilities;
using System.Runtime.Serialization;
using BackToFront.Expressions.Visitors;
using BackToFront.Extensions.IEnumerable;

namespace BackToFront.Framework
{
    /// <summary>
    /// End of a pathway, Throw violation
    /// </summary>
    /// <typeparam name="TEntity"></typeparam>
    public class ThrowViolation<TEntity> : PathElement<TEntity>
    {
        private readonly IEnumerable<MemberChainItem> _violatedMembers;
        private readonly Func<TEntity, IViolation> _violation;
        //TODO: pass in affected members and pass to copile method
        public ThrowViolation(Func<TEntity, IViolation> violation, Rule<TEntity> parentRule, IEnumerable<MemberChainItem> violatedMembers)
            : base(parentRule)
        {
            if (violation == null)
                throw new ArgumentNullException("##6");

            _violation = violation;
            _violatedMembers = (violatedMembers ?? Enumerable.Empty<MemberChainItem>()).ToArray();
        }

        public override IEnumerable<PathElement<TEntity>> AllPossiblePaths
        {
            get
            {
                yield break;
            }
        }

        public override IEnumerable<AffectedMembers> AffectedMembers
        {
            get
            {
                yield break;
            }
        }

        public override bool PropertyRequirement
        {
            get { return false; }
        }

        #region Meta

        private PathElementMeta _Meta;
        public override PathElementMeta Meta
        {
            get
            {
                return _Meta ?? (_Meta = new PathElementMeta(AllPossiblePaths.Where(a => a != null).Select(a => a.Meta), null, PathElementType.ThrowViolation));
            }
        }

        #endregion

        protected override Expression _NewCompile(SwapPropVisitor visitor)
        {
            //var add = typeof(IList<IViolation>).GetMethod("Add", new[] { typeof(IViolation) });
            //var toArray = typeof(Enumerable).GetMethod("ToArray", System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.Public).MakeGenericMethod(typeof(MemberChainItem));
            
            //var violation = Expression.Variable(typeof(IViolation));
            //var getViolation = Expression.Invoke(Expression.Constant(_violation), entity);
            //var getViolatedEntity = Expression.Property(violation, "ViolatedEntity");
            //var getViolated = Expression.Property(violation, "Violated");
            //var enumerateViolatedMembers = Expression.Call(null, toArray, Expression.Constant(_violatedMembers));
            //var getViolations = Expression.Property(context, "Violations");
            
            //var setViolation = Expression.Assign(violation, getViolation);
            //var setEntity = Expression.Assign(getViolatedEntity, entity);
            //var setViolated = Expression.Assign(getViolated, enumerateViolatedMembers);
            //var addViolation = Expression.Call(getViolations, add, violation);

            //return Expression.Block(
            //    Expression.Assign(violation, getViolation),
            //    Expression.Assign(getViolatedEntity, entity),
            //    Expression.Assign(getViolated, enumerateViolatedMembers),
            //    Expression.Call(getViolations, add, violation));


            Action<TEntity, ValidationContextX> block = (entity, context) =>
            {
                var violation = _violation(entity);
                violation.ViolatedEntity = entity;
                violation.Violated = _violatedMembers.ToArray();
                context.Violations.Add(violation);
            };
            
            return Expression.Invoke(Expression.Constant(block), visitor.EntityParameter, visitor.ContextParameter);
        }
    }
}
