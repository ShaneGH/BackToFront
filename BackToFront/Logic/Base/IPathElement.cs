using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BackToFront.Logic.Base
{
    public interface IPathElement
    {
        IPathElement NextOption { get; }
    }
}
