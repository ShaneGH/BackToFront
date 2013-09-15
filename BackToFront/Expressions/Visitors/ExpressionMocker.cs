using BackToFront.Dependency;
using BackToFront.Framework;
using BackToFront.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace BackToFront.Expressions.Visitors
{
    //public interface IExpressionMocker
    //{
    //    Expression Mock(Expression node);
    //    bool ContainsNothing { get; }
    //}

    public class ExpressionMocker : ExpressionVisitor//, IExpressionMocker
    {
        public const string EntityParameterName = "entity";
        public const string ValidationContextParameterName = "validationContext";

        public readonly Mocks Mocks;
        public readonly Dependencies Dependences;
        public readonly ParameterExpression EntityParameter;
        public readonly ParameterExpression ContextParameter = Expression.Parameter(typeof(ValidationContext), ValidationContextParameterName);

        private readonly Dictionary<MemberExpression, string> DependencyNameCache = new Dictionary<MemberExpression, string>();

        public ExpressionMocker(Type entityType)
            : this(null, null, entityType)
        {
        }

        public ExpressionMocker(IEnumerable<Mock> mocks, IDictionary<string, object> dependences, Type entityType)
        {
            if (entityType == null)
                throw new InvalidOperationException("##");

            Mocks = new Utilities.Mocks(mocks ?? Enumerable.Empty<Mock>(), Expression.PropertyOrField(ContextParameter, "Mocks"));
            Dependences = new Dependencies(dependences ?? new Dictionary<string, object>(), Expression.PropertyOrField(ContextParameter, "Dependencies"));
            EntityParameter = Expression.Parameter(entityType, EntityParameterName);
        }

        protected override Expression VisitParameter(ParameterExpression node)
        {
            lock (_EntityParameters)
                if (_EntityParameters.Any(a => a.Parameter == node))
                    return EntityParameter;

            return base.VisitParameter(node);
        }

        protected override Expression VisitMember(MemberExpression node)
        {
            if (DependencyNameCache.ContainsKey(node) || node.Member == DependencyWrapper.ValProperty(node.Type))
            {
                if (!DependencyNameCache.ContainsKey(node))
                {
                    DependencyNameCache.Add(node, Expression.Lambda<Func<DependencyWrapper>>(node.Expression).Compile()().DependencyName);
                }

                foreach (var d in Dependences)
                {
                    if (d.Key == DependencyNameCache[node])
                        return Dependences.ParameterForDependency(DependencyNameCache[node]);
                }
            }

            return base.VisitMember(node);
        }

        public Expression Mock(Expression node)
        {
            return Visit(node);
        }

        public override Expression Visit(Expression node)
        {
            if (!ContainsNothing)
            {
                foreach (var m in Mocks)
                {
                    if (m.Expression.IsSameExpression(node))
                    {
                        return Mocks.ParameterForMock(m);
                    }
                }
            }

            return base.Visit(node);
        }

        /// <summary>
        /// Returns whether there are any mocks or dependencies to insert
        /// </summary>
        public bool ContainsNothing
        {
            get
            {
                return Mocks.Count() == 0 && Dependences.Count() == 0;
            }
        }

        public object[] MockValues
        {
            get
            {
                return Mocks.Select(m => m.Value).ToArray();
            }
        }

        public IDictionary<string, object> DependencyValues
        {
            get
            {
                return Dependences.ToDictionary();
            }
        }

        public IDisposable WithEntityParameter(ParameterExpression parameter)
        {
            return new AddEntityParameter(parameter, this);
        }

        /// <summary>
        /// A temp list of ParameterExpressions with represent the EntityParameter. Be sure to lock this property before using it
        /// </summary>
        private readonly IList<AddEntityParameter> _EntityParameters = new List<AddEntityParameter>();

        private class AddEntityParameter : IDisposable
        {
            private readonly ExpressionMocker _Parent;
            public readonly ParameterExpression Parameter;

            public AddEntityParameter(ParameterExpression entityParameter, ExpressionMocker parent)
            {
                _Parent = parent;
                Parameter = entityParameter;

                lock (_Parent._EntityParameters)
                    _Parent._EntityParameters.Add(this);
            }

            public void Dispose()
            {
                lock (_Parent._EntityParameters)
                    _Parent._EntityParameters.Remove(this);
            }
        }
    }
}
