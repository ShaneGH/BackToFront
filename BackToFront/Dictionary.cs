using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

using BackToFront.Logic;
using BackToFront.Framework;

namespace BackToFront
{
    public class Rules
    {
        #region Static

        internal static readonly Rules Repository = new Rules();
        public static void Add<TEntity, TViolation>(Action<IRule<TEntity, TViolation>> rule)
            where TViolation : IViolation
        {
            Repository._Add(rule);
        }

        #endregion

        internal Dictionary<Type, IValidate> Registered = new Dictionary<Type, IValidate>();

        private Rules()
        { }

        /// <summary>
        /// Exposed through static method
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        /// <typeparam name="TViolation"></typeparam>
        /// <param name="rule"></param>
        private void _Add<TEntity, TViolation>(Action<IRule<TEntity, TViolation>> rule)
            where TViolation : IViolation
        {
            var type = typeof(TEntity);
            if (Repository.Registered.ContainsKey(type))
                throw new InvalidOperationException();

            Registered.Add(type, new Rule<TEntity, TViolation>());
            rule((Rule<TEntity, TViolation>)Repository.Registered[type]);
        }
    }
}