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
                    MutateProp((PropMutatorType)storyMutatorParams.Param1, storyMutatorParams.Param2, storyMutatorParams.Param3);
                }
                break;
                case StoryMutatorType.Party:
                {
                    MutateParty((PartyMutatorType)storyMutatorParams.Param1, storyMutatorParams.Param2, storyMutatorParams.Param3);
                }
                break;
                case StoryMutatorType.Scenario:
                {
                    MutateScenario((ScenarioMutatorType)storyMutatorParams.Param1, storyMutatorParams.Param2, storyMutatorParams.Param3);
                }
                break;
                case StoryMutatorType.Cinematic:
                {
                    MutateCinematic((CinematicMutatorType)storyMutatorParams.Param1, storyMutatorParams.Param2, storyMutatorParams.Param3);
                }
                break;
                case StoryMutatorType.Encounter:
                {
                    MutateEncounter((EncounterMutatorType)storyMutatorParams.Param1, storyMutatorParams.Param2, storyMutatorParams.Param3);
                }
                break;

                default:
                {
                    Debug.Assert(false);
                }
                break;
            }
        }

        private static void MutateParty(PartyMutatorType partyMutatorType, int param1, int param2)
        {
            Logger.Log(LogTag.Story, TAG, string.Format("::MutateParty() - MutateType [{0}] Param [{1}]",
                partyMutatorType.ToString(), param1));

            switch (partyMutatorType)
            {
                case PartyMutatorType.Item_Add:
                {
                    WorldService.Get().AddToInventory(param1);
                }
                break;
                case PartyMutatorType.Item_Remove:
                {
                    WorldService.Get().RemoveFromInventory(param1);
                }
                break;
                case PartyMutatorType.Party_Add:
                {
                    WorldService.Get().AddCharacterToParty(param1);
                }
                break;
                case PartyMutatorType.Party_Remove:
                {
                    WorldService.Get().RemoveCharacterFromParty(param1);
                }
                break;
            }
        }

        private static void MutateProp(PropMutatorType mutateType, int propId, int param)
        {
            DB.DBPropInfo propInfo = DBService.Get().LoadPropInfo(propId);

            Logger.Log(LogTag.Story, TAG, string.Format("::MutateProp() - MutateType [{0}] PropId [{1}] PropName [{2}] Param [{3}]",
                mutateType.ToString(), propId, propInfo.Name, param));

            switch (mutateType)
            {
                case PropMutatorType.Item_Add:
                {
                    propInfo.Inventory.Add(param);
                }
                break;
                case PropMutatorType.Item_Remove:
                {
                    propInfo.Inventory.Remove(param);
                }
                break;
                case PropMutatorType.Conversation_Add:
                {
                    propInfo.Conversations.Add(param);
                }
                break;
                case PropMutatorType.Conversation_Remove:
                {
                    propInfo.Conversations.Remove(param);
                }
                break;
                case PropMutatorType.Interactible:
                {
                    propInfo.IsInteractable = param == MutatorConstants.TRUE;
                }
                break;
                case PropMutatorType.Activate:
                {
                    propInfo.IsActive = param == MutatorConstants.TRUE;
                }
                break;
                case PropMutatorType.StateChange:
                {
                    propInfo.State = param;
                }
                break;
                default:
                {
                    Debug.Assert(false);
                }
                break;
            }

            DBService.Get().WritePropInfo(propId, propInfo);
        }

        private static void MutateCinematic(CinematicMutatorType mutateType, int cinematicId, int param)
        {
            DB.DBCinematicInfo cinematicInfo = DBService.Get().LoadCinematicInfo(cinematicId);

            Logger.Log(LogTag.Story, TAG, string.Format("::MutateCinematic() - MutateType [{0}] CinematicId [{1}] CinematicName [{2}] Param [{3}]",
                mutateType.ToString(), cinematicId, cinematicInfo.Name, param));

            switch (mutateType)
            {
                case CinematicMutatorType.Activate:
                {
                    cinematicInfo.IsActive = param == MutatorConstants.TRUE;
                }
                break;
                default:
                {
                    Debug.Assert(false);
                }
                break;
            }

            DBService.Get().WriteCinematicInfo(cinematicId, cinematicInfo);
        }

        private static void MutateEncounter(EncounterMutatorType mutateType, int encounterId, int param)
        {
            DB.DBEncounterInfo encounterInfo = DBService.Get().LoadEncounterInfo(encounterId);

            Logger.Log(LogTag.Story, TAG, string.Format("::MutateEncounter() - MutateType [{0}] EncounterId [{1}] EncounterName [{2}] Param [{3}]",
                mutateType.ToString(), encounterId, encounterInfo.Name, param));

            switch (mutateType)
            {
                case EncounterMutatorType.Activate:
                {
                    encounterInfo.IsActive = param == MutatorConstants.TRUE;
                }
                break;
                default:
                {
                    Debug.Assert(false);
                }
                break;
            }

            DBService.Get().WriteEncounterInfo(encounterId, encounterInfo);
        }

        private static void MutateScenario(ScenarioMutatorType mutatorType, int param1, int param2)
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
