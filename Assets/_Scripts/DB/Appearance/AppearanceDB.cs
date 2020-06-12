using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DB
{
    class AppearnaceDB : DBBase<int, Character.DBAppearance>
    {
        public AppearnaceDB() : base("AppearnaceDB")
        {
        }

        [System.Serializable]
        class Entry
        {
            public int Key;
            public Character.DBAppearance Value;
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

            FileUtil.WriteFile(path, FileUtil.FileName.AppearanceDB.ToString(), jsonString);
        }

        public override void Load(string path)
        {
            mDB.Clear();

            string jsonString = FileUtil.ReadFile(path, FileUtil.FileName.AppearanceDB.ToString());

            EntryList list = UnityEngine.JsonUtility.FromJson<EntryList>(jsonString);
            foreach (Entry entry in list.List)
            {
                Write(entry.Key, entry.Value);
            }
        }
    }
}
