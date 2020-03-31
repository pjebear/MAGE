using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DB
{
    [System.Serializable]
    class DBAttributeModifier : DBEntryBase
    {
        public int AttributeCategory;
        public int AttributeId;
        public int ModifierType;
        public float Modifier;

        public override void Copy(DBEntryBase _from, DBEntryBase _to)
        {
            DBAttributeModifier from = _from as DBAttributeModifier;
            DBAttributeModifier to = _to as DBAttributeModifier;

            to.AttributeCategory = from.AttributeCategory;
            to.AttributeId = from.AttributeId;
            to.ModifierType = from.ModifierType;
            to.Modifier = from.Modifier;
        }
    }

    [System.Serializable]
    class DBAttributeScalar : DBEntryBase
    {
        public int AttributeCategory;
        public int AttributeId;
        public float Scalar;

        public override void Copy(DBEntryBase _from, DBEntryBase _to)
        {
            DBAttributeScalar from = _from as DBAttributeScalar;
            DBAttributeScalar to = _to as DBAttributeScalar;

            to.AttributeCategory = from.AttributeCategory;
            to.AttributeId = from.AttributeId;
            to.Scalar = from.Scalar;
        }
    }

    [System.Serializable]
    class DBAttribute : DBEntryBase
    {
        public int AttributeCategory;
        public int AttributeId;
        public float Value;

        public override void Copy(DBEntryBase _from, DBEntryBase _to)
        {
            DBAttribute from = _from as DBAttribute;
            DBAttribute to = _to as DBAttribute;

            to.AttributeCategory = from.AttributeCategory;
            to.AttributeId = from.AttributeId;
            to.Value = from.Value;
        }
    }

    [System.Serializable]
    class DBAttributes : DBEntryBase
    {
        public int AttributeCategory;
        public List<float> Attributes = new List<float>();

        public override void Copy(DBEntryBase _from, DBEntryBase _to)
        {
            DBAttributes from = _from as DBAttributes;
            DBAttributes to = _to as DBAttributes;

            to.AttributeCategory = from.AttributeCategory;
            to.Attributes = new List<float>(from.Attributes);
        }
    }
}