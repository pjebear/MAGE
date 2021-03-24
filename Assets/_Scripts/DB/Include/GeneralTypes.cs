using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MAGE.DB
{
    delegate void DBUpdateCB<KeyType>(KeyType key);

    [System.Serializable]
    class DBRangeInfo : Internal.DBEntryBase
    {
        public int Min;
        public int Max;
        public int Elevation;
        public int AreaType;
        public int TargetingType;

        public override void Copy(Internal.DBEntryBase _from, Internal.DBEntryBase _to)
        {
            DBRangeInfo from = _from as DBRangeInfo;
            DBRangeInfo to = _to as DBRangeInfo;

            to.Min = from.Min;
            to.Max = from.Max;
            to.Elevation = from.Elevation;
            to.AreaType = from.AreaType;
            to.TargetingType = from.TargetingType;
        }
    }

    [System.Serializable]
    class DBAttributeModifier : Internal.DBEntryBase
    {
        public int AttributeCategory;
        public int AttributeId;
        public int ModifierType;
        public float Modifier;

        public override void Copy(Internal.DBEntryBase _from, Internal.DBEntryBase _to)
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
    class DBAttributeScalar : Internal.DBEntryBase
    {
        public int AttributeCategory;
        public int AttributeId;
        public float Scalar;

        public override void Copy(Internal.DBEntryBase _from, Internal.DBEntryBase _to)
        {
            DBAttributeScalar from = _from as DBAttributeScalar;
            DBAttributeScalar to = _to as DBAttributeScalar;

            to.AttributeCategory = from.AttributeCategory;
            to.AttributeId = from.AttributeId;
            to.Scalar = from.Scalar;
        }
    }

    [System.Serializable]
    class DBAttribute : Internal.DBEntryBase
    {
        public int AttributeCategory;
        public int AttributeId;
        public float Value;

        public override void Copy(Internal.DBEntryBase _from, Internal.DBEntryBase _to)
        {
            DBAttribute from = _from as DBAttribute;
            DBAttribute to = _to as DBAttribute;

            to.AttributeCategory = from.AttributeCategory;
            to.AttributeId = from.AttributeId;
            to.Value = from.Value;
        }
    }

    [System.Serializable]
    class DBAttributes : Internal.DBEntryBase
    {
        public int AttributeCategory;
        public List<float> Attributes = new List<float>();

        public override void Copy(Internal.DBEntryBase _from, Internal.DBEntryBase _to)
        {
            DBAttributes from = _from as DBAttributes;
            DBAttributes to = _to as DBAttributes;

            to.AttributeCategory = from.AttributeCategory;
            to.Attributes = new List<float>(from.Attributes);
        }
    }

    class DBCharacterInfo : Internal.DBEntryBase
    {
        public string Name = "EMPTY";
        public int Level = 1;
        public int Experience = 0;
        public List<DBAttributes> Attributes = new List<DBAttributes>();
        public int CurrentSpecialization;

        public override string ToString()
        {
            return "Name: " + Name;
        }

        public override void Copy(Internal.DBEntryBase _from, Internal.DBEntryBase _to)
        {
            DBCharacterInfo from = _from as DBCharacterInfo;
            DBCharacterInfo to = _to as DBCharacterInfo;

            to.Name = from.Name;
            to.Level = from.Level;
            to.Experience = from.Experience;
            to.Attributes.Clear();
            foreach (DBAttributes attributes in from.Attributes)
            {
                DBAttributes copy = new DBAttributes();
                copy.AttributeCategory = attributes.AttributeCategory;
                copy.Attributes = new List<float>(attributes.Attributes);

                to.Attributes.Add(copy);
            }

            to.CurrentSpecialization = from.CurrentSpecialization;
        }
    }

    [System.Serializable]
    class DBDialogue : Internal.DBEntryBase
    {
        public int SpeakerIdx;
        public string Content;

        public override void Copy(Internal.DBEntryBase _from, Internal.DBEntryBase _to)
        {
            DBDialogue from = _from as DBDialogue;
            DBDialogue to = _to as DBDialogue;

            to.SpeakerIdx = from.SpeakerIdx;
            to.Content = from.Content;
        }
    }
}