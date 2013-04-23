using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BackToFront.Enum;
using BackToFront.Expressions;

namespace BackToFront.Framework.Meta
{
    public interface IMetaElement
    {
        IEnumerable<IMetaElement> Children { get; }
        PathElementType Type { get; }
        ExpressionWrapperBase Code { get; }
    }
}
