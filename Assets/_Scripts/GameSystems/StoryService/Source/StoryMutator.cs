using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace MAGE.GameSystems.Story.Internal
{
    static class StoryMutator
    {
        private static string TAG = "StoryMutator";

        public static void MutateGame(StoryMutatorParams storyMutatorParams)
        {
            switch (storyMutatorParams.StoryMutatorType)
            {
                case StoryMutatorType.Prop:
                {
                    MutateProp(storyMutatorParams);
                }
                break;
                case StoryMutatorType.Party:
                {
                    MutateParty(storyMutatorParams);
                }
                break;
                case StoryMutatorType.Scenario:
                {
                    MutateScenario(storyMutatorParams);
                }
                break;
                case StoryMutatorType.Cinematic:
                {
                    MutateCinematic(storyMutatorParams);
                }
                break;
                case StoryMutatorType.Encounter:
                {
                    MutateEncounter(storyMutatorParams);
                }
                break;
                case StoryMutatorType.LootTable:
                {
                    MutateLootTable(storyMutatorParams);
                }
                break;

                default:
                {
                    Debug.Assert(false);
                }
                break;
            }
        }

        private static void MutateParty(StoryMutatorParams storyMutatorParams)
        {
            switch ((PartyMutateType)storyMutatorParams.GetParam((int)PartyParam.MutateType))
            {
                case PartyMutateType.Item:
                {
                    int itemId = storyMutatorParams.GetParam((int)PartyParam.Param1);
                    int itemAmount = storyMutatorParams.GetParam((int)PartyParam.Param2);
                    bool addItem = storyMutatorParams.GetParam((int)PartyParam.Param3) == MutatorConstants.ADD;
                    Logger.Log(LogTag.Story, TAG, string.Format("::MutateParty() - {0} Item to Inventory. ItemId[{1}]"
                        , addItem ? "Add" : "Remove", itemId));
                    if (addItem)
                    {
                        WorldService.Get().AddToInventory(itemId, itemAmount);
                    }
                    else
                    {
                        WorldService.Get().RemoveFromInventory(itemId, itemAmount);
                    }
                }
                break;
                case PartyMutateType.PartyMember:
                {
                    int characterId = storyMutatorParams.GetParam((int)PartyParam.Param1);
                    bool addCharacter = storyMutatorParams.GetParam((int)PartyParam.Param2) == MutatorConstants.ADD;
                    Logger.Log(LogTag.Story, TAG, string.Format("::MutateParty() - {0} Character to Part. CharacterId[{1}]"
                        , addCharacter ? "Add" : "Remove", characterId));
                    if (addCharacter)
                    {
                        WorldService.Get().AddCharacterToParty(characterId);
                    }
                    else
                    {
                        WorldService.Get().RemoveCharacterFromParty(characterId);
                    }
                }
                break;
                default:
                    Debug.Assert(false);
                    break;
            }
        }

        private static void MutateProp(StoryMutatorParams storyMutatorParams)
        {
            DB.DBPropInfo propInfo = DBService.Get().LoadPropInfo(storyMutatorParams.GetParam((int)PropMutateParam.PropId));

            switch ((PropMutateType)storyMutatorParams.GetParam((int)PropMutateParam.MutateType))
            {
                case PropMutateType.Item:
                {
                    int itemId = storyMutatorParams.GetParam((int)PropMutateParam.Param1);
                    bool addItem = storyMutatorParams.GetParam((int)PropMutateParam.Param2) == MutatorConstants.ADD;

                    Logger.Log(LogTag.Story, TAG, string.Format("::MutateProp() - {0} Item to Prop. Item[{1}] Prop[{2}]"
                        , addItem ? "Add" : "Remove", itemId, propInfo.Name));

                    if (addItem)
                    {
                        propInfo.Inventory.Add(itemId);
                    }
                    else
                    {
                        propInfo.Inventory.Remove(itemId);
                    }
                }
                break;
                case PropMutateType.Conversation:
                {
                    int conversationId = storyMutatorParams.GetParam((int)PropMutateParam.Param1);
                    bool add = storyMutatorParams.GetParam((int)PropMutateParam.Param2) == MutatorConstants.ADD;

                    Logger.Log(LogTag.Story, TAG, string.Format("::MutateProp() - {0} Conversation to Prop. Conversation[{1}] Prop[{2}]"
                        , add ? "Add" : "Remove", ((ConversationId)conversationId).ToString(), propInfo.Name));

                    if (add)
                    {
                        propInfo.Conversations.Add(conversationId);
                    }
                    else
                    {
                        propInfo.Conversations.Remove(conversationId);
                    }
                }
                break;

                case PropMutateType.Interactible:
                {
                    bool interactible = storyMutatorParams.GetParam((int)PropMutateParam.Param1) == MutatorConstants.TRUE;
                    Logger.Log(LogTag.Story, TAG, string.Format("::MutateProp() - Setting Prop [{0}] Interactibility to [{1}]"
                        , propInfo.Name, interactible ? "TRUE" : "FALSE"));
                    propInfo.IsInteractable = interactible;
                }
                break;
                case PropMutateType.Activate:
                {
                    bool active = storyMutatorParams.GetParam((int)PropMutateParam.Param1) == MutatorConstants.TRUE;
                    Logger.Log(LogTag.Story, TAG, string.Format("::MutateProp() - Setting Prop [{0}] Active to [{1}]"
                        , propInfo.Name, active ? "TRUE" : "FALSE"));
                    propInfo.IsActive = active;
                }
                break;
                case PropMutateType.StateChange:
                {
                    int state = storyMutatorParams.GetParam((int)PropMutateParam.Param1);
                    Logger.Log(LogTag.Story, TAG, string.Format("::MutateProp() - Setting Prop [{0}] State to [{1}]"
                        , propInfo.Name, state));
                    propInfo.State = state;
                }
                break;
                default:
                {
                    Debug.Assert(false);
                }
                break;
            }

            DBService.Get().WritePropInfo(propInfo.Id, propInfo);
        }

        private static void MutateCinematic(StoryMutatorParams storyMutatorParams)
        {
            DB.DBCinematicInfo cinematicInfo = DBService.Get().LoadCinematicInfo(storyMutatorParams.GetParam((int)CinematicParam.CinematicId));

            switch ((CinematicMutateType)storyMutatorParams.GetParam((int)CinematicParam.MutateType))
            {
                case CinematicMutateType.Active:
                {
                    bool active = storyMutatorParams.GetParam((int)CinematicParam.Param1) == MutatorConstants.TRUE;
                    Logger.Log(LogTag.Story, TAG, string.Format("::MutateCinematic() - Setting Cinematic [{0}] to [{1}]"
                        , cinematicInfo.Name, active ? "ACTIVE" : "INACTIVE"));
                    cinematicInfo.IsActive = active;
                }
                break;
                default:
                {
                    Debug.Assert(false);
                }
                break;
            }

            DBService.Get().WriteCinematicInfo(cinematicInfo.Id, cinematicInfo);
        }

        private static void MutateEncounter(StoryMutatorParams storyMutatorParams)
        {
            DB.DBEncounterInfo encounterInfo = DBService.Get().LoadEncounterInfo(storyMutatorParams.GetParam((int)EncounterParam.EncounterId));

            switch ((EncounterMutateType)storyMutatorParams.GetParam((int)EncounterParam.MutateType))
            {
                case EncounterMutateType.Active:
                {
                    bool active = storyMutatorParams.GetParam((int)EncounterParam.Param1) == MutatorConstants.TRUE;
                    Logger.Log(LogTag.Story, TAG, string.Format("::MutateEncounter() - Setting Encounter [{0}] to [{1}]"
                        , encounterInfo.Name, active ? "ACTIVE" : "INACTIVE"));
                    encounterInfo.IsActive = active;
                }
                break;
                default:
                {
                    Debug.Assert(false);
                }
                break;
            }

            DBService.Get().WriteEncounterInfo(encounterInfo.Id, encounterInfo);
        }

        private static void MutateLootTable(StoryMutatorParams storyMutatorParams)
        {
            Loot.LootTable lootTable = WorldService.Get().DEBUG_GetLootTable();
            switch ((LootTableMutateType)storyMutatorParams.GetParam((int)LootTableParam.MutateType))
            {
                case LootTableMutateType.Level:
                {
                    
                }
                break;
                case LootTableMutateType.Encounter:
                {
                    
                }
                break;
                case LootTableMutateType.Mob:
                {
                    Mobs.MobId mobId = (Mobs.MobId)storyMutatorParams.GetParam((int)LootTableParam.EntryId);
                    int itemId = storyMutatorParams.GetParam((int)LootTableParam.ItemId);
                    int chance = storyMutatorParams.GetParam((int)LootTableParam.Chance);
                    int amount = storyMutatorParams.GetParam((int)LootTableParam.Amount);
                    int varience = storyMutatorParams.GetParam((int)LootTableParam.Varience);
                    bool add = storyMutatorParams.GetParam((int)LootTableParam.Add) == MutatorConstants.ADD;

                    if (add)
                    {
                        Loot.LootInfo entry = new Loot.LootInfo();
                        entry.LootType = Loot.LootType.Item;
                        entry.Value = itemId;
                        entry.DropChance = chance;
                        entry.DropAmount = amount;
                        entry.DropVarience = varience;

                        Logger.Log(LogTag.Story, TAG, string.Format("::MutateLootTable() - Adding [{0}] to Mob [{1}]"
                        , itemId, mobId.ToString()));

                        if (!lootTable.MobLoot.ContainsKey(mobId)) lootTable.MobLoot.Add(mobId, new Loot.TableEntry());
                        if (lootTable.MobLoot[mobId].Loot.Find(x => x.Value == itemId) == null)
                        {
                            lootTable.MobLoot[mobId].Loot.Add(entry);
                        }
                        else
                        {
                            Debug.Assert(false);
                        }
                    }
                    else
                    {
                        Logger.Log(LogTag.Story, TAG, string.Format("::MutateLootTable() - Removing [{0}] from Mob [{1}]"
                        , itemId, mobId.ToString()));

                        if (lootTable.MobLoot.ContainsKey(mobId))
                        {
                            lootTable.MobLoot[mobId].Loot.RemoveAll(x => x.LootType == Loot.LootType.Item && x.Value == itemId);   
                        }
                    }
                }
                break;
                default:
                {
                    Debug.Assert(false);
                }
                break;
            }
        }

        private static void MutateScenario(StoryMutatorParams storyMutatorParams)
        {
            UnityEngine.Debug.Assert(false);
            //ScenarioId scenarioId = (ScenarioId)param1;
            
            //Logger.Log(LogTag.Story, TAG, string.Format("::MutateScenario() - MutateType [{0}] Scenario [{1}] Param[{2}]",
            //    mutatorType.ToString(), scenarioId.ToString(), param2));

            //switch (mutatorType)
            //{
            //    case ScenarioMutatorType.Item_Add:
            //    {
            //        WorldService.Get().AddToInventory(param1);
            //    }
            //    break;
            //    case PartyMutatorType.Item_Remove:
            //    {
            //        WorldService.Get().RemoveFromInventory(param1);
            //    }
            //    break;
            //    case PartyMutatorType.Party_Add:
            //    {
            //        WorldService.Get().AddCharacterToParty(param1);
            //    }
            //    break;
            //    case PartyMutatorType.Party_Remove:
            //    {
            //        WorldService.Get().RemoveCharacterFromParty(param1);
            //    }
            //    break;
            //}
        }
    }
}
