﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MAGE.GameSystems.Stats
{
    [System.Serializable]
    class Attributes
    {
        [SerializeField]
        private Attribute[][] mAttributes;

        public static Attribute[][] Empty
        {
            get
            {
                Attribute[][] attributes = new Attribute[][]
                {
                new Attribute[(int)PrimaryStat.NUM]
                , new Attribute[(int)SecondaryStat.NUM]
                , new Attribute[(int)TertiaryStat.NUM]
                , new Attribute[(int)ResourceType.NUM]
                , new Attribute[(int)AllignmentType.NUM]
                , new Attribute[(int)StatusType.NUM]
                };
                for (int categoryIdx = 0; categoryIdx < (int)AttributeCategory.NUM; ++categoryIdx)
                {
                    for (int attributeIdx = 0; attributeIdx < attributes[categoryIdx].Length; ++attributeIdx)
                    {
                        attributes[categoryIdx][attributeIdx] = new Attribute(AttributeUtil.ToString((AttributeCategory)categoryIdx, attributeIdx));
                    }
                }
                return attributes;
            }
        }

        public float this[PrimaryStat stat] { get { return this[new AttributeIndex(stat)]; } }
        public float this[SecondaryStat stat] { get { return this[new AttributeIndex(stat)]; } }
        public float this[TertiaryStat stat] { get { return this[new AttributeIndex(stat)]; } }
        public float this[ResourceType resource] { get { return this[new AttributeIndex(resource)]; } }
        public float this[AllignmentType allignmentType] { get { return this[new AttributeIndex(allignmentType)]; } }
        public int this[StatusType statusType] { get { return UnityEngine.Mathf.RoundToInt(this[new AttributeIndex(statusType)]); } }
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

        public void Clear(AttributeCategory category)
        {
            Attribute[] toClear = mAttributes[(int)category];
            for (int i = 0; i < toClear.Length; ++i)
            {
                toClear[i].Set(0);
            }
        }
    }
}

