﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MAGE.DB.Internal
{
    class CinematicDB : DBBase<int, DBCinematicInfo>
    {
        public CinematicDB() : base("CinematicDB")
        {
        }

        [System.Serializable]
        class Entry
        {
            public int Key;
            public DBCinematicInfo Value;
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

            FileUtil.WriteFile(path, FileUtil.FileName.CinematicDB.ToString(), jsonString);
        }

        public override void Load(string path)
        {
            mDB.Clear();

            string jsonString = FileUtil.ReadFile(path, FileUtil.FileName.CinematicDB.ToString());

            EntryList list = UnityEngine.JsonUtility.FromJson<EntryList>(jsonString);
            foreach (Entry entry in list.List)
            {
                Write(entry.Key, entry.Value);
            }
        }
    }
}
