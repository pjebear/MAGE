using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Common.AttributeEnums;
using Common.AttributeTypes;
using Common.StatusEnums;
using Common.StatusTypes;

using Common.ActionEnums;
using Common.ActionTypes;

using WorldSystem.Character;
namespace EncounterSystem.Character
{
    using Action;
    using StatusEffects;

    using Common.EquipmentEnums;
    using Common.EquipmentTypes;
    using EventSystem;
    using Managers;
    using Screens.Payloads;
    using MapTypes;

    public class CharacterManager : MonoBehaviour
    {
        private OffenseManager mOffenseManager = null;
        private DefenseManager mDefenseManager = null;
        private ResourceManager mResourceManager = null;
        private StatusManager mStatusManager = null;
        private MovementController mMoveController = null;
        private ScreenTextManager mScreenTextManager = null;
        private List<Aura> mAuras = null;

        public CharacterBase CharacterBase { get; private set; }
        public GameObject LeftHand;
        public GameObject RightHand;
        public GameObject ChestDisplay;
        public float[] Stats { get { return CharacterBase.Attributes[AttributeType.Stat]; } }
        public float[] Resources { get { return mResourceManager.EncounterResources; } }
        public float[] Status { get { return CharacterBase.Attributes[AttributeType.Status]; } }
        public float[] EXPOSED_STATS;
        public float[] EXPOSED_RESOURCES;

        public bool IsPlayerControlled { get { return CharacterBase.OwnedBy == Common.CharacterEnums.UnitGroup.Player; } }
        public int CharacterID { get { return CharacterBase.CharacterID; } }

        public bool CanAct { get { return !mOffenseManager.HasActed && !mOffenseManager.IsChargingAction && CharacterBase.Attributes[AttributeType.Status][(int)StatusType.Disarmed] == 0; } }
        public bool HasActed { private get { return mOffenseManager.HasActed; } set { mOffenseManager.HasActed = value; } }
        public bool IsChargingAction { private get { return mOffenseManager.IsChargingAction; } set { mOffenseManager.IsChargingAction = value; } }

        public bool CanMove { get { return !mMoveController.HasMoved && CharacterBase.Attributes[AttributeType.Status][(int)StatusType.Rooted] == 0; } }
        public bool HasMoved { private get { return mMoveController.HasMoved; } set { mMoveController.HasMoved = value; } }
        public bool IsAlive { get { return mResourceManager != null ? mResourceManager.IsAlive : true; } }
        public bool IsKO { get { return CharacterBase.Attributes[AttributeType.Status][(int)StatusType.KO] > 0; } }
        public bool RemovedFromBattle { get { return !IsAlive && !IsKO; } }

        protected bool TurnOver;
        public bool FinishedTurn { get { return TurnOver || (HasActed || IsChargingAction) && HasMoved || !IsAlive; } }

        private void Awake()
        {
            mAuras = new List<Aura>();
        }

        public void Initialize(CharacterBase character)
        {
            CharacterBase = character;
            // get unit model
            // TODO: set animation models for holding the weapons
            List<string> heldIds = CharacterBase.GetHeldEquipmentModles();

            for (int i = 0; i < heldIds.Count; i++)
            {
                if (heldIds[i] != "")
                {
                    Instantiate(UnityEngine.Resources.Load("Weapons/" + heldIds[i]), i == 0 ? LeftHand.transform : RightHand.transform);
                }
            }

            Material newMat = UnityEngine.Resources.Load(IsPlayerControlled ? "PlayerMaterial" : "AIMaterial", typeof(Material)) as Material;

            ChestDisplay.GetComponent<MeshRenderer>().material = newMat;

            mMoveController = GetComponent<MovementController>();
        }

        public void BeginEncounter()
        {
            Debug.Assert(CharacterBase != null, "Attempting to Initialize before PreEncounterSetup is called");

            mOffenseManager = new OffenseManager();
            mDefenseManager = new DefenseManager();
            mResourceManager = new ResourceManager();
            mStatusManager = new StatusManager();
            mScreenTextManager = new ScreenTextManager();

            mScreenTextManager.Initialize(GetComponent<Screen.ScreenTextDisplayQueue>());
            mOffenseManager.Initialize(CharacterBase.GetActionContainer(), CharacterBase.Attributes, CharacterBase.GetHeldWeapons());
            mDefenseManager.Initialize(CharacterBase.Attributes);
            mResourceManager.Initialize(CharacterBase.Attributes);
            mStatusManager.Initialize( this, CharacterBase, mScreenTextManager);

            //----------------TEMP-------------------------------------

            EXPOSED_STATS = Stats;
            EXPOSED_RESOURCES = mResourceManager.EncounterResources;
            //---------------------------------------------------------

            // add auras
            foreach (AuraIndex aura in CharacterBase.GetAuras())
            {
                mAuras.Add(StatusEffectFactory.CheckoutStatusAura(aura, this));
            }

            // Event Listeners
            foreach (EventListenerIndex listener in CharacterBase.GetEventListeners())
            {
                EventSystemFactory.CheckoutListenerHandler(listener, transform);
            }
        }

        public bool IsEquipped(EquipmentType type, int equipmentIndex)
        {
            return CharacterBase.IsEquipped(type, equipmentIndex);
        }

        public MapTile GetCurrentTile()
        {
            return mMoveController.CurrentTile;
        }

        public void UpdateCurrentTile(MapTile tile, bool placeAtTile)
        {
            Debug.Assert(tile.GetCharacterOnTile() == null, "Assigning character to tile owned by another character");
            mMoveController.CurrentTile = tile;
            tile.SetCharacterOnTile(this);
            if (placeAtTile)
            {
                tile.PlaceCharacterAtTileCenter();
            }
        }

        public UnitPanelPayload GetUnitPanelPayload()
        {
            UnitPanelPayload toReturn = CharacterBase.GetUnitPanelPayload();
            if (mResourceManager != null)
            {
                toReturn.CurrentHealth = mResourceManager.EncounterResources[(int)Resource.Health];
                toReturn.MaxHealth = mResourceManager.EncounterResources[(int)Resource.MaxHealth];
                toReturn.CurrentEndurance = mResourceManager.EncounterResources[(int)Resource.Endurance];
                toReturn.MaxEndurance = mResourceManager.EncounterResources[(int)Resource.MaxEndurance];
                toReturn.CurrentMana = mResourceManager.EncounterResources[(int)Resource.Mana];
                toReturn.MaxMana = mResourceManager.EncounterResources[(int)Resource.MaxMana];
                toReturn.CurrentClock = mResourceManager.ClockGuague;
                toReturn.MaxClock = 100;
            }
            
            return toReturn;
        }

        #region ActionInteraction

        //-----------------------------------------------------------------------------
        public List<WeaponBase> GetHeldWeapons()
        {
            return CharacterBase.GetHeldWeapons();
        }

        //-----------------------------------------------------------------------------
        public bool CanPerformAction(ActionBase action)
        {
            return IsAlive && mOffenseManager.CanPerformAction(action);
        }

        //-----------------------------------------------------------------------------
        public int GetChargeSpeedForAction(ActionBase action)
        {
            float chargeTimeModifier = 1f + (CharacterBase.Attributes[AttributeType.Stat][(int)SecondaryStat.Attunement] / 100f);
            chargeTimeModifier += CharacterBase.Attributes[AttributeType.Stat][(int)TertiaryStat.CastSpeed];

            return (int)(action.ActionInfo.ChargeTime * chargeTimeModifier);
        }

        //-----------------------------------------------------------------------------
        public void RemoveActionResourceCost(ActionBase action)
        {
            ResourceChange cost = mOffenseManager.GetActionResourceCost(action);

            if (cost.Resource != Resource.INVALID)
            {
                mResourceManager.ModifyCurrentResource(cost);
            }
        }

        //-----------------------------------------------------------------------------
        public Dictionary<ActionContainerCategory, List<ScreenActionPayload>> GetScreenActionPayload()
        {
            return mOffenseManager.GetScreenActionPayloads(this);
        }

        //-----------------------------------------------------------------------------
        public Dictionary<ActionRootEffect, List<ActionBase>> GetAIActionPayload()
        {
            return mOffenseManager.GetAIActionPayloads(this);
        }

        //-----------------------------------------------------------------------------
        public ActionBase GetCharacterAction(CharacterActionIndex index)
        {
            return mOffenseManager.GetCharacterAction(index);
        }

        //-----------------------------------------------------------------------------
        public void AttemptActionResourceChange(ActionResourceChangeInformation resourceChangeInfo, ref ActionInteractionResult interactionResult, ActionOrientation orientation = ActionOrientation.NONE)
        {
            if (IsAlive)
            {
                if (!resourceChangeInfo.IsBeneficial) // Attempt avoidance
                {
                    mDefenseManager.DefendAgainstAction(ref resourceChangeInfo, orientation, ref interactionResult.AvoidanceResult, ref interactionResult.InteractionResult);
                }

                interactionResult.ChangedResource = resourceChangeInfo.ResourceChange;
                if (interactionResult.ChangedResource.Value != 0)
                {
                    interactionResult.WasSuccessful |= true;
                }

                mScreenTextManager.DisplayResourceChangedText(interactionResult.ChangedResource, interactionResult.InteractionResult, interactionResult.AvoidanceResult);
                ModifyCurrentResource(resourceChangeInfo.ResourceChange);
            }
        }

        public void ModifyCurrentResource(ResourceChange toChange)
        {
            if (mResourceManager.IsAlive)
            {
                mResourceManager.ModifyCurrentResource(toChange);

                if (mResourceManager.IsAlive)
                {
                    if (toChange.Value < 0)
                    {
                        GetComponent<Animator>().SetTrigger("GetHit2Trigger");
                    }
                    else
                    {
                        // display healing animation
                    }
                }
                else
                {
                    OnDeath();
                }
            }
        }

        //-----------------------------------------------------------------------------
        public ActionResourceChangeInformation GetModifiedActionStrength(ActionResourceChangeInformation actionStrength, List<ActionModifier> modifiers)
        {
            return mOffenseManager.GetModifiedActionStrength(actionStrength, modifiers);
        }

        #endregion

        #region TurnFlow
        //-----------------------------------------------------------------------------
        public bool ClockGuageFull { get { return mResourceManager.ClockGuague >= 100; } }
        public int ClockGuage { get { return (int)mResourceManager.ClockGuague; } }
        //-----------------------------------------------------------------------------
        public void NewTurn()
        {
            //Debug.Assert(IsAlive, "Attempted to Start new turn when unit is dead!");
            if (IsAlive)
            {
                mOffenseManager.HasActed = false;
                mMoveController.HasMoved = false;
                TurnOver = false;
                mStatusManager.OnTurnStart();
                mResourceManager.OnTurnStart();
            }
        }

        public void FinishTurn()
        {
            TurnOver = true;
        }

        //-----------------------------------------------------------------------------
        public void CleanupAfterTurn()
        {
            float clockReduction = 40f;
            if (mOffenseManager.HasActed && mMoveController.HasMoved)
            {
                clockReduction += 60f;
            }
            else if (mOffenseManager.HasActed || mMoveController.HasMoved)
            {
                clockReduction += 40f;
            }
            mResourceManager.DecrementClockGuage(clockReduction);
        }

        //-----------------------------------------------------------------------------
        private void RemoveFromBattle()
        {
            gameObject.SetActive(false);
        }

        //-----------------------------------------------------------------------------
        public void AddExperience()
        {
            float exp = Random.Range(20, 30);// calculate exp
            mScreenTextManager.DisplayExperienceGainedText(exp);
            bool levelUp = CharacterBase.AddCharacterExperience(exp);

            if (levelUp)
            {
                mScreenTextManager.DisplayLevelUpText();
            }

            exp = Random.Range(20, 30);
            
            mScreenTextManager.DisplayProfExperienceGainedText(exp);
            levelUp = CharacterBase.AddProfessionExperience(exp);
            if (levelUp)
            {
                mScreenTextManager.DisplayProfLevelUpText();
            }
        }

        //-----------------------------------------------------------------------------
        public bool Revive(float withHealth)
        {
            if (!IsAlive && IsKO)
            {
                GetComponent<Animator>().SetTrigger("Revive1Trigger");
                mResourceManager.ModifyCurrentResource(new ResourceChange(Resource.Health, withHealth));
                Status[(int)StatusType.KO] = 0f;
                mStatusManager.RemoveStatusEffect(StatusEffectIndex.KO);
                foreach (Aura aura in mAuras)
                {
                    aura.SetIsActive(true);
                }
                return true;
            }
            return false;
        }

        //-----------------------------------------------------------------------------
        private void OnDeath()
        {
            GetComponent<Animator>().SetTrigger("Death1Trigger");
            foreach (Aura aura in mAuras)
            {
                aura.SetIsActive(false);
            }
            mStatusManager.DispelStatusEffects(true, true);
            Status[(int)StatusType.KO] = 3f;
            Status[(int)StatusType.KOCounter] = 0f;
            mStatusManager.ApplyStatusEffect(StatusEffectFactory.CheckoutStatusEffect(StatusEffectIndex.KO));
        }

        //-----------------------------------------------------------------------------
        public void ProgressCharacterClock()
        {
            if (IsAlive)
            {
                mResourceManager.ProgressCharacterClock();
            }
        }

        //-----------------------------------------------------------------------------
        public bool ProgressStatusClock()
        {
            if (IsKO)
            {
                Status[(int)StatusType.KOCounter] += Stats[(int)TertiaryStat.Speed];
                if (Status[(int)StatusType.KOCounter] > 100)
                {
                    Status[(int)StatusType.KOCounter] -= 100;
                    Status[(int)StatusType.KO]--;
                    if (Status[(int)StatusType.KO] == 0)
                    {
                        RemoveFromBattle();
                    }
                    else
                    {
                        mScreenTextManager.DisplayText(Status[(int)StatusType.KO].ToString(), Color.red);
                    }
                }
            }
            else if (IsAlive)
            {
                mStatusManager.ProgressStatusClock();
            }
            
            return false;
        }

        #endregion

        #region StatusManagerFuncs
        //-----------------------------------------------------------------------------
        public bool AttemptStatusApplication(StatusEffect toApply, int numStacks = 1)
        {
            if (IsAlive)
            {
                return mStatusManager.ApplyStatusEffect(toApply, numStacks);
            }
            return false;
        }

        //-----------------------------------------------------------------------------
        public bool AttemptRemoveStatusEffect(StatusEffectIndex toRemove, int numStacks = -1)
        {
            return mStatusManager.RemoveStatusEffect(toRemove, numStacks);
        }

        //-----------------------------------------------------------------------------
        public bool DispelStatusEffects(bool removeBeneficial, bool removeHarmful)
        {
            return mStatusManager.DispelStatusEffects(removeBeneficial, removeHarmful);
        }

        //-----------------------------------------------------------------------------
        public bool DispelStatusEffectOfType(StatusType toRemove)
        {
            return mStatusManager.DispelStatusEffectOfType(toRemove);
        }

        //-----------------------------------------------------------------------------
        public StatusEffect QueryStatusEffect(StatusEffectIndex toFind)
        {
            return mStatusManager.QueryStatusEffect(toFind);
        }
        #endregion
    }
}

