using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MAGE.DB
{
    [System.Serializable]
    class DBStoryMutatorParams : Internal.DBEntryBase
    {
        public int Param0;
        public int Param1;
        public int Param2;
        public int Param3;

        public override void Copy(Internal.DBEntryBase _from, Internal.DBEntryBase _to)
        {
            DBStoryMutatorParams from = _from as DBStoryMutatorParams;
            DBStoryMutatorParams to = _to as DBStoryMutatorParams;

            to.Param0 = from.Param0;
            to.Param1 = from.Param1;
            to.Param2 = from.Param2;
            to.Param3 = from.Param3;
        }
    }

    [System.Serializable]
    class DBStoryCondition : Internal.DBEntryBase
    {
        public int EventType = -1;
        public int Param = -1;

        public override void Copy(Internal.DBEntryBase _from, Internal.DBEntryBase _to)
        {
            DBStoryCondition from = _from as DBStoryCondition;
            DBStoryCondition to = _to as DBStoryCondition;

            to.EventType = from.EventType;
            to.Param = from.Param;
        }
    }

    [System.Serializable]
    class DBStoryNodeInfo : Internal.DBEntryBase
    {
        public string Name = "NAME";
        public string Description = "DESCRIPTION";
        public DBStoryCondition CompletionCondition = new DBStoryCondition();
        public List<DBStoryMutatorParams> OnActivateChanges = new List<DBStoryMutatorParams>();
        public List<DBStoryMutatorParams> OnCompleteChanges = new List<DBStoryMutatorParams>();

        public override void Copy(Internal.DBEntryBase _from, Internal.DBEntryBase _to)
        {
            DBStoryNodeInfo from = _from as DBStoryNodeInfo;
            DBStoryNodeInfo to = _to as DBStoryNodeInfo;

            to.Name = from.Name;
            to.Description = from.Description;

            from.CompletionCondition.CopyTo(to.CompletionCondition);
            
            to.OnActivateChanges = new List<DBStoryMutatorParams>();
            foreach (DBStoryMutatorParams dBStoryMutatorParams in from.OnActivateChanges)
            {
                DBStoryMutatorParams copy = new DBStoryMutatorParams();
                dBStoryMutatorParams.CopyTo(copy);
                to.OnActivateChanges.Add(copy);
            }

            to.OnCompleteChanges = new List<DBStoryMutatorParams>();
            foreach (DBStoryMutatorParams dBStoryMutatorParams in from.OnCompleteChanges)
            {
                DBStoryMutatorParams copy = new DBStoryMutatorParams();
                dBStoryMutatorParams.CopyTo(copy);
                to.OnCompleteChanges.Add(copy);
            }
        }
    }
}
