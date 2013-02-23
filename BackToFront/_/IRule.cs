﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

using BackToFront.Logic;
using BackToFront.Logic.Compilations;

namespace BackToFront
{
    public interface IRule<TEntity>
    {
        IOperators<TEntity> ElseIf(Expression<Func<TEntity, object>> property);
        IConditionSatisfied<TEntity> Else { get; }
    }
}
