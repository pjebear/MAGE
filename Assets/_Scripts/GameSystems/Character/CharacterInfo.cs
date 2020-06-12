using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

class CharacterInfo
{
    public int Id;
    public string Name;
    public int Experience;
    public int Level;
    public Specialization[] Specializations = new Specialization[(int)SpecializationType.NUM];
    public SpecializationType CurrentSpecializationType;
    public Specialization CurrentSpecialization { get { return Specializations[(int)CurrentSpecializationType]; } }
    public Attributes Attributes = null;
    public Appearance Appearance = new Appearance();

    public Equipment Equipment = new Equipment();

    public List<IActionModifier> ActionModifiers = new List<IActionModifier>();
    public List<EquippableModifier> EquippableModifiers = new List<EquippableModifier>();
    public List<ActionId> Actions = new List<ActionId>();
    public List<AuraType> Auras = new List<AuraType>();
    public List<ActionResponseId> Listeners = new List<ActionResponseId>();
}

