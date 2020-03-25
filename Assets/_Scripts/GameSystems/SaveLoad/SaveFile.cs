using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SaveLoad
{
    [System.Serializable]
    class Party
    {
        public int Currency = 0;
        public List<int> ItemIds = new List<int>();
        public List<int> ItemCounts = new List<int>();
        public List<int> CharacterIds = new List<int>();
    }

    [System.Serializable]
    class SaveFile
    {
        public string Name;
        public Party Party = new Party();
    }
}




