using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;



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
}

