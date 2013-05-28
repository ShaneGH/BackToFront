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

        public override IEnumerable<MemberChainItem> ValidationSubjects
        {
            get { yield break; }
        }

        public override IEnumerable<MemberChainItem> RequiredForValidation
        {
            get { yield break; }
        }

        public override bool PropertyRequirement
        {
            get { return false; }
        }

        protected override Expression _Compile(SwapPropVisitor visitor)
        {
            // var violation
            var violation = Expression.Variable(typeof(IViolation), "violation");
            // violationX = _violation(entity);
            var createViolation = Expression.Assign(violation, Expression.Invoke(Expression.Constant(_violation), visitor.EntityParameter));
            // violationX.ViolatedEntity = entity;
            var assignViolatedEntity = Expression.Assign(Expression.PropertyOrField(violation, "ViolatedEntity"), visitor.EntityParameter);
            // violation.Violated = _violatedMembers.ToArray();
            var assignViolated = Expression.Assign(Expression.PropertyOrField(violation, "Violated"), Expression.Call(_ToArray, Expression.Constant(_violatedMembers)));
            // context.Violations.Add(violationX);
            var assignContext = Expression.Call(Expression.PropertyOrField(visitor.ContextParameter, "Violations"), _Add, violation);

            return Expression.Block(new[] { violation }, createViolation, assignViolatedEntity, assignViolated, assignContext);
        }
    }
}
