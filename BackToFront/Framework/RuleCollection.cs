using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

using BackToFront.Utilities;
using BackToFront.Expressions;
using BackToFront.Extensions.IEnumerable;
using BackToFront.Logic;
using BackToFront.Framework.Base;
using BackToFront.Validation;
using BackToFront.Meta;
using BackToFront.Enum;
using System.Runtime.Serialization;
using BackToFront.Dependency;
using BackToFront.Expressions.Visitors;

namespace BackToFront.Framework
{
    public class RuleCollection<TEntity> : IValidate<TEntity>
    {
        private readonly List<IValidate<TEntity>> _Rules = new List<IValidate<TEntity>>();
        public IEnumerable<IValidate<TEntity>> Rules
        {
            get
            {
                // TODO: cache
                return _Rules.ToArray();
            }
        }

        public void AddRule(IValidate<TEntity> rule)
        {
            _Rules.Add(rule);
        }

        public Expression Compile(SwapPropVisitor visitor)
        {
            var compiled = Rules.Select(r => r.Compile(visitor));

            return Expression.Block(compiled);
        }

        public IEnumerable<MemberChainItem> ValidationSubjects
        {
            get { return _Rules.Select(r => r.ValidationSubjects).Aggregate(); }
        }

        public IEnumerable<MemberChainItem> RequiredForValidation
        {
            get { return _Rules.Select(r => r.RequiredForValidation).Aggregate(); }
        }

        public bool PropertyRequirement
        {
            get { return false; }
        }
    }
}
