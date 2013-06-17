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
using System.Reflection;

namespace BackToFront.Framework
{
    /// <summary>
    /// End of a pathway, Throw violation
    /// </summary>
    /// <typeparam name="TEntity"></typeparam>
    public class ThrowViolation<TEntity> : PathElement<TEntity>
    {
        private static readonly MethodInfo _Add = typeof(ICollection<IViolation>).GetMethod("Add");
        private static readonly MethodInfo _ToArray = typeof(Enumerable).GetMethod("ToArray", System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.Public)
            .MakeGenericMethod(typeof(MemberChainItem));

        private readonly MemberChainItem[] _violatedMembers;
        private readonly Expression<Func<TEntity, IViolation>> _violation;

        public ThrowViolation(Expression<Func<TEntity, IViolation>> violation, Rule<TEntity> parentRule, IEnumerable<MemberChainItem> violatedMembers)
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

        protected override Expression _Compile(SwapPropVisitor visitor)
        {
            Expression createViolationMethod = null;
            using (visitor.WithEntityParameter(_violation.Parameters.First()))
            {
                createViolationMethod = visitor.Visit(_violation.Body);
            }

            // IViolation violation;
            var violation = Expression.Variable(typeof(IViolation), "violation");
            // violation = _violation(entity);
            var createViolation = Expression.Assign(violation, createViolationMethod);
            // violation.ViolatedEntity = entity;
            var assignViolatedEntity = Expression.Assign(Expression.PropertyOrField(violation, "ViolatedEntity"), visitor.EntityParameter);
            // violation.Violated = _violatedMembers;
            var assignViolated = Expression.Assign(Expression.PropertyOrField(violation, "Violated"), Expression.Constant(_violatedMembers));
            // context.Violations.Add(violation);
            var assignContext = Expression.Call(Expression.PropertyOrField(visitor.ContextParameter, "Violations"), _Add, violation);

            return Expression.Block(new[] { violation }, createViolation, assignViolatedEntity, assignViolated, assignContext);
        }
    }
}
