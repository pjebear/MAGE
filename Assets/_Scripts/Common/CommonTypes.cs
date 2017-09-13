
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using WorldSystem.Character;

namespace Common
{
    namespace UnitTypes
    {
        class UnitRoster
        {
            public Dictionary<int, CharacterBase> Roster { get; private set; }
            
            public UnitRoster()
            {
                Roster = new Dictionary<int, CharacterBase>();
            }

            public void AddCharacter(CharacterBase toAdd)
            {
                UnityEngine.Debug.Assert(!Roster.ContainsKey(toAdd.CharacterID), "Character id " + toAdd.CharacterID + " already in roster");
                Roster.Add(toAdd.CharacterID, toAdd);
            }

            public void RemoveCharacter(int characterId)
            {
                UnityEngine.Debug.Assert(Roster.ContainsKey(characterId), "Trying to remove " + characterId + " which is not in roster");
                UnityEngine.Debug.Assert(!Roster[characterId].IsStoryCharacter, "Trying to remove story character. Use story character function");
                Roster.Remove(characterId);
            }

            public void RemoveStoryCharacter(int characterId)
            {
                UnityEngine.Debug.Assert(Roster.ContainsKey(characterId), "Trying to remove " + characterId + " which is not in roster");
                UnityEngine.Debug.Assert(Roster[characterId].IsStoryCharacter, "Trying to remove non story character. Use remove character function");
                Roster.Remove(characterId);
            }

            public int GetAverageCharacterLevel()
            {
                int averageLevel = 0;
                int numCharacters = 0;
                foreach (var character in Roster)
                {
                    averageLevel += character.Value.Level;
                    numCharacters++;
                }
                return numCharacters > 0 ? averageLevel / numCharacters : 0;
            }
        }
    }
    //--------------------------------------------------------------------------
    //--------------------------------------------------------------------------
    namespace AttributeTypes
    {
        using AttributeEnums;

        public class AttributeModifier
        {
            public AttributeIndex AttributeToModify;
            public _GetModifierValue GetModifierValue;
            public ModifierType ModifierType;

            public AttributeModifier(AttributeIndex attributeIndex, ModifierType modType, _GetModifierValue del)
            {
                AttributeToModify = attributeIndex;
                ModifierType = modType;
                GetModifierValue = del;
            }
        }

        public class AttributeContainer
        {
            private Dictionary<AttributeType, float[]> mAttributes;

            public float this[AttributeIndex index]
            {
                get
                {
                    return mAttributes[index.Type][index.Index];
                }
                set
                {
                    mAttributes[index.Type][index.Index] = value;
                }
            }

            public float[] this[AttributeType type] { get { return mAttributes[type]; } }
            public AttributeContainer(Dictionary<AttributeType, float[]> attributes)
            {
                mAttributes = attributes;
            }

            public AttributeContainer(AttributeContainer container)
            {
                mAttributes = new Dictionary<AttributeType, float[]>();
                foreach (var pair in container.mAttributes)
                {
                    mAttributes.Add(pair.Key, new float[pair.Value.Count()]);
                    pair.Value.CopyTo(mAttributes[pair.Key], 0);
                }
            }

            public void AddAttribute(AttributeType type, float[] values)
            {
                mAttributes.Add(type, values);
            }
        }

        public delegate AttributeModifier Effect(int stackCount);

        public delegate float _GetModifierValue(AttributeContainer stats);

        public struct AttributeIndex
        {
            public AttributeType Type;
            public int Index;
            public AttributeIndex(AttributeType type, int index)
            {
                Type = type;
                Index = index;
            }
            public static bool operator ==(AttributeIndex lhs, AttributeIndex rhs)
            {
                return lhs.Type == rhs.Type && lhs.Index == rhs.Index;
            }
            public static bool operator !=(AttributeIndex lhs, AttributeIndex rhs)
            {
                return lhs.Type != rhs.Type || lhs.Index != rhs.Index;
            }
        }
    }

    //--------------------------------------------------------------------------
    //--------------------------------------------------------------------------
    namespace ActionTypes
    {
        using ActionEnums;
        using StatusEnums;
        using AttributeEnums;
        using AttributeTypes;
        using EncounterSystem.Character;
        using EquipmentEnums;
        using StatusTypes;
        using EncounterSystem.Action;
        using EncounterSystem.MapTypes;

        public struct ActionModifier
        {
            public _GetModifierValue GetModifierValue;
            public ModifierType ModifierType;
            public ActionModifier(_GetModifierValue getModifierValue, ModifierType modifierType)
            {
                GetModifierValue = getModifierValue;
                ModifierType = modifierType;
            }
        }

        public struct PostActionResult
        {
            public CharacterManager Actor;
            List<ActionInteractionResult> ActionInteractions;
        }

        public struct ActionInteractionResult
        {
            public ResourceChange ChangedResource;
            public List<StatusEffectIndex> AppliedStatusEffects;

            public CharacterManager Target;
            public InteractionResult InteractionResult;
            public AvoidanceResult AvoidanceResult;
            public bool WasSuccessful;

            public ActionInteractionResult(CharacterManager target)
            {
                Target = target;
                ChangedResource = new ResourceChange(Resource.INVALID, 0);
                AppliedStatusEffects = new List<StatusEffectIndex>();
                InteractionResult = new InteractionResult();
                AvoidanceResult = AvoidanceResult.Invalid;
                WasSuccessful = false;
            }
        }

        public struct ActionTargetSelection
        {
            public ActionTargetType TargetType;
            public CharacterManager CharacterTarget;
            public MapTile TileTarget;

            public ActionTargetSelection(CharacterManager characterTarget)
            {
                TargetType = ActionTargetType.Character;
                CharacterTarget = characterTarget;
                TileTarget = null;
            }
            public ActionTargetSelection(MapTile tileTarget)
            {
                TargetType = ActionTargetType.Tile;
                CharacterTarget = null;
                TileTarget = tileTarget;
            }

            public MapTile GetActionOrigin()
            {
                if (TargetType == ActionTargetType.Character)
                {
                    return CharacterTarget.GetCurrentTile();
                }
                else
                {
                    return TileTarget;
                }
            }
        }

        public struct CharacterActionIndex
        {
            private static int CATEGORY_MULTIPLIER = 1000;
            public ActionContainerCategory ActionType;
            public ActionIndex ActionIndex;
            public CharacterActionIndex(ActionContainerCategory actionType, ActionIndex actionIndex)
            {
                ActionType = actionType;
                ActionIndex = actionIndex;
            }

            public int Hash()
            {
                return (int)ActionType * CATEGORY_MULTIPLIER + (int)ActionIndex;
            }

            public static CharacterActionIndex ConvertHash(int hash)
            {
                int category = hash / CATEGORY_MULTIPLIER;
                int index = hash % CATEGORY_MULTIPLIER;
                if (category < (int)ActionContainerCategory.Attack || category > (int)ActionContainerCategory.Secondary
                    || index <= (int)ActionIndex.INVALID || index >= (int)ActionIndex.NUM )
                {
                    UnityEngine.Debug.LogErrorFormat("Invalid hash recieved for CharacterActionIndex, got {0}", hash);
                }
                return new CharacterActionIndex((ActionContainerCategory)category, (ActionIndex)index);
            }
        }

        public struct ScreenActionPayload
        {
            public CharacterActionIndex ActionIndex;
            public bool CanUse;
            public bool AutoTargetSelection;
            public ResourceChange ActionCost;
            public string ActionName;
            public ScreenActionPayload(string actionName, CharacterActionIndex actionIndex, ResourceChange actionCost, bool canUse, bool autoTargetSelection)
            {
                ActionName = actionName;
                ActionIndex = actionIndex;
                ActionCost = actionCost;
                CanUse = canUse;
                AutoTargetSelection = autoTargetSelection;
            }
        }

        public struct AIActionPayload
        {
            public ActionBase Action;
            public bool CanUse;

            public AIActionPayload(ActionBase action, bool canUse)
            {
                Action = action;
                CanUse = canUse;
            }
        }

        public class ActionContainer
        {
            public Dictionary<ActionContainerCategory, List<ActionIndex>> ActionMap;
            public Dictionary<ActionIndex, List<ActionModifier>> ActionModifierMap;// { get; private set; }
            public Dictionary<ActionIndex, List<StatusEffectIndex>> ActionStatusMap;// { get; private set; }

            public ActionContainer()
            {
                ActionMap = new Dictionary<ActionContainerCategory, List<ActionIndex>>()
            {
                { ActionContainerCategory.Attack, new List<ActionIndex>() },
                { ActionContainerCategory.Primary, new List<ActionIndex>() },
                { ActionContainerCategory.Secondary, new List<ActionIndex>() }
            };
                ActionModifierMap = new Dictionary<ActionIndex, List<ActionModifier>>();
                ActionStatusMap = new Dictionary<ActionIndex, List<StatusEffectIndex>>();
            }
        }

        public struct ResourceChange
        {
            public Resource Resource;
            public float Value;
            public ResourceChange(Resource resource, float value)
            {
                Resource = resource;
                Value = value;
            }
        }

        public struct ActionResourceChangeInformation
        {
            public ResourceChange ResourceChange;
            public ActionBaseType ActionBaseType;
            public ActionEffectType ActionEffectType;
            public bool IsBeneficial;
            // both crit values will be added to from 
            public float CriticalChance; // base crit chance of ability
            public float CriticalMultiplier; // base crit multi of ability
            public Dictionary<PhysicalActionType, float> PhysicalComposition; // Will be querried if Actionbase is physical
            public AllignmentType Allignment; // will be querried if actionBase is magic

            public ActionResourceChangeInformation(Resource resource, float amount, ActionBaseType baseType, ActionEffectType effectType, bool isBeneficial, float critChance, float critMultiplier, AllignmentType allignment = AllignmentType.Unalligned, Dictionary<PhysicalActionType, float> physicalComp = null)
            {
                ResourceChange = new ResourceChange(resource, amount);
                ActionBaseType = baseType;
                IsBeneficial = isBeneficial;
                ActionEffectType = effectType;
                CriticalChance = critChance;
                CriticalMultiplier = critMultiplier;
                Allignment = allignment;
                PhysicalComposition = physicalComp != null ? physicalComp : new Dictionary<PhysicalActionType, float>();
            }
        }

        public struct QueuedActionPayload
        {
            public CharacterManager Actor;
            public ActionBase Action;
            public ActionTargetSelection TargetSelection;
            public MapTile ActionOrigin;

            public QueuedActionPayload(CharacterManager actor, ActionBase action, ActionTargetSelection targetSelection)
            {
                Actor = actor;
                TargetSelection = targetSelection;    
                Action = action;
                ActionOrigin = null;
            } 
        }

        public delegate float _GetActionPriority(CharacterManager caster, List<CharacterManager> targets);

        #region ActionPrerequisites
        public abstract class Prerequisite
        {
            public abstract bool HasPrerequisite(CharacterManager actor);
        }

        public class EquipmentPrerequisite : Prerequisite
        {
            private List<KeyValuePair<EquipmentCategory, int>> mEquipmentPrerequisites;
            public EquipmentPrerequisite(List<KeyValuePair<EquipmentCategory, int>> prerequisites)
            {
                mEquipmentPrerequisites = prerequisites;
            }
            public override bool HasPrerequisite(CharacterManager character)
            {
                bool hasPrereqs = true;
                foreach (var prerequesite in mEquipmentPrerequisites)
                {
                    hasPrereqs &= character.IsEquipped(prerequesite.Key, prerequesite.Value);
                }
                return hasPrereqs;
            }
        }

        public class StatusPrerequisite : Prerequisite
        {
            private List<KeyValuePair<StatusEffectIndex, int>> mStatusPrerequisites; // status, stack count

            public StatusPrerequisite(List<KeyValuePair<StatusEffectIndex, int>> prereqs)
            {
                mStatusPrerequisites = prereqs;
            }

            public override bool HasPrerequisite(CharacterManager character)
            {
                bool hasPrereqs = true;
                foreach (var prereq in mStatusPrerequisites)
                {
                    StatusEffect effect = character.QueryStatusEffect(prereq.Key);
                    hasPrereqs &= effect != null && effect.StackCount >= prereq.Value;
                }
                return hasPrereqs;
            }
        }
        #endregion
    }

    //--------------------------------------------------------------------------
    //--------------------------------------------------------------------------
    namespace StatusTypes
    {
        using AttributeTypes;
        using StatusEnums;

        public enum StatusEffectProgression
        {
            TurnBased,
            ClockBased,
            INVALID
        }


        public class StatusEffect
        {
            public StatusEffectIndex Index;
            public string ToolTip;
            public bool IsHidden;
            public bool DisplayOnApply;
            public int TicksRemaining;
            public int StackCount;
            public bool Beneficial;
            public bool CanStack;
            public bool IsDispelable;
            public StatusEffectProgression EffectProgression;
            public Effect OnApply;
            public Effect PersistentEffect;
            public Effect StatusPhaseEffect;
            public Effect OnRemove;

            public StatusEffect(StatusEffectIndex index, string toolTip, StatusEffectProgression effectProgression, int duration, bool isBeneficial, bool canStack, bool isDispelable,
                Effect persistentEffect = null, Effect statusPhaseEffect = null, Effect onApply = null, Effect onRemove = null,  bool isHidden = false, bool displayOnApply = true)
            {
                Index = index;
                ToolTip = toolTip;
                TicksRemaining = duration;
                Beneficial = isBeneficial;
                CanStack = canStack;
                IsDispelable = isDispelable;
                EffectProgression = effectProgression;
                OnApply = onApply;
                PersistentEffect = persistentEffect;
                StatusPhaseEffect = statusPhaseEffect;
                OnRemove = onRemove;
                IsHidden = isHidden;
                DisplayOnApply = displayOnApply;
            }
        }
    }

    //--------------------------------------------------------------------------
    //--------------------------------------------------------------------------
    namespace EncounterTypes
    {
        using AttributeEnums;
        using CharacterEnums;
        using CinematicEnums;
        using EncounterEnums;
        using EncounterSystem.Character;
        using EncounterSystem.MapTypes;
        using WorldSystem.Character;

        struct UnitPlacement
        {
            public CharacterBase Unit;
            public TileIndex Placement;
            public UnitPlacement(CharacterBase character, TileIndex placement)
            {
                Unit = character;
                Placement = placement;
            }
        }

        class EncounterState
        {
            public EncounterFlowState FlowState;
            public EncounterSuccessState SuccessState { get; private set; }
            public Dictionary<int, CharacterManager> CinematicUnits { get; private set; }
            public HashSet<CharacterManager> PlayerUnits { get { return mEncounterUnits[(int)UnitGroup.Player]; } }
            public HashSet<CharacterManager> EnemyUnits { get { return mEncounterUnits[(int)UnitGroup.AI]; } }
            
            private HashSet<CharacterManager>[] mEncounterUnits;

            public CharacterManager CurrentUnit { get; private set; }
            public int TurnCount { get; private set; }

            public EncounterState()
            {
                mEncounterUnits = new HashSet<CharacterManager>[(int)UnitGroup.NUM];
                CinematicUnits = new Dictionary<int, CharacterManager>();

                for (int i = 0; i < mEncounterUnits.Length; ++i)
                {
                    mEncounterUnits[i] = new HashSet<CharacterManager>();
                }
                TurnCount = 0;
            }

            public void IncrementTurnCount()
            {
                TurnCount++;
            }

            public void SetCinematicUnits(List<CharacterManager> cinematicCharacters)
            {
                foreach (CharacterManager character in cinematicCharacters)
                {
                    CinematicUnits.Add(character.CharacterID, character);
                }
            }

            public void TransferCinematicUnitToBattle(int unitId)
            {
                if (CinematicUnits.ContainsKey(unitId))
                {
                    CharacterManager character = CinematicUnits[unitId];
                    if (character.IsPlayerControlled)
                    {
                        PlayerUnits.Add(character);
                    }
                    else
                    {
                        EnemyUnits.Add(character);
                    }
                }
            }

            public void RemoveUnitFromEncounterLineup(CharacterManager character)
            {
                if (character.IsPlayerControlled)
                {
                    PlayerUnits.Remove(character);
                }
                else
                {
                    EnemyUnits.Remove(character);
                }
            }

            public void SetPlayers(UnitGroup group, List<CharacterManager> players)
            {
                mEncounterUnits[(int)group].Clear();
                mEncounterUnits[(int)group] = new HashSet<CharacterManager>(players);
            }

            public void AddPlayers(UnitGroup group, List<CharacterManager> players)
            {
                foreach (CharacterManager toAdd in players)
                {
                    mEncounterUnits[(int)group].Add(toAdd);
                }
            }

            public List<CharacterManager> GetAllEncounterUnits()
            {
                List<CharacterManager> unitsInEncounter = new List<CharacterManager>();
                for (int i = 0; i < mEncounterUnits.Length; ++i)
                {
                    unitsInEncounter.AddRange(mEncounterUnits[i]);
                }
                return unitsInEncounter;
            }

            public CharacterManager GetCharacter(int characterId)
            {
                CharacterManager query = null;

                if (CinematicUnits.ContainsKey(characterId))
                {
                    query = CinematicUnits[characterId];
                }
                else
                {
                    foreach (CharacterManager character in GetAllEncounterUnits())
                    {
                        if (character.CharacterBase.CharacterID == characterId)
                        {
                            query = character;
                            break;
                        }
                    }
                }

                if (query == null)
                {
                    UnityEngine.Debug.Log(characterId + " not found in encounter lineups");
                }
               
                return query;
            }

            public List<CharacterManager> GetUnitGroup(UnitGroup group)
            {
                return mEncounterUnits[(int)group].ToList();
            }

            public void SetCurrentUnit(CharacterManager character)
            {
                CurrentUnit = character;
            }

            public void SetCurrentSuccessState(EncounterSuccessState successState)
            {
                SuccessState = successState;
            }
        }

        struct EncounterBluePrint
        {
            public EncounterScenarioId ScenarioId;
            public string MapId;

            public List<EncounterSuccessEvent> WinConditions;

            public CinematicBlueprint OpeningCinematic;
            public Dictionary<int, CharacterBase> CinematicCharacters; // instantiated at start until cinematic event occurs
            public List<EncounterCinematicEvent> CinematicEvents;

            public List<CharacterBase> UnitPlacementCharacters;
            public int UnitPlacementSide;
            public int NumUnitsToPlace;

            public List<UnitPlacement> StaticEnemyCharacters;


            public EncounterBluePrint(EncounterScenarioId scenarioId, string mapId, List<EncounterSuccessEvent> winConditions,
                CinematicBlueprint openingCinematic, List<EncounterCinematicEvent> cinematicEvents,
                Dictionary<int, CharacterBase> cinematicCharacters,
                List<CharacterBase> unitPlacementCharacters, int unitPlacementSide, int maxUnits,
                List<UnitPlacement> staticEnemyCharacters)
            {
                ScenarioId = scenarioId;
                MapId = mapId;
                WinConditions = winConditions;
                OpeningCinematic = openingCinematic;
                CinematicEvents = cinematicEvents;
                CinematicCharacters = cinematicCharacters;
                UnitPlacementCharacters = unitPlacementCharacters;
                UnitPlacementSide = unitPlacementSide;
                NumUnitsToPlace = maxUnits;
                StaticEnemyCharacters = staticEnemyCharacters;
            }
        }

        #region EncounterEvents


        abstract class EncounterEvent
        {
            //public bool EventConsumed { get; private set; }
            protected EventTrigger mEventTrigger;
            protected EncounterEvent(EventTrigger trigger)
            {
                mEventTrigger = trigger;
                //EventConsumed = false;
            }
            public bool IsEventTriggered(EncounterState currentState)
            {
                return mEventTrigger.IsTriggered(currentState);
            }

            public virtual bool IsEventValid(EncounterState currentState)
            {
                return true;
            }
        }

        class EncounterSuccessEvent : EncounterEvent
        {
            public EncounterSuccessState EventConsequence;
            public EncounterSuccessEvent(EncounterSuccessState consequence, EventTrigger trigger)
                : base(trigger)
            {
                EventConsequence = consequence;
            }
        }

        class EncounterCinematicEvent : EncounterEvent
        {
            public CinematicBlueprint CinematicBlueprint;
            public EncounterCinematicEvent(CinematicBlueprint cinematic, EventTrigger trigger)
                : base(trigger)
            {
                CinematicBlueprint = cinematic;
            }
        }

        // used my cinematic manager to 
        struct CinematicBlueprint
        {
            public bool EmptyCinematic { get { return CinematicComponents == null; } }
            public Queue<CinematicComponent> CinematicComponents { get; private set; }
            public CinematicBlueprint(Queue<CinematicComponent> cinematicComponents)
            {
                CinematicComponents = cinematicComponents;
            }
        }

        abstract class CinematicComponent
        {
            public CinematicComponentType ComponentType { get; private set; }
            protected CinematicComponent(CinematicComponentType type)
            {
                ComponentType = type;
            }
        }

        struct UnitReveal
        {
            public int CharacterId;
            public int TileId;
            public bool IsReveal;
            public bool AddToEncounter;
            public UnitReveal(int characterId, int tileId, bool isReveal, bool addToEncounter)
            {
                CharacterId = characterId;
                TileId = tileId;
                IsReveal = isReveal;
                AddToEncounter = addToEncounter;
            }
        }

        class UnitRevealCinematic : CinematicComponent
        {
            public List<UnitReveal> UnitsToReveal { get; private set; }
            public UnitRevealCinematic(List<UnitReveal> unitsToReveal)
                : base(CinematicComponentType.UnitReveal)
            {
                UnitsToReveal = new List<UnitReveal>();
                UnitsToReveal.AddRange(unitsToReveal);
            }
        }

        struct Dialogue
        {
            public int SpeakerId;
            public int ListenerId;
            public string Content;
            public Dialogue(int characterId, int listenerId, string content)
            {
                SpeakerId = characterId;
                ListenerId = listenerId;
                Content = content;
            }
        }

        class DialogueCinematic : CinematicComponent
        {
            private HashSet<KeyValuePair<int, bool>> mCharactersInExchange;
            public Queue<Dialogue> DialogueExchange;
            
            public DialogueCinematic(HashSet<KeyValuePair<int, bool>> charactersInExchange, Queue<Dialogue> dialogues)
                : base(CinematicComponentType.Dialogue)
            {
                DialogueExchange = new Queue<Dialogue>(dialogues);
                mCharactersInExchange = new HashSet<KeyValuePair<int, bool>>();
                
                foreach (var character in charactersInExchange)
                {
                    mCharactersInExchange.Add(character);
                }
            }

            public bool IsDialogueValid(EncounterState currentState)
            {
                bool isValid = true;
                foreach (var pair in mCharactersInExchange)
                {
                    CharacterManager character = currentState.GetCharacter(pair.Key);
                    // character is in encounter, character is alive or the character can be dead during exchance (dieing remarks)
                    isValid &= character != null && (character.IsAlive || !pair.Value); 
                }
                return isValid;
            }
        }

        class MovementCinematic : CinematicComponent
        {
            public HashSet<KeyValuePair<int, int>> CharactersToMove; // character id, tile Id

            public MovementCinematic(HashSet<KeyValuePair<int, int>> charactersToMove)
                : base(CinematicComponentType.Movement)
            {
                CharactersToMove = new HashSet<KeyValuePair<int, int>>(charactersToMove);
            }
        }

       
        #endregion

        #region EventTriggers
        abstract class EventTrigger
        {
            protected bool mIsInvalidTrigger = false;
            public abstract bool IsTriggered(EncounterState state);
        }

        /// <summary>
        /// .Character health has reached a certain percentage. Use for enemy kill conditions
        /// </summary>
        class CharacterHealthTrigger : EventTrigger
        {
            private int mCharacterId;
            private float mHealthThreshold;

            public CharacterHealthTrigger(int characterId, float healthPercent)
            {
                mCharacterId = characterId;
                mHealthThreshold = healthPercent;
            }

            public override bool IsTriggered(EncounterState state)
            {
                if (mIsInvalidTrigger)
                    return false;

                CharacterManager character = state.GetCharacter(mCharacterId);
                if (character != null)
                {
                    return character.Resources[(int)Resource.Health] / character.Resources[(int)Resource.MaxHealth] <= mHealthThreshold;
                }
                else
                {
                    UnityEngine.Debug.Log("Character id " + mCharacterId + " for Character Health Trigger not found.");
                    mIsInvalidTrigger = true;
                }
                return false;
            }
        }

        /// <summary>
        /// .Character KO timer has reached zero
        /// </summary>
        class CharacterDeathTrigger : EventTrigger
        {
            private int mCharacterId;

            public CharacterDeathTrigger(int characterId)
            {
                mCharacterId = characterId;
            }

            public override bool IsTriggered(EncounterState state)
            {
                if (mIsInvalidTrigger)
                    return false;

                CharacterManager character = state.GetCharacter(mCharacterId);
                if (character != null)
                {
                    return character.RemovedFromBattle;
                }
                else
                {
                    UnityEngine.Debug.Log("Character id " + mCharacterId + " for Character Death Trigger not found.");
                    mIsInvalidTrigger = true;
                }
                return false;
            }
        }

        class TurnCountTrigger : EventTrigger
        {
            int mTurnCount;
            public TurnCountTrigger(int turnNumber)
            {
                mTurnCount = turnNumber;
            }

            public override bool IsTriggered(EncounterState state)
            {
                return state.TurnCount >= mTurnCount;
            }
        }

        class TeamDefeatTrigger : EventTrigger
        {
            private UnitGroup mTeamSide;
            public TeamDefeatTrigger(UnitGroup side)
            {
                mTeamSide = side;
            }
            public override bool IsTriggered(EncounterState state)
            {
                List<CharacterManager> units = state.GetUnitGroup(mTeamSide);
                foreach (CharacterManager unit in units)
                {
                    if (unit.IsAlive)
                    {
                        return false;
                    }
                }
                return true;
            }
        }
        #endregion
    }
}
