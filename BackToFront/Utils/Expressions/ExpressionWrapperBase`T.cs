using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

using BackToFront.Extensions.Reflection;

namespace BackToFront.Utils.Expressions
{
    internal abstract class ExpressionWrapperBase<TExpression> : ExpressionWrapperBase
        where TExpression : Expression
    {
        protected readonly TExpression Expression;

        protected readonly ReadOnlyCollection<ParameterExpression> Parameters;

        public override ReadOnlyCollection<ParameterExpression> WrappedExpressionParameters
        {
            get { return Parameters; }
        }

        public ExpressionWrapperBase(TExpression expression, ReadOnlyCollection<ParameterExpression> parameters)
        {
            if (expression == null)
                throw new ArgumentNullException("expression");
            if (parameters == null)
                throw new ArgumentNullException("parameters");
            if (parameters.Count == 0)
                throw new ArgumentException("The expression must contain paramaters");

            Expression = expression;
            Parameters = parameters;
        }

        protected ExpressionWrapperBase CreateChildWrapper(Expression expression)
        {
            return CreateChildWrapper(expression, Parameters);
        }

        public override void OnSet(object root, object value)
        {
            foreach (var member in _PropertyChain.Take(_PropertyChain.Count() - 1))
            {
                root = member.Get(root);
            }

            _PropertyChain.Last().Set(root, value);
        }

        IEnumerable<MemberInfo> _PropertyChain;
        public override bool CanSet
        {
            get
            {
                if (_PropertyChain == null)
                {
                    if (Parameters.Count != 1)
                    {
                        _PropertyChain = Enumerable.Empty<MemberInfo>();
                    }
                    else
                    {
                        var param = false;
                        List<MemberInfo> members = new List<MemberInfo>();
                        Expression current = this.Expression;
                        while (!param)
                        {
                            if (current is MemberExpression)
                            {
                                var member = (current as MemberExpression);
                                if (member.Member is PropertyInfo)
                                {
                                    if ((member.Member as PropertyInfo).GetSetMethod(false) == null)
                                        break;

                                    members.Add(member.Member);
                                }
                                else if (member.Member is FieldInfo)
                                {
                                    if ((member.Member as FieldInfo).IsPrivate)
                                        break;

                                    members.Add(member.Member);
                                }
                                else
                                {
                                    break;
                                }

                                current = member.Expression;
                            }
                            else if (current is ParameterExpression)
                            {
                                param = true;
                            }
                            else
                            {
                                break;
                            }
                        }

                        members.Reverse();
                        _PropertyChain = param ? members.ToArray() : Enumerable.Empty<MemberInfo>();
                    }
                }

                return _PropertyChain.Any();
            }
        }
    }
}
