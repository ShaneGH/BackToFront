﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BackToFront.Logic
{
    public interface ISubRule<TEntity, TViolation> : IRule<TEntity, TViolation>, IRequires<TEntity, TViolation>
    {
    }
}
