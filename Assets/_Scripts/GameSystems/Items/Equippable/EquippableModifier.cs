using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MAGE.GameServices
{
    abstract class EquippableModifier : IModifier<Equippable>
    {
        public abstract void Modify(Equippable t);
    }
}

