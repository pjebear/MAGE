using MAGE.GameSystems;
using MAGE.GameSystems.Actions;
using MAGE.GameSystems.Characters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace MAGE.GameModes.Encounter
{
    class EffectActionComposer
    {
        public static void ComposeAction(CharacterActorController ownerController, ActionInfo actionInfo, TargetSelection targetSelection, Map map, out ActionResult result, out Timeline<ActionEvent> timeline)
        {
            List<ActionEvent> timelineEvents = new List<ActionEvent>();

            ActorInteractionBlock casterAnimationBlock = ActionCompositionUtil.CreateOwnerInteractionBlock(ownerController, targetSelection.FocalTarget, actionInfo.AnimationInfo.AnimationId);
            timelineEvents.AddRange(casterAnimationBlock.Events);

            CharacterPosition ownerPosition = map.GetCharacterPosition(ownerController.Character);
            List<Character> targets = EncounterModule.MapControl.Map.GetTargetedCharacters(ownerPosition, targetSelection);
            List<InteractionResult> interactionResults = InteractionResolver.ResolveInteraction(ownerController.Character, actionInfo, targets, map);
            Dictionary<Character, InteractionResult> targetResults = new Dictionary<Character, InteractionResult>();

            for (int i = 0; i < targets.Count; ++i)
            {
                InteractionResult interactionResult = interactionResults[i];
                targetResults.Add(targets[i], interactionResult);
                CharacterActorController targetController = EncounterModule.CharacterDirector.CharacterActorLookup[targets[i]];

                // Effect Spawn
                Logger.Assert(actionInfo.EffectInfo.EffectId != EffectType.INVALID, LogTag.GameModes,
                   "EffectActionComposer", string.Format("No Effcet information provided for action {0}", actionInfo.ActionId.ToString()));
                EffectSpawnBlock effectSpawnBlock = new EffectSpawnBlock(actionInfo.EffectInfo.EffectId, targetController.transform);
                effectSpawnBlock.SyncronizeTo(AllignmentPosition.Start, 0, casterAnimationBlock, AllignmentPosition.Interaction);
                timelineEvents.AddRange(effectSpawnBlock.Events);

                // Time the state change to effect spawn

                ActorInteractionBlock targetInteractionBlock = ActionCompositionUtil.CreateTargetInteractionBlock(
                    ownerController
                    , targetController
                    , interactionResult
                    , effectSpawnBlock);

                timelineEvents.AddRange(targetInteractionBlock.Events);
            }

            result = new ActionResult(ownerController.Character, actionInfo,
                new InteractionResult(InteractionUtil.GetOwnerResultTypeFromResults(interactionResults), actionInfo.ActionCost),
                targetResults);

            timeline = new Timeline<ActionEvent>(timelineEvents);
        }
    }
}
