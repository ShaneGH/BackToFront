using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using BackToFront.Enum;

namespace BackToFront.Utils
{    
    internal class Condition<TEntity>
    {
        private readonly List<ConditionPart> _parts = new List<ConditionPart>();
        
        private Func<TEntity, bool> _compiledCondition;
        public Func<TEntity, bool> CompiledCondition
        {
            get
            {
                lock (_parts)
                {
                    // lazy compile
                    if (_compiledCondition == null)
                    {
                        var parts = _parts.Select(a => a.Copy).ToList();

                        // first, concatonate all AND statements into the sibling before
                        for (int i = 1, ii = parts.Count; i < ii; i++)
                        {
                            if (parts[i].Operator == LogicalOperator.And)
                            {
                                // record conditions
                                Func<TEntity, bool> c1 = parts[i - 1].Eval, c2 = parts[i].Eval;
                                // concatonate conditions
                                parts[i - 1].Eval = a => c1(a) && c2(a);
                                // remove second condition
                                parts.RemoveAt(i);
                                i--;
                                ii--;
                            }
                        }

                        // second, concatonate all OR statements into 1 single statement
                        for (int i = 1, ii = parts.Count; i < ii; i++)
                        {
                            Func<TEntity, bool> c1 = parts[0].Eval, c2 = parts[i].Eval;
                            parts[0].Eval = a => c1(a) || c2(a);
                        }

                        _compiledCondition = parts[0].Eval;
                    }

                    return _compiledCondition;
                }
            }
        }

        public void Add(LogicalOperator op, Func<TEntity, object> lhs, Func<TEntity, Func<TEntity, object>, Func<TEntity, object>, bool> @operator, Func<TEntity, object> rhs)
        {
            lock (_parts)
            {
                // force re-compile
                _compiledCondition = null;

                _parts.Add(new ConditionPart
                {
                    Operator = op,
                    Eval = e => @operator(e, lhs, rhs)
                });
            }
        }

        private class ConditionPart
        {
            public Func<TEntity, bool> Eval { get; set; }
            public LogicalOperator Operator { get; set; }

            public ConditionPart Copy
            {
                get
                {
                    return new ConditionPart
                    {
                        Eval = Eval,
                        Operator = Operator
                    };
                }
            }
        }
    }
}
