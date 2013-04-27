using BackToFront.Dependency;
using BackToFront.Utilities;
using System.Linq.Expressions;

namespace BackToFront.Expressions.Visitors
{
    public class MockVisitor : ExpressionVisitor
    {
        public readonly Mocks Mocks;

        public MockVisitor(Mocks mocks)
        {
            Mocks = mocks;
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
    }
}
