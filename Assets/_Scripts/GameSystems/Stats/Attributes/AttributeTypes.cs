using System.Collections;
using System.Collections.Generic;

namespace MAGE.GameSystems.Stats
{
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
            Type = AttributeCategory.PrimaryStat;
            Index = (int)stat;
        }

        public AttributeIndex(SecondaryStat stat)
        {
            Type = AttributeCategory.SecondaryStat;
            Index = (int)stat;
        }

        public AttributeIndex(TertiaryStat stat)
        {
            Type = AttributeCategory.TertiaryStat;
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

        public AttributeIndex(StatusType statusType)
        {
            Type = AttributeCategory.Status;
            Index = (int)statusType;
        }
    }

    struct AttributeModifier
    {
        public AttributeIndex AttributeIndex;
        public ModifierType ModifierType;
        public float Delta;

        public AttributeModifier(PrimaryStat stat, ModifierType type, float delta) : this(new AttributeIndex(stat), type, delta) { }
        public AttributeModifier(SecondaryStat stat, ModifierType type, float delta) : this(new AttributeIndex(stat), type, delta) { }
        public AttributeModifier(TertiaryStat stat, ModifierType type, float delta) : this(new AttributeIndex(stat), type, delta) { }
        public AttributeModifier(ResourceType resourceType, ModifierType type, float delta) : this(new AttributeIndex(resourceType), type, delta) { }
        public AttributeModifier(AllignmentType allignmentType, ModifierType type, float delta) : this(new AttributeIndex(allignmentType), type, delta) { }
        public AttributeModifier(StatusType statusType, ModifierType type, float delta) : this(new AttributeIndex(statusType), type, delta) { }

        public AttributeModifier(AttributeIndex index, ModifierType type, float delta)
        {
            AttributeIndex = index;
            ModifierType = type;
            Delta = delta;
        }

        public AttributeModifier Copy()
        {
            return new AttributeModifier(AttributeIndex, ModifierType, Delta);
        }

        public override string ToString()
        {
            return string.Format("AttribModifier: [{0}][{1}][{2}][{3}]", AttributeIndex.Type.ToString(), AttributeIndex.Index, ModifierType.ToString(), Delta);
        }
    }

    struct AttributeScalar
    {
        public AttributeIndex AttributeIndex;
        public float Scalar;

        public AttributeScalar(PrimaryStat stat, float scalar) : this(new AttributeIndex(stat), scalar) { }
        public AttributeScalar(SecondaryStat stat, float scalar) : this(new AttributeIndex(stat), scalar) { }
        public AttributeScalar(TertiaryStat stat, float scalar) : this(new AttributeIndex(stat), scalar) { }
        public AttributeScalar(ResourceType resourceType, float scalar) : this(new AttributeIndex(resourceType), scalar) { }
        public AttributeScalar(AllignmentType allignmentType, float scalar) : this(new AttributeIndex(allignmentType), scalar) { }

        public AttributeScalar(AttributeIndex index, float scalar)
        {
            AttributeIndex = index;
            Scalar = scalar;
        }

        public float GetScalar(Attributes attributes)
        {
            return attributes[AttributeIndex] * Scalar;
        }

        public override string ToString()
        {
            return string.Format("AttributeScalar: [{0}][{1}][{2}]", AttributeIndex.Type.ToString(), AttributeIndex.Index, Scalar);
        }
    }
}