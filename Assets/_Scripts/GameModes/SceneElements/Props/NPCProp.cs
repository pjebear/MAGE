using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

class NPCInfo : PropInfoBase
{
    public string Name;
    public Appearance Appearance = new Appearance();

    public int Currency = 0;
    public List<Item> Inventory = new List<Item>();
    public List<Conversation> Conversations = new List<Conversation>();
}

