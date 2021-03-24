using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace MAGE.GameSystems.Stats
{
    [System.Serializable]
    class Attribute
    {
        // members
        private string mName;
        private float mBase;
        private float mIncrement;
        private float mMultiplier;

        // properties
        public float Value
        {
            get
            {
                return (mBase + mIncrement) * mMultiplier;
            }
        }

        public float Base
        {
            get
            {
                return mBase;
            }
        }

        // Operators
        public static Attribute operator +(Attribute lhs, AttributeModifier rhs)
        {
            Attribute modified = (Attribute)lhs.MemberwiseClone();
            Modify(modified, rhs);
            return modified;
        }

        public static Attribute operator -(Attribute lhs, AttributeModifier rhs)
        {
            Attribute modified = (Attribute)lhs.MemberwiseClone();
            Revert(modified, rhs);
            return modified;
        }

        public override string ToString()
        {
            return string.Format("{0}: Base: [{1}] Inc: [{2}] Mult: [{3}]", mName, mBase, mIncrement, mMultiplier);
        }

        // Methods
        public Attribute(string name) : this(0, name) { /* empty */ }

        public Attribute(float value, string name)
        {
            mName = name;
            Set(value);
        }

        public void Set(float value)
        {
            mBase = value;
            mIncrement = 0;
            mMultiplier = 1;
        }

        public static void Modify(Attribute attribute, AttributeModifier modifier)
        {
            switch (modifier.ModifierType)
            {
                case ModifierType.Increment:
                    attribute.mIncrement += modifier.Delta;
                    break;
                case ModifierType.Multiply:
                    attribute.mMultiplier += modifier.Delta;
                    break;
            }
        }

        public static void Revert(Attribute attribute, AttributeModifier modifier)
        {
            switch (modifier.ModifierType)
            {
                case ModifierType.Increment:
                    attribute.mIncrement -= modifier.Delta;
                    break;
                case ModifierType.Multiply:
                    attribute.mMultiplier -= modifier.Delta;
                    break;
            }
        }
    }

}


