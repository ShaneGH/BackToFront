﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace BackToFront.Logic
{
    public interface IRequirementFailed2<TEntity>
    {
        IModelViolation<TEntity> RequirementFailed { get; }
    }
}