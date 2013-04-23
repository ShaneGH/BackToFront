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

namespace BackToFront.Framework
{
    public class RuleCollection<TEntity> : IValidate, IValidate<TEntity>
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

        public IViolation ValidateEntity(object subject, Mocks mocks)
        {
            // TODO: catch cast exception
            var sub = (TEntity)subject;
            return ValidateEntity(sub, new ValidationContext { Mocks = mocks });
        }

        public IEnumerable<IViolation> FullyValidateEntity(object subject, Mocks mocks)
        {
            // TODO: catch cast exception
            var sub = (TEntity)subject;
            IList<IViolation> violationList = new List<IViolation>();
            FullyValidateEntity(sub, violationList, new ValidationContext { Mocks = mocks });
            return violationList.ToArray();
        }

        public void AddRule(IValidate<TEntity> rule)
        {
            _Rules.Add(rule);
        }

        public IViolation ValidateEntity(TEntity subject, ValidationContext context)
        {
            IViolation violation;
            foreach (var rule in _Rules)
                if ((violation = rule.ValidateEntity(subject, context.Copy())) != null)
                    return violation;

            return null;
        }

        public void FullyValidateEntity(TEntity subject, IList<IViolation> violationList, ValidationContext context)
        {
            _Rules.Each(r => r.FullyValidateEntity(subject, violationList, context.Copy()));
        }

        public IEnumerable<AffectedMembers> AffectedMembers
        {
            get { return _Rules.Select(r => r.AffectedMembers).Aggregate(); }
        }

        public bool PropertyRequirement
        {
            get { return false; }
        }

        private MetaData _Meta;
        public PathElementMeta Meta
        {
            get { return _Meta ?? (_Meta = new MetaData(this)); }
        }

        [DataContract]
        private class MetaData : PathElementMeta
        {
            private readonly RuleCollection<TEntity> _Owner;

            public MetaData(RuleCollection<TEntity> owner)
            {
                _Owner = owner;
            }

            public override IEnumerable<PathElementMeta> Children
            {
                get
                {
                    return _Owner.Rules.Select(a => a.Meta);
                }
            }

            public override ExpressionElementMeta Code
            {
                get { return null; }
            }

            public override PathElementType Type
            {
                get { return PathElementType.RuleCollection; }
            }
        }
    }
}
