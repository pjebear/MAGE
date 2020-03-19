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
    public SpecializationType Specialization;
    public Attributes Attributes;

    public List<IActionModifier> ActionModifiers = new  List<IActionModifier>();
    public List<ActionId> Actions = new List<ActionId>();
    public List<AuraType> Auras = new List<AuraType>();
    public List<ActionResponseId> Listeners = new List<ActionResponseId>();
    public Equipment Equipment = new Equipment();

    public Character(int id, DB.CharacterInfo info)
    {
        Id = id;
        Name = info.Name;
        Experience = info.Experience;

        Attributes = new Attributes(info.Attributes);
        Specialization = info.CurrentSpecialization;
    }
}

