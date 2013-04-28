using BackToFront.Dependency;
using BackToFront.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace BackToFront.Expressions.Visitors
{
    public interface ISwapPropVisitor
    {
        Expression Visit(Expression node);
        bool ContainsNothing { get; }
    }

    public class SwapPropVisitor : ExpressionVisitor, ISwapPropVisitor
    {
        public readonly Mocks Mocks;
        public readonly Dependencies Dependences;

        private readonly Dictionary<MemberExpression, string> DependencyNameCache = new Dictionary<MemberExpression, string>();

        // TODO: delete this constructor
        public SwapPropVisitor()
            : this(new Mocks(), new Dependencies())
        {
        }

        public SwapPropVisitor(Mocks mocks, Dependencies dependences)
        {
            Mocks = mocks;
            Dependences = dependences;
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

        public override Expression Visit(Expression node)
        {
            foreach (var m in Mocks)
            {
                if (m.Expression.IsSameExpression(node))
                {
                    return Mocks.ParameterForMock(m);
                }
            }

            return base.Visit(node);
        }

        public bool ContainsNothing
        {
            get
            {
                return Mocks.Count() == 0 && Dependences.Count() == 0;
            }
        }

        public IEnumerable<ParameterExpression> Parameters
        {
            get
            {
                yield return Mocks.Parameter;
                yield return Dependences.Parameter;
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
                return Dependences.ToDictionary(a => a.Key, a => a.Value);
            }
        }
    }
}
