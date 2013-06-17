using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using BackToFront.Enum;
using BackToFront.Expressions;
using BackToFront.Extensions.IEnumerable;
using BackToFront.Framework.Base;
using BackToFront.Meta;
using BackToFront.Logic;
using BackToFront.Logic.Compilations;
using BackToFront.Utilities;
using System.Runtime.Serialization;
using BackToFront.Expressions.Visitors;
using System.Reflection;

namespace BackToFront.Framework
{
    public class RequirementFailed<TEntity> : ExpressionElement<TEntity, bool>, IModelViolation<TEntity>
    {
        private static ConstructorInfo _SimpleViolation = typeof(SimpleViolation).GetConstructor(new[] { typeof(string) });

        public override IEnumerable<PathElement<TEntity>> AllPossiblePaths
        {
            get
            {
                yield return Violation;
            }
        }

        public RequirementFailed(Expression<Func<TEntity, bool>> property, Rule<TEntity> rule)
            : base(property, rule)
        {
        }

        protected ThrowViolation<TEntity> Violation;
        public IAdditionalRuleCondition<TEntity> WithModelViolation(Expression<Func<IViolation>> violation)
        {
            return WithModelViolation(Expression.Lambda<Func<TEntity, IViolation>>(violation.Body, Expression.Parameter(typeof(TEntity))));
        }

        public IAdditionalRuleCondition<TEntity> WithModelViolation(string violation)
        {
            var ctor = Expression.New(_SimpleViolation, Expression.Constant(violation));
            return WithModelViolation(Expression.Lambda<Func<TEntity, IViolation>>(ctor, Expression.Parameter(typeof(TEntity))));
        }

        //TODO: cache???
        //TODO: Test
        public override IEnumerable<MemberChainItem> ValidationSubjects
        {
            get
            {
                return base.ValidationSubjects.Union(Descriptor.GetMembersForParameter(EntityParameter));
            }
        }

        //TODO: does this helper work if serialized into JSON via a meta class, other helpers had to be converted to using pure Expressions (not lambdas)
        public IAdditionalRuleCondition<TEntity> WithModelViolation(Expression<Func<TEntity, IViolation>> violation)
        {
            Do(() => { Violation = new ThrowViolation<TEntity>(violation, ParentRule, ValidationSubjects); });
            return ParentRule;
        }

        protected override Expression _Compile(SwapPropVisitor visitor)
        {
            var next = AllPossiblePaths.SingleOrDefault(a => a != null);            
            if (next != null)
            {
                using (visitor.WithEntityParameter(EntityParameter))
                {
                    var des = visitor.Visit(Descriptor.WrappedExpression);
                    var nxt = next.Compile(visitor);
                    return Expression.IfThen(Expression.Not(des), nxt);
                }
            }

            return Expression.Empty();
        }
    }
}
