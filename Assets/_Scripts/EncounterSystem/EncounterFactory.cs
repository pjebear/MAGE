using Common.CharacterEnums;
using Common.EncounterEnums;
using Common.EncounterTypes;
using Common.ProfessionEnums;
using Common.UnitTypes;
using EncounterSystem.MapTypes;
using StorySystem.StoryCast;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using WorldSystem.Character;
using WorldSystem.Common;

namespace EncounterSystem.Factory
{
    static class EncounterFactory
    {
        public static void CheckoutEncounterBluePrint(EncounterScenarioId encounterId, MapLocationId currentLocation, UnitRoster unitRoster, out EncounterBluePrint blueprint)
        {
            string assetId = "demo";// currentLocation.ToString();
            int unitPlacementSide = 0;
            int numUnitsToPlace = 5;
            bool hasUnitPlacementPhase = true;
            CinematicBlueprint openingCinematic = new CinematicBlueprint();
            List<EncounterCinematicEvent> cinematicEvents = new List<EncounterCinematicEvent>();
            Dictionary<int, CharacterBase> cinematicCharacters = new Dictionary<int, CharacterBase>();

            List<EncounterSuccessEvent> winConditions = new List<EncounterSuccessEvent>()
            {
                new EncounterSuccessEvent(EncounterSuccessState.Loss, new TeamDefeatTrigger(UnitGroup.Player)), // Don't lose your players
                new EncounterSuccessEvent(EncounterSuccessState.Loss, new CharacterDeathTrigger((int)StoryCharacterId.Rheinhardt)) // Don't lose your main character
            };

            List<CharacterBase> unitPlacementCharacters = new List<CharacterBase>();
            List<UnitPlacement> staticEnemyCharacters = new List<UnitPlacement>();

            int averageLevel = unitRoster.GetAverageCharacterLevel();

            switch (encounterId)
            {
                case (EncounterScenarioId.Main_Opening):
                    #region Main_Opening
                    winConditions.Add(new EncounterSuccessEvent(EncounterSuccessState.Loss, new TeamDefeatTrigger(UnitGroup.AI))); // Defeat all enemies
                    unitPlacementSide = 0;
                    hasUnitPlacementPhase = false;
                    int rheinhardt = (int)StoryCharacterId.Rheinhardt;
                    int asmund = (int)StoryCharacterId.Asmund;
                    int thanos = (int)StoryCharacterId.Thanos;
                    int magnus = (int)StoryCharacterId.Magnus;

                    cinematicCharacters.Add(rheinhardt, unitRoster.Roster[rheinhardt]);
                    cinematicCharacters.Add(asmund, unitRoster.Roster[asmund]);
                    cinematicCharacters.Add(thanos, unitRoster.Roster[thanos]);
                    cinematicCharacters.Add(magnus, unitRoster.Roster[magnus]);
                    cinematicCharacters.Add(2000, unitRoster.Roster[2000]);
                    cinematicCharacters.Add(2001, unitRoster.Roster[2001]);
                    cinematicCharacters.Add(2002, unitRoster.Roster[2002]);

                    Queue<CinematicComponent> openingCinematics = new Queue<CinematicComponent>();
                    openingCinematics.Enqueue(new UnitRevealCinematic(new List<UnitReveal>()
                    {
                        new UnitReveal(rheinhardt, 35, true, true)
                    }));
                    openingCinematics.Enqueue(new UnitRevealCinematic(new List<UnitReveal>()
                    {
                        new UnitReveal((int)StoryCharacterId.Asmund, 6, true, true)
                    }));

                    Queue<Dialogue> exchange = new Queue<Dialogue>();
                    exchange.Enqueue(new Dialogue(asmund, rheinhardt, "Ahh back at it again, this business of doling out judgement to those who will not take a knee"));
                    openingCinematics.Enqueue(new DialogueCinematic(new HashSet<KeyValuePair<int, bool>>() { new KeyValuePair<int, bool>(asmund, true) }, exchange));
                       
                    openingCinematics.Enqueue(new MovementCinematic(new HashSet<KeyValuePair<int, int>>()
                    {
                        new KeyValuePair<int, int>(asmund, 36)
                    }));

                    exchange = new Queue<Dialogue>();
                    exchange.Enqueue(new Dialogue(rheinhardt, asmund, "You know as well as I Asmund these rebels struck the first blow. Answer must be given or they will only grow bolder"));
                    exchange.Enqueue(new Dialogue(asmund, rheinhardt, "And you know how much I dislike these slave camps. Punished for a generation’s crimes whose bones have long since fell to dust."));
                    exchange.Enqueue(new Dialogue(rheinhardt, asmund, "The only crime committed is taint of their fathers blood coursing through their veins. Curse _BlackDragon_ that his corruption should still seep into this world."));
                    exchange.Enqueue(new Dialogue(asmund, rheinhardt, "(Chuckles). Our conversation follows well worn paths. One day we shall hang up our banners and bicker endlessly in a tavern like proper old men"));
                    openingCinematics.Enqueue(new DialogueCinematic(new HashSet<KeyValuePair<int, bool>>() { new KeyValuePair<int, bool>(asmund, true), new KeyValuePair<int, bool>(rheinhardt, true) }, exchange));

                    openingCinematics.Enqueue(new UnitRevealCinematic(new List<UnitReveal>()
                    {
                        new UnitReveal(thanos, 5, true, false),
                        new UnitReveal(magnus, 6, true, false)
                    }));

                    exchange = new Queue<Dialogue>();
                    exchange.Enqueue(new Dialogue(magnus, asmund, "Something tells me you won't live that long monk. I’m surprised your brittle bones have lasted this long at all. The irony would be complete if you were to fall to one of these low borns you so passionately defend."));
                    openingCinematics.Enqueue(new DialogueCinematic(new HashSet<KeyValuePair<int, bool>>() { new KeyValuePair<int, bool>(asmund, true) }, exchange));

                    openingCinematics.Enqueue(new MovementCinematic(new HashSet<KeyValuePair<int, int>>()
                    {
                        new KeyValuePair<int, int>(thanos, 25),
                        new KeyValuePair<int, int>(magnus, 26)
                    }));

                    exchange = new Queue<Dialogue>();
                    exchange.Enqueue(new Dialogue(rheinhardt, thanos, "What do you have to report Thanos?"));
                    exchange.Enqueue(new Dialogue(thanos, rheinhardt, "Our ranger says this is just a scouting party, they shouldn’t cause us too much trouble. "));
                    exchange.Enqueue(new Dialogue(rheinhardt, thanos, " I’ll stay behind with a few soldier and take care of them. You and _Captain_ can go ahead and begin retaking the settlement. I don’t want to waste time rejoining the host for the siege. You’re in command Thanos until I catch up"));
                    exchange.Enqueue(new Dialogue(magnus, rheinhardt, "Surely you jest putting this pup above me!"));
                    exchange.Enqueue(new Dialogue(rheinhardt, magnus, "Thanos leads with his actions, and not with his tongue, something you would do best to aspire to Magnus. I welcome the chance for him to prove his valor. Dismissed."));
                    openingCinematics.Enqueue(new DialogueCinematic(new HashSet<KeyValuePair<int, bool>>() { new KeyValuePair<int, bool>(asmund, true), new KeyValuePair<int, bool>(rheinhardt, true), new KeyValuePair<int, bool>(thanos, true), new KeyValuePair<int, bool>(magnus, true) }, exchange));

                    openingCinematics.Enqueue(new MovementCinematic(new HashSet<KeyValuePair<int, int>>()
                    {
                        new KeyValuePair<int, int>(thanos, 5)
                    }));

                    openingCinematics.Enqueue(new UnitRevealCinematic(new List<UnitReveal>()
                    {
                        new UnitReveal(thanos, 5, false, false)
                    }));

                    openingCinematics.Enqueue(new MovementCinematic(new HashSet<KeyValuePair<int, int>>()
                    {
                        new KeyValuePair<int, int>(magnus, 6)
                    }));

                    openingCinematics.Enqueue(new UnitRevealCinematic(new List<UnitReveal>()
                    {
                        new UnitReveal(magnus, 6, false, false)
                    }));

                    exchange = new Queue<Dialogue>();
                    exchange.Enqueue(new Dialogue(asmund, rheinhardt, "I’ve told you I don’t like him right?"));
                    exchange.Enqueue(new Dialogue(rheinhardt, asmund, "Many times old friend, many times."));
                    exchange.Enqueue(new Dialogue(asmund, rheinhardt, "Well, let’s go deal with this rabel so this humble monk can find himself a drink."));
                    openingCinematics.Enqueue(new DialogueCinematic(new HashSet<KeyValuePair<int, bool>>() { new KeyValuePair<int, bool>(asmund, true), new KeyValuePair<int, bool>(rheinhardt, true) }, exchange));

                    openingCinematics.Enqueue(new UnitRevealCinematic(new List<UnitReveal>()
                    {
                        new UnitReveal(2000, 4, true, true),
                        new UnitReveal(2001, 5, true, true),
                        new UnitReveal(2002, 6, true, true)
                    }));

                    openingCinematics.Enqueue(new MovementCinematic(new HashSet<KeyValuePair<int, int>>()
                    {
                        new KeyValuePair<int, int>(2000, 24),
                        new KeyValuePair<int, int>(2001, 25),
                        new KeyValuePair<int, int>(2002, 26)
                    }));

                    openingCinematic = new CinematicBlueprint(openingCinematics);

                    staticEnemyCharacters.Add(new UnitPlacement(new CharacterBase(UnitGroup.AI, 0, "Rebel " + 1, (CharacterGender)UnityEngine.Random.Range(0, 1), ProfessionType.Adept, UnityEngine.Random.Range(UnityEngine.Mathf.Max(averageLevel - 1, 1), averageLevel + 1)), new TileIndex(4, 9)));
                    staticEnemyCharacters.Add(new UnitPlacement(new CharacterBase(UnitGroup.AI, 1, "Rebel " + 2, (CharacterGender)UnityEngine.Random.Range(0, 1), ProfessionType.Footman, UnityEngine.Random.Range(UnityEngine.Mathf.Max(averageLevel - 1, 1), averageLevel + 1)), new TileIndex(4, 8)));
                    staticEnemyCharacters.Add(new UnitPlacement(new CharacterBase(UnitGroup.AI, 2, "Rebel " + 3, (CharacterGender)UnityEngine.Random.Range(0, 1), ProfessionType.Footman, UnityEngine.Random.Range(UnityEngine.Mathf.Max(averageLevel - 1, 1), averageLevel + 1)), new TileIndex(6, 8)));
                    staticEnemyCharacters.Add(new UnitPlacement(new CharacterBase(UnitGroup.AI, 3, "Rebel " + 4, (CharacterGender)UnityEngine.Random.Range(0, 1), ProfessionType.Monk, UnityEngine.Random.Range(UnityEngine.Mathf.Max(averageLevel - 1, 1), averageLevel + 1)), new TileIndex(6, 9)));
                    #endregion
                    break;

                case (EncounterScenarioId.Random):
                    winConditions.Add(new EncounterSuccessEvent(EncounterSuccessState.Loss, new TeamDefeatTrigger(UnitGroup.AI))); // Defeat all enemies
                    unitPlacementSide = 0;//UnityEngine.Random.Range(0, 2);
                    int numEnemies = UnityEngine.Random.Range(4, 6);
                    unitPlacementCharacters = unitRoster.Roster.Values.ToList();
                    numUnitsToPlace = Math.Min(5, unitPlacementCharacters.Count);
                    for (int i = 0; i < numEnemies; ++i)
                    {
                        CharacterBase enemy = new CharacterBase(UnitGroup.AI, i, "Lacky " + (i + 1).ToString(), (CharacterGender)UnityEngine.Random.Range(0, 1), (ProfessionType)UnityEngine.Random.Range(0, 4), UnityEngine.Random.Range(UnityEngine.Mathf.Max(averageLevel - 1, 1), averageLevel + 1));
                        staticEnemyCharacters.Add(new UnitPlacement(enemy, TileIndex.Invalid));
                    }
                    break;

                default:
                    UnityEngine.Debug.LogError("No Encounter set up for " + encounterId.ToString());
                    break;
            }

            if (hasUnitPlacementPhase)
            {
                foreach (var character in unitRoster.Roster)
                {
                    // Remove units in placement phase if they are already added in a cinematic phase
                    if (!cinematicCharacters.ContainsKey(character.Key))
                    {
                        unitPlacementCharacters.Add(character.Value);
                    }
                }
            }

            blueprint = new EncounterBluePrint(encounterId, assetId, 
                winConditions,
                openingCinematic,
                cinematicEvents, cinematicCharacters, 
                unitPlacementCharacters, unitPlacementSide, numUnitsToPlace,
                staticEnemyCharacters);
        }
    }
}
