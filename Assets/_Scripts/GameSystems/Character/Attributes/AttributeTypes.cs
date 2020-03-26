using UnityEngine;
using System.Collections;
using System.Collections.Generic;


struct AttributeIndex
{
    public AttributeCategory Type;
    public int Index;
        
    public AttributeIndex(AttributeCategory type, int index)
    {
        Type = type;
        Index = index;
    }

    public override bool Equals(object obj)
    {
        if (!(obj is AttributeIndex))
        {
            return false;
        }

        var index = (AttributeIndex)obj;
        return Type == index.Type &&
               Index == index.Index;
    }

    public override int GetHashCode()
    {
        var hashCode = 686506176;
        hashCode = hashCode * -1521134295 + Type.GetHashCode();
        hashCode = hashCode * -1521134295 + Index.GetHashCode();
        return hashCode;
    }

    public static bool operator ==(AttributeIndex lhs, AttributeIndex rhs)
    {
        return lhs.Type == rhs.Type && lhs.Index == rhs.Index;
    }

    public static bool operator !=(AttributeIndex lhs, AttributeIndex rhs)
    {
        return lhs.Type != rhs.Type || lhs.Index != rhs.Index;
    }

    public AttributeIndex(PrimaryStat stat)
    {
        Type = AttributeCategory.Stat;
        Index = (int)stat;
    }

    public AttributeIndex(SecondaryStat stat)
    {
        Type = AttributeCategory.Stat;
        Index = (int)stat;
    }

    public AttributeIndex(TertiaryStat stat)
    {
        Type = AttributeCategory.Stat;
        Index = (int)stat;
    }

    public AttributeIndex(ResourceType resource)
    {
        Type = AttributeCategory.Resource;
        Index = (int)resource;
    }

    public AttributeIndex(AllignmentType allignmentType)
    {
        Type = AttributeCategory.Allignment;
        Index = (int)allignmentType;
    }
}

struct AttributeModifier
{
    public AttributeIndex AttributeIndex;
    public ModifierType ModifierType;
    public float Delta;
    public AttributeModifier(AttributeIndex index, ModifierType type, float delta)
    {
        AttributeIndex = index;
        ModifierType = type;
        Delta = delta;
    }

    public AttributeModifier(PrimaryStat stat, ModifierType type, float delta)
    {
        AttributeIndex = new AttributeIndex(stat);
        ModifierType = type;
        Delta = delta;
    }

    public AttributeModifier(SecondaryStat stat, ModifierType type, float delta)
    {
        AttributeIndex = new AttributeIndex(stat);
        ModifierType = type;
        Delta = delta;
    }

    public AttributeModifier(TertiaryStat stat, ModifierType type, float delta)
    {
        AttributeIndex = new AttributeIndex(stat);
        ModifierType = type;
        Delta = delta;
    }

    public AttributeModifier(ResourceType resourceType, ModifierType type, float delta)
    {
        AttributeIndex = new AttributeIndex(resourceType);
        ModifierType = type;
        Delta = delta;
    }

    public AttributeModifier(AllignmentType allignmentType, ModifierType type, float delta)
    {
        AttributeIndex = new AttributeIndex(allignmentType);
        ModifierType = type;
        Delta = delta;
    }

    public override string ToString()
    {
        return string.Format("AttribModifier: [{0}][{1}][{2}][{3}]", AttributeIndex.Type.ToString(), AttributeIndex.Index, ModifierType.ToString(), Delta);
    }
}

static class AttributeUtil
{
    public static int ResourceFromAttribtues(ResourceType resourceType, Attributes stats)
    {
        float resourceValue = stats[AttributeCategory.Resource][(int)resourceType].Value;

        switch (resourceType)
        {
            case (ResourceType.Health):
                resourceValue += stats[AttributeCategory.Stat][(int)PrimaryStat.Might].Value;
                break;
            case (ResourceType.Mana):
                resourceValue += stats[AttributeCategory.Stat][(int)PrimaryStat.Magic].Value;
                break;
            case (ResourceType.Endurance):
                resourceValue += stats[AttributeCategory.Stat][(int)PrimaryStat.Finese].Value;
                break;
        }

        return (int)resourceValue;
    }
}
