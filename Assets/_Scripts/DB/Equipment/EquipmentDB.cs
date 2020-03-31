using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DB
{
    class EquipmentDB : DBBase<int, DBEquipment>
    {
        public EquipmentDB() : base("EquipmentDB")
        {
        }

        [System.Serializable]
        class Entry
        {
            public int Key;
            public DBEquipment Value;
        }

        [System.Serializable]
        class EntryList
        {
            public List<Entry> List;
        }

        public override void Save(string path)
        {
            EntryList list = new EntryList();
            list.List = new List<Entry>();

            foreach (var keyValuePair in mDB)
            {
                list.List.Add(new Entry() { Key = keyValuePair.Key, Value = keyValuePair.Value });
            }

            string jsonString = UnityEngine.JsonUtility.ToJson(list);

            FileUtil.WriteFile(path, FileUtil.FileName.EquipmentDB.ToString(), jsonString);
        }

        public override void Load(string path)
        {
            mDB.Clear();

            string jsonString = FileUtil.ReadFile(path, FileUtil.FileName.EquipmentDB.ToString());

            EntryList list = UnityEngine.JsonUtility.FromJson<EntryList>(jsonString);
            foreach (Entry entry in list.List)
            {
                Write(entry.Key, entry.Value);
            }
        }
    }
}
