﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BackToFront.Logic
{
    public interface IModelViolation1<TEntity>
    {
        IRule<TEntity> ModelViolationIs(IViolation violation);
    }
}
