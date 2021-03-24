using MAGE.GameModes.SceneElements;
using MAGE.GameModes.SceneElements.Encounters;
using MAGE.GameSystems;
using MAGE.GameSystems.Actions;
using MAGE.GameSystems.Characters;
using MAGE.GameSystems.Stats;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MAGE.GameModes.Encounter
{
    class CharacterDirector : MonoBehaviour
    {
        //public Dictionary<Character, CharacterActorController> CharacterActorLookup;
        //public Dictionary<Character, Character> CharacterToParentLookup;

        //public Transform CharacterControlParent;

        //protected void Awake()
        //{
        //    CharacterActorLookup = new Dictionary<Character, CharacterActorController>();
        //    CharacterToParentLookup = new Dictionary<Character, Character>();
        //    CharacterControlParent = new GameObject("CharacterControllers").transform;
        //    CharacterControlParent.transform.SetParent(transform);
        //}

        //// Start is called before the first frame update
        //void Start()
        //{

        //}

        //public void CleanupCharacters()
        //{
        //    foreach (Character character in CharacterActorLookup.Keys)
        //    {
        //        EncounterFlowControl_Deprecated.AuraDirector.RemoveActor(CharacterActorLookup[character]);
        //        EncounterFlowControl_Deprecated.AnimationDirector.UnRegisterActor(CharacterActorLookup[character]);

        //        Destroy(CharacterActorLookup[character].gameObject);
        //    }
        //}

        //public void AddCharacter(Character character, ActorSpawner spawner, CharacterPosition characterPosition, Character parent = null)
        //{
        //    GameModel.Encounter.Characters.Add(character.Id, character);
        //    GameModel.Encounter.Teams[character.TeamSide].Add(character);

        //    CharacterActorController characterController = spawner.gameObject.AddComponent<CharacterActorController>();
        //    characterController.BillboardEmitter = Instantiate(EncounterPrefabLoader.LoadBillBoardEmitterPrefab(), characterController.transform);
        //    characterController.ActorSpawner = spawner;
        //    characterController.ActorController = characterController.GetComponent<ActorController>();
        //    characterController.ActorController.SetControllerState(ActorController.ControllerState.TopDown);

        //    CharacterActorLookup.Add(character, characterController);
        //    characterController.Character = character;

        //    foreach (AuraType type in character.GetAuras())
        //    {
        //        EncounterFlowControl_Deprecated.AuraDirector.RegisterAura(character.GetAuraInfo(type), characterController, false);
        //    }

        //    EncounterFlowControl_Deprecated.MapControl.AddCharacterToMap(characterController, characterPosition);
        //    EncounterFlowControl_Deprecated.AnimationDirector.RegisterActor(characterController);

        //    if (parent != null)
        //    {
        //        CharacterToParentLookup.Add(character, parent);
        //    }
        //}

        //public void AddCharacter(Character character, CharacterPosition characterPosition, Character parent = null)
        //{
        //    ActorSpawner spawner = ActorLoader.Instance.CreateActorSpawner();
        //    spawner.CharacterPicker.RootCharacterId = character.Id;

        //    AddCharacter(character, spawner, characterPosition, parent);
        //}

        //public void RemoveCharacter()
        //{

        //}

        //public Character GetCharacterParent(Character character)
        //{
        //    Character parent = null;
        //    if (CharacterToParentLookup.ContainsKey(character))
        //    {
        //        parent = CharacterToParentLookup[character];
        //    }
        //    return parent;
        //}

        //public bool HasParent(Character character)
        //{
        //    return CharacterToParentLookup.ContainsKey(character);
        //}

        //public CharacterActorController GetController(Character character)
        //{
        //    return CharacterActorLookup[character];
        //}

        //public void UpdateCharacterPosition(Character character, CharacterPosition characterPosition)
        //{
        //    EncounterFlowControl_Deprecated.MapControl.UpdateCharacterPosition(CharacterActorLookup[character], characterPosition);
        //}

        //public void ApplyStateChange(Character character, StateChange stateChange)
        //{
        //    character.ApplyStateChange(stateChange);
        //    if (!character.IsAlive)
        //    {
        //        CharacterActorController controller = CharacterActorLookup[character];
        //        EncounterFlowControl_Deprecated.AnimationDirector.AnimateActor(controller, AnimationFactory.CheckoutAnimation(AnimationId.Faint));
        //        controller.GetComponent<AudioSource>().PlayOneShot(AudioManager.Instance.GetSFXClip(SFXId.MaleDeath));

        //        Messaging.MessageRouter.Instance.NotifyMessage(new EncounterMessage(EncounterMessage.EventType.CharacterKO, character));
        //    }
        //}

        //public void ApplyAura(Character character, StatusEffect auraEffect)
        //{
        //    CharacterActorLookup[character].DisplayStatusApplication(auraEffect);
        //    character.ApplyStatusEffect(auraEffect);
        //}

        //public void RemoveAura(Character character, StatusEffect auraEffect)
        //{
        //    CharacterActorLookup[character].DisplayStatusRemoval(auraEffect);
        //    character.RemoveStatusEffect(auraEffect);
        //}

        //public void IncrementStatusEffects()
        //{
        //    foreach (Character character in GameModel.Encounter.Characters.Values)
        //    {
        //        if (character.IsAlive)
        //        {
        //            foreach (StatusEffect effect in character.ProgressStatusEffects())
        //            {
        //                CharacterActorLookup[character].DisplayStatusRemoval(effect);
        //            }
        //        }
        //    }
        //}

        //public void ApplyStatusEffects()
        //{
        //    foreach (Character character in GameModel.Encounter.Characters.Values)
        //    {
        //        if (character.IsAlive)
        //        {
        //            foreach (StateChange stateChange in character.GetTurnStartStateChanges())
        //            {
        //                CharacterActorLookup[character].DisplayStateChange(stateChange);
        //                ApplyStateChange(character, stateChange);
        //            }
        //        }
        //    }
        //}

        //// Update is called once per frame
        //void Update()
        //{

        //}
    }
}


