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

    public Attributes(Attribute[][] attributes)
    {
        mAttributes = attributes;
    }

    public Attributes(List<DB.DBAttributes> attributes)
        : this(Attributes.Empty)
    {
        for (int attributeCategory = 0; attributeCategory < attributes.Count; ++attributeCategory)
        {
            Logger.Assert(attributes[attributeCategory].AttributeCategory == attributeCategory, LogTag.Character, "Attributes",
                string.Format("Invalid attribute category for db attributes. Expected {0}, Got {1}",
                ((AttributeCategory)attributeCategory).ToString(), attributes[attributeCategory].AttributeCategory.ToString()), LogLevel.Error);

            Logger.Assert(attributes[attributeCategory].Attributes.Count == mAttributes[attributeCategory].Length, LogTag.Character, "Attributes",
                string.Format("Invalid attribute length from db for attribute type {0}. Expected {1}, Got {2}", 
                attributeCategory, mAttributes[attributeCategory].Length, attributes[attributeCategory].Attributes.Count), LogLevel.Error);

            if (attributes[attributeCategory].Attributes.Count == mAttributes[attributeCategory].Length 
                && attributes[attributeCategory].AttributeCategory == attributeCategory)
            {
                for (int attributeIdx = 0; attributeIdx < mAttributes[attributeCategory].Length; ++attributeIdx)
                {
                    mAttributes[attributeCategory][attributeIdx].Set(attributes[attributeCategory].Attributes[attributeIdx]);
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
