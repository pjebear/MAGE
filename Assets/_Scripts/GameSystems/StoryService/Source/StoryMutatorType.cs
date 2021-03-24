using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MAGE.GameSystems.Story.Internal
{
    static class MutatorConstants
    {
        public const int OPEN = 1;
        public const int CLOSE = 0;
        public const int TRUE = 1;
        public const int FALSE = 0;
        public const int ADD = 1;
        public const int REMOVE = 0;
        public const int ALL = -1;
    }


    enum StoryMutatorType
    {
        Cinematic,
        Encounter,
        LootTable,
        Prop,
        Scenario,
        Party
    }

    enum PropMutateParam
    {
        PropId,
        MutateType,
        Param1,
        Param2,
        NUM
    }

    enum PropMutateType
    {
        Item,
        Conversation,
        Interactible,
        Activate,
        StateChange,
        NUM
    }

    enum CinematicParam
    {
        CinematicId,
        MutateType,
        Param1,
        NUM
    }

    enum CinematicMutateType
    {
        Active
    }

    enum EncounterParam
    {
        EncounterId,
        MutateType,
        Param1,
        NUM
    }

    enum EncounterMutateType
    {
        Active
    }

    enum LootTableMutateType
    {
        Level,
        Encounter,
        Mob
    }
    enum LootTableParam
    {
        MutateType,
        EntryId,
        ItemId,
        Add,
        Chance,
        Amount,
        Varience,
        NUM
    }

    enum PartyMutateType
    {
        Item,
        PartyMember
    }
    enum PartyParam
    {
        MutateType,
        Param1,
        Param2,
        Param3,
        NUM
    }

    class StoryMutatorParams
    {
        public StoryMutatorType StoryMutatorType;
        public List<int> Params = new List<int>();
        public int GetParam(int index) { return Params[index]; }
        public void SetParam(int index, int value) { Params[index] = value; }

        public StoryMutatorParams(StoryMutatorType mutatorType, int paramsSize)
        {
            StoryMutatorType = mutatorType;
            Params = Enumerable.Repeat(int.MinValue, paramsSize).ToList();
        }

        public StoryMutatorParams(StoryMutatorType mutatorType, List<int> mutateParams)
        {
            StoryMutatorType = mutatorType;
            Params = new List<int>(mutateParams);
        }

        public static StoryMutatorParams LootTableParams(int mutateType, int entryId, int itemId, int add, int dropChance, int dropAmount, int dropVarience)
        {
            StoryMutatorParams mutateParams = new StoryMutatorParams(StoryMutatorType.LootTable, (int)LootTableParam.NUM);

            mutateParams.SetParam((int)LootTableParam.MutateType, mutateType);
            mutateParams.SetParam((int)LootTableParam.EntryId, entryId);
            mutateParams.SetParam((int)LootTableParam.ItemId, itemId);
            mutateParams.SetParam((int)LootTableParam.Add, add);
            mutateParams.SetParam((int)LootTableParam.Chance, dropChance);
            mutateParams.SetParam((int)LootTableParam.Amount, dropAmount);
            mutateParams.SetParam((int)LootTableParam.Varience, dropVarience);

            return mutateParams;
        }

        public static StoryMutatorParams PropMutateParams(int propId, int mutateType, int param1 = -1, int param2 = -1)
        {
            StoryMutatorParams mutateParams = new StoryMutatorParams(StoryMutatorType.Prop, (int)PropMutateParam.NUM);

            mutateParams.SetParam((int)PropMutateParam.PropId, propId);
            mutateParams.SetParam((int)PropMutateParam.MutateType, mutateType);
            mutateParams.SetParam((int)PropMutateParam.Param1, param1);
            mutateParams.SetParam((int)PropMutateParam.Param2, param2);

            return mutateParams;
        }

        public static StoryMutatorParams PartyMutateParams(int mutateType, int param1 = -1, int param2 = -1)
        {
            StoryMutatorParams mutateParams = new StoryMutatorParams(StoryMutatorType.Party, (int)PartyParam.NUM);

            mutateParams.SetParam((int)PartyParam.MutateType, mutateType);
            mutateParams.SetParam((int)PartyParam.Param1, param1);
            mutateParams.SetParam((int)PartyParam.Param2, param2);

            return mutateParams;
        }

        public static StoryMutatorParams AddPartyItem(int itemId, int amount)
        {
            StoryMutatorParams mutateParams = new StoryMutatorParams(StoryMutatorType.Party, (int)PartyParam.NUM);

            mutateParams.SetParam((int)PartyParam.MutateType, (int)PartyMutateType.Item);
            mutateParams.SetParam((int)PartyParam.Param1, itemId);
            mutateParams.SetParam((int)PartyParam.Param2, amount);
            mutateParams.SetParam((int)PartyParam.Param3, MutatorConstants.ADD);

            return mutateParams;
        }

        public static StoryMutatorParams RemovePartyItem(int itemId, int amount = MutatorConstants.ALL)
        {
            StoryMutatorParams mutateParams = new StoryMutatorParams(StoryMutatorType.Party, (int)PartyParam.NUM);

            mutateParams.SetParam((int)PartyParam.MutateType, (int)PartyMutateType.Item);
            mutateParams.SetParam((int)PartyParam.Param1, itemId);
            mutateParams.SetParam((int)PartyParam.Param2, amount);
            mutateParams.SetParam((int)PartyParam.Param3, MutatorConstants.REMOVE);

            return mutateParams;
        }

        public static StoryMutatorParams EncounterMutateParams(int encounterId, int mutateType, int param)
        {
            StoryMutatorParams mutateParams = new StoryMutatorParams(StoryMutatorType.Encounter, (int)EncounterParam.NUM);

            mutateParams.SetParam((int)EncounterParam.EncounterId, encounterId);
            mutateParams.SetParam((int)EncounterParam.MutateType, mutateType);
            mutateParams.SetParam((int)EncounterParam.Param1, param);

            return mutateParams;
        }

        public static StoryMutatorParams CinematicMutatorParams(int cinematicId, int mutateType, int param)
        {
            StoryMutatorParams mutateParams = new StoryMutatorParams(StoryMutatorType.Cinematic, (int)CinematicParam.NUM);

            mutateParams.SetParam((int)CinematicParam.CinematicId, cinematicId);
            mutateParams.SetParam((int)CinematicParam.MutateType, mutateType);
            mutateParams.SetParam((int)CinematicParam.Param1, param);

            return mutateParams;
        }
    }
}
