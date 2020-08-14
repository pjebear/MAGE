using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MAGE.GameServices.World.Internal
{
    class PartyInfo
    {
        public int Currency = 0;
        public Inventory Inventory = new Inventory();
        public List<int> CharacterIds = new List<int>();
        public int AvatarId = 0;
    }
}