using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MAGE.GameServices
{
    interface IModifier<T>
    {
        void Modify(T t);
    }
}


