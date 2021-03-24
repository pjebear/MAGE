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
    class ProjectileActionComposer
    {
        public static void ComposeAction(CharacterActorController ownerController, ActionInfoBase actionInfo, TargetSelection targetSelection, Map map, out ActionResult result, out Timeline<ActionEvent> timeline)
        {
            List<ActionEvent> timelineEvents = new List<ActionEvent>();

            ActorInteractionBlock casterAnimationBlock = ActionCompositionUtil.CreateOwnerInteractionBlock(ownerController, targetSelection.FocalTarget, actionInfo.AnimationInfo.AnimationId);
            timelineEvents.AddRange(casterAnimationBlock.Events);

            Logger.Assert(actionInfo.ProjectileInfo.ProjectileId != ProjectileId.INVALID, LogTag.GameModes,
                "ProjectileActionComposer", string.Format("No projectile information provided for action {0}", actionInfo.ActionId.ToString()));

            CharacterPosition ownerPosition = map.GetCharacterPosition(ownerController.Character);
            ProjectileSpawnParams projectileSpawnParams = ProjectileUtil.GenerateSpawnParams(
                EncounterFlowControl_Deprecated.MapControl[ownerPosition.Location],
                targetSelection.FocalTarget,
                actionInfo.CastRange.AreaType == AreaType.Expanding ? ProjectilePathType.Arc : ProjectilePathType.Linear,
                actionInfo.ProjectileInfo.ProjectileId);

            ProjectileSpawnBlock projectileSpawnBlock = new ProjectileSpawnBlock(projectileSpawnParams);
            projectileSpawnBlock.SyncronizeTo(AllignmentPosition.Start, 0, casterAnimationBlock, AllignmentPosition.Interaction);
            timelineEvents.AddRange(projectileSpawnBlock.Events);

            List<Character> targets = new List<Character>();
            if (projectileSpawnParams.CollisionWith != null)
            {
                CharacterActorController actorController = projectileSpawnParams.CollisionWith.GetComponent<CharacterActorController>();
                if (actorController != null)
                {
                    targets.Add(actorController.Character);
                }
            }

            List<InteractionResult> interactionResults = InteractionResolver.ResolveInteraction(ownerController.Character, actionInfo, targets, map);
            Dictionary<Character, InteractionResult> targetResults = new Dictionary<Character, InteractionResult>();

            for (int i = 0; i < targets.Count; ++i)
            {
                InteractionResult interactionResult = interactionResults[i];
                targetResults.Add(targets[i], interactionResult);

                //ActorInteractionBlock targetInteractionBlock = ActionCompositionUtil.CreateTargetInteractionBlock(
                //    ownerController
                //    , EncounterFlowControl_Deprecated.CharacterDirector.CharacterActorLookup[targets[i]]
                //    , interactionResult
                //    , projectileSpawnBlock);

                //timelineEvents.AddRange(targetInteractionBlock.Events);
            }

            result = new ActionResult(ownerController.Character, actionInfo,
                new InteractionResult(InteractionUtil.GetOwnerResultTypeFromResults(interactionResults), actionInfo.ActionCost),
                targetResults);

            timeline = new Timeline<ActionEvent>(timelineEvents);
        }
    }
}