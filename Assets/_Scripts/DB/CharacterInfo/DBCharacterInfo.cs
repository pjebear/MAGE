using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DB
{
    class CharacterInfo
    {
        public string Name = "EMPTY";
        public int Level = 1;
        public int Experience = 0;
        public List<List<float>> Attributes = new List<List<float>>();
        public SpecializationType CurrentSpecialization = SpecializationType.NONE;

        public override string ToString()
        {
            return "Name: " + Name;
        }
    }

    class DBCharacterInfo : DBEntryBase<CharacterInfo>
    {
        public override void Copy(CharacterInfo from, CharacterInfo to)
        {
            to.Name = from.Name;
            to.Level = from.Level;
            to.Experience = from.Experience;
            to.Attributes.Clear();
            foreach (List<float> attributes in from.Attributes)
            {
                List<float> attributeList = new List<float>();
                attributeList.AddRange(attributes);
                to.Attributes.Add(attributeList);
            }

            to.CurrentSpecialization = from.CurrentSpecialization;
        }
    }
}


