using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


abstract class EquippableModifier : IModifier<Equippable>
{
    public abstract void Modify(Equippable t);
}

