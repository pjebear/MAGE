﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace DB
{
    class TeamDB : DBBase<TeamSide, DBTeam>
    {
        protected Dictionary<int, TeamSide> mCharacterTeamLookup;

        public TeamDB() : base("TeamDB")
        {
        }

        [System.Serializable]
        class Entry
        {
            public TeamSide Key;
            public DBTeam Value;
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

            FileUtil.WriteFile(path, FileUtil.FileName.TeamDB.ToString(), jsonString);
        }

        public override void Load(string path)
        {
            mDB.Clear();

            string jsonString = FileUtil.ReadFile(path, FileUtil.FileName.TeamDB.ToString());

            EntryList list = UnityEngine.JsonUtility.FromJson<EntryList>(jsonString);
            foreach (Entry entry in list.List)
            {
                Write(entry.Key, entry.Value);
            }
        }
    }
}



