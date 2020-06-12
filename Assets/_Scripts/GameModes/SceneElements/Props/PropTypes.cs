using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

struct PropTag
{
    public int Id;
    public PropType PropType;

    public PropTag(ContainerPropId id) : this((int)id, PropType.Container) { }
    public PropTag(EnvironmentPropId id) : this((int)id, PropType.Environment) { }
    public PropTag(NPCId id) : this((int)id, PropType.NPC) { }
    public PropTag(int id, PropType propType)
    {
        Id = id;
        PropType = propType;
    }
}

class PropAttributes
{
    public int Currency = 0;
    public List<Item> Inventory = new List<Item>();
    public List<Conversation> Conversations = new List<Conversation>();
}

