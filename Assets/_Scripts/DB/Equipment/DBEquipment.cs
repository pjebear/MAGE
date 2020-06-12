﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DB
{
    [System.Serializable]
    class DBEquipment : DBEntryBase
    {
        public int Id;
        public string Name;
        public int Category;
        public int Type;
        public float BlockChance;
        public float ParryChance;
        public int SpriteId;
        public int PrefabId;
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
