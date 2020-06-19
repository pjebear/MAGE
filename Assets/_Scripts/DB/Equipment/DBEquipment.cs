using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DB
{
    [System.Serializable]
    class DBEquipment : DBEntryBase
    {
        public int Id = -1;
        public string Name = "INVALID";
        public int Category = -1;
        public int Type = -1;
        public float BlockChance = 0;
        public float ParryChance = 0;
        public int SpriteId = -1;
        public int PrefabId = -1;
        public int ActionId = -1;
        public DBRangeInfo Range = new DBRangeInfo();
        public List<DBAttributeScalar> EffectivenessScalars = new List<DBAttributeScalar>();
        public List<DBAttributeModifier> EquipBonuses = new List<DBAttributeModifier>();
        
        public override void Copy(DBEntryBase _from, DBEntryBase _to)
        {
            DBEquipment from = _from as DBEquipment;
            DBEquipment to = _to as DBEquipment;

            to.Id = from.Id;
            to.Name = from.Name;
            to.Category = from.Category;
            to.Type = from.Type;
            to.BlockChance = from.BlockChance;
            to.ParryChance = from.ParryChance;
            to.SpriteId = from.SpriteId;
            to.PrefabId = from.PrefabId;
            to.ActionId = from.ActionId;
            to.Range.Set(from.Range);

            to.EffectivenessScalars.Clear();
            foreach (DBAttributeScalar scalar in from.EffectivenessScalars)
            {
                DBAttributeScalar toAdd = new DBAttributeScalar();
                toAdd.Set(scalar);
                to.EffectivenessScalars.Add(toAdd);
            }

            to.EquipBonuses.Clear();
            foreach (DBAttributeModifier bonus in from.EquipBonuses)
            {
                DBAttributeModifier toAdd = new DBAttributeModifier();
                toAdd.Set(bonus);
                to.EquipBonuses.Add(toAdd);
            }
        }
    }
    
}
