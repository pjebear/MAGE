using UnityEngine;
using System.Collections;
using System.Collections.Generic;

class Attributes
{
    private Attribute[][] mAttributes;

    public static Attribute[][] Empty
    {
        get
        {
            Attribute[][] attributes = new Attribute[][]
            {
                new Attribute[(int)CharacterStat.NUM]
                , new Attribute[(int)ResourceType.NUM]
                , new Attribute[(int)AllignmentType.NUM]
                , new Attribute[(int)ProficiencyType.NUM]
            };
            for (int categoryIdx = 0; categoryIdx < (int)AttributeCategory.NUM; ++categoryIdx)
            {
                for (int attributeIdx = 0; attributeIdx < attributes[categoryIdx].Length; ++attributeIdx)
                {
                    attributes[categoryIdx][attributeIdx] = new Attribute();
                }
            }
            return attributes;
        }
    }

    public float this[AttributeIndex index]
    {
        get
        {
            return mAttributes[(int)index.Type][index.Index].Value;
        }
    }

    public Attribute[] this[AttributeCategory type] { get { return mAttributes[(int)type]; } }

    //public Attributes(AttributesSeed seed)
    //{
    //    mAttributes = Empty;
    //    int statIdx = 0;
    //    for (int i = 0; i < (int)PrimaryStat.NUM; ++i)
    //    {
    //        mAttributes[(int)AttributeCategory.Stat][statIdx++] = new Attribute(seed.PrimaryStats[i]);
    //    }
    //    for (int i = 0; i < (int)SecondaryStat.NUM; ++i)
    //    {
    //        mAttributes[(int)AttributeCategory.Stat][statIdx++] = new Attribute(seed.SecondaryStats[i]);
    //    }
    //    for (int i = 0; i < (int)TertiaryStat.NUM; ++i)
    //    {
    //        mAttributes[(int)AttributeCategory.Stat][statIdx++] = new Attribute(seed.TertiaryStats[i]);
    //    }

    //    AttributeUtil.CalculateResourcesFromStats(mAttributes[(int)AttributeCategory.Stat], mAttributes[(int)AttributeCategory.Resource], false);

    //    mAttributes[(int)AttributeCategory.Allignment][(int)seed.Allignment] = new Attribute(AttributeConstants.MainAllignmentValue);
    //    AllignmentType[] subAllignments = AttributeUtil.GetSubAllignments(seed.Allignment);
    //    for (int i = 0; i < subAllignments.Length; ++i)
    //    {
    //        mAttributes[(int)AttributeCategory.Allignment][i] = new Attribute(AttributeConstants.SubAllignmentValue);
    //    }
    //}

    public Attributes(Attribute[][] attributes)
    {
        mAttributes = attributes;
    }

    public Attributes(List<List<float>> attributes)
        : this(Attributes.Empty)
    {
        for (int attributeType = 0; attributeType < attributes.Count; ++attributeType)
        {
            Logger.Assert(attributes[attributeType].Count == mAttributes[attributeType].Length, LogTag.Character,
                "Attributes",
                string.Format("Invalid attribute length from db for attribute type {0}. Expected {1}, Got {2}", attributeType, mAttributes[attributeType].Length, attributes[attributeType].Count)
                , LogLevel.Error);
            if (attributes[attributeType].Count == mAttributes[attributeType].Length)
            {
                for (int attributeIdx = 0; attributeIdx < mAttributes[attributeType].Length; ++attributeIdx)
                {
                    mAttributes[attributeType][attributeIdx].Set(attributes[attributeType][attributeIdx]);
                }
            }  
        }
    }

    public Attributes(Attributes container)
    {
        mAttributes = Empty;
        for (int i = 0; i < (int)AttributeCategory.NUM; ++i)
        {
            container.mAttributes[i].CopyTo(mAttributes[i], 0);
        }
    }

    public void Modify(AttributeModifier delta)
    {
        this[delta.AttributeIndex.Type][delta.AttributeIndex.Index] += delta;
    }

    public void Revert(AttributeModifier delta)
    {
        this[delta.AttributeIndex.Type][delta.AttributeIndex.Index] -= delta;
    }
}
