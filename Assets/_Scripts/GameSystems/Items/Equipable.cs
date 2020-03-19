using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


class Equippable : Item
{
    public EquippableId EquipmentId { get { return (EquippableId)Tag.ItemId; } }
    public EquipableType EquipableType;

    public Equippable(EquippableId id, EquipableType type, Appearance appearance)
        : base(new ItemTag(id), appearance)
    {
        EquipableType = type;
    }
}

