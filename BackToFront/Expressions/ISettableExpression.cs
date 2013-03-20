using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BackToFront.Expressions
{
    public interface IPropertyChainGetter
    {
        // TODO: generics here
        object Get(object root);
    }

    public interface IPropertyChain : IPropertyChainGetter
    {
        bool CanSet { get; }

        // TODO: generics here
        void Set(object root, object value);
    }
}
