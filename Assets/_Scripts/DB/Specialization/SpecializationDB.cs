using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace DB
{
    class SpecializationDB : DBBase<int, DBSpecializations>
    {
        public SpecializationDB() : base ("SpecializationDB")
        {

        }

        [System.Serializable]
        class Entry
        {
            public int Key;
            public DBSpecializations Value;
        }

        [System.Serializable]
        class EntryList
        {
            public List<Entry> List;
        }

        public override void Save()
        {
            EntryList list = new EntryList();
            list.List = new List<Entry>();

            foreach (var keyValuePair in mDB)
            {
                list.List.Add(new Entry() { Key = keyValuePair.Key, Value = keyValuePair.Value });
            }

            string jsonString = UnityEngine.JsonUtility.ToJson(list);

            FileUtil.Write(FileUtil.FolderName.SaveFiles, FileUtil.FileName.SpecializationDB, jsonString);
        }

        public override void Load()
        {
            mDB.Clear();

            string jsonString = FileUtil.Read(FileUtil.FolderName.SaveFiles, FileUtil.FileName.SpecializationDB);

            EntryList list = UnityEngine.JsonUtility.FromJson<EntryList>(jsonString);
            foreach (Entry entry in list.List)
            {
                Write(entry.Key, entry.Value);
            }
        }
    }
}