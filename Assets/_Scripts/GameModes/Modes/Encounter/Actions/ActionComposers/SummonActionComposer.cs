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
    class SummonActionComposer
    {
        public static void ComposeAction(CharacterActorController ownerController, ActionInfo actionInfo, TargetSelection targetSelection, Map map, out ActionResult result, out Timeline<ActionEvent> timeline)
        {
            List<ActionEvent> timelineEvents = new List<ActionEvent>();

            ActorInteractionBlock casterAnimationBlock = ActionCompositionUtil.CreateOwnerInteractionBlock(ownerController, targetSelection.FocalTarget, actionInfo.AnimationInfo.AnimationId);
            timelineEvents.AddRange(casterAnimationBlock.Events);

            CharacterPosition ownerPosition = map.GetCharacterPosition(ownerController.Character);
            List<TileIdx> tiles = map.GetTargetedTiles(ownerPosition.Location, targetSelection);

            SummonInfoBase summonInfo = actionInfo as SummonInfoBase;
            Debug.Assert(summonInfo != null);
            foreach (TileIdx tile in tiles)
            {
                CharacterCreateParams summonParams = new CharacterCreateParams();
                summonParams.characterType = CharacterType.Temporary;
                summonParams.characterClass = CharacterClass.MonoSpecialization;
                summonParams.currentSpecialization = summonInfo.SummonType;
                summonParams.name = summonInfo.SummonType.ToString();
                summonParams.appearanceOverrides = new Appearance() { BodyType = BodyType.Bear_0 };

                int characterId = CharacterService.Get().CreateCharacter(summonParams);
                Character summon = CharacterService.Get().GetCharacter(characterId);
                summon.TeamSide = ownerController.Character.TeamSide;

                EncounterFlowControl.CharacterDirector.AddCharacter(
                    summon, 
                    new CharacterPosition(tile, OrientationUtil.Flip(ownerPosition.Orientation)), 
                    ownerController.Character);
            }

            Dictionary<Character, InteractionResult> targetResults = new Dictionary<Character, InteractionResult>();
            result = new ActionResult(ownerController.Character, actionInfo,
                new InteractionResult(InteractionResultType.Hit, actionInfo.ActionCost),
                targetResults);

            timeline = new Timeline<ActionEvent>(timelineEvents);
        }
    }
}
