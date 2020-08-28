using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace MAGE.GameSystems.Characters
{
    class Equipment
    {
        public static Equippable NO_EQUIPMENT = null;

        public enum Slot
        {
            INVALID = -1,

            Armor,
            LeftHand,
            RightHand,
            Accessory,

            NUM
        }

        private Equippable[] mEquipment;

        public Equipment()
        {
            mEquipment = new Equippable[(int)Slot.NUM];
        }

        public Equippable this[Slot slot]
        {
            get { return mEquipment[(int)slot]; }
            set { mEquipment[(int)slot] = value; }
        }

        public bool IsSlotEmpty(Slot slot)
        {
            return this[slot] == NO_EQUIPMENT;
        }

        public List<AttributeModifier> GetAttributeModifiers()
        {
            List<AttributeModifier> modifiers = new List<AttributeModifier>();

            foreach (Equippable equippable in mEquipment)
            {
                if (equippable != NO_EQUIPMENT)
                {
                    modifiers.AddRange(equippable.EquipBonuses);
                }
            }

            return modifiers;
        }
    }
}


