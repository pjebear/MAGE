using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

class Character
{
    public int Id;
    public string Name;
    public int Experience;
    public int Level;
    public Specialization Specialization;
    public Attributes Attributes;

    public Equipment Equipment = new Equipment();

    public List<IActionModifier> ActionModifiers = new List<IActionModifier>();
    public List<ActionId> Actions = new List<ActionId>();
    public List<AuraType> Auras = new List<AuraType>();
    public List<ActionResponseId> Listeners = new List<ActionResponseId>();
}

