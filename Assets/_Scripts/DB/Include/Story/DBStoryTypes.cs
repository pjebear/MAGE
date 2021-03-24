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
        public int MutateType;
        public List<int> Params = new List<int>();

        public override void Copy(Internal.DBEntryBase _from, Internal.DBEntryBase _to)
        {
            DBStoryMutatorParams from = _from as DBStoryMutatorParams;
            DBStoryMutatorParams to = _to as DBStoryMutatorParams;

            to.MutateType = from.MutateType;
            to.Params.AddRange(from.Params);
        }
    }

    [System.Serializable]
    class DBStoryObjective : Internal.DBEntryBase
    {
        public int EventType = -1;
        public int Param = -1;
        public int Progress = 0;
        public int Goal = 1;

        public override void Copy(Internal.DBEntryBase _from, Internal.DBEntryBase _to)
        {
            DBStoryObjective from = _from as DBStoryObjective;
            DBStoryObjective to = _to as DBStoryObjective;

            to.EventType = from.EventType;
            to.Param = from.Param;
            to.Progress = from.Progress;
            to.Goal = from.Goal;
        }
    }

    [System.Serializable]
    class DBStoryNodeInfo : Internal.DBEntryBase
    {
        public string Name = "NAME";
        public string Description = "DESCRIPTION";
        public List<DBStoryObjective> CompletionObjectives = new List<DBStoryObjective>();
        public List<DBStoryMutatorParams> OnActivateChanges = new List<DBStoryMutatorParams>();
        public List<DBStoryMutatorParams> OnCompleteChanges = new List<DBStoryMutatorParams>();

        public override void Copy(Internal.DBEntryBase _from, Internal.DBEntryBase _to)
        {
            DBStoryNodeInfo from = _from as DBStoryNodeInfo;
            DBStoryNodeInfo to = _to as DBStoryNodeInfo;

            to.Name = from.Name;
            to.Description = from.Description;

            to.CompletionObjectives = new List<DBStoryObjective>();
            foreach (DBStoryObjective storyCondition in from.CompletionObjectives)
            {
                DBStoryObjective copy = new DBStoryObjective();
                storyCondition.CopyTo(copy);
                to.CompletionObjectives.Add(copy);
            }

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
