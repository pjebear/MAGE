using MAGE.GameModes.SceneElements;
using MAGE.GameSystems;
using MAGE.GameSystems.Actions;
using MAGE.GameSystems.Characters;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MAGE.GameModes.Encounter
{
    class CharacterDirector : MonoBehaviour
    {
        public Dictionary<Character, CharacterActorController> CharacterActorLookup;
        public Dictionary<Character, Character> CharacterToParentLookup;

        public Transform CharacterControlParent;

        protected void Awake()
        {
            CharacterActorLookup = new Dictionary<Character, CharacterActorController>();
            CharacterToParentLookup = new Dictionary<Character, Character>();
            CharacterControlParent = new GameObject("CharacterControllers").transform;
            CharacterControlParent.transform.SetParent(transform);
        }

        // Start is called before the first frame update
        void Start()
        {

        }

        public void CleanupCharacters()
        {
            foreach (Character character in CharacterActorLookup.Keys)
            {
                EncounterFlowControl.AuraDirector.RemoveActor(CharacterActorLookup[character]);
                EncounterFlowControl.AnimationDirector.UnRegisterActor(CharacterActorLookup[character]);

                Destroy(CharacterActorLookup[character].gameObject);
            }
        }

        public CharacterActorController AddCharacter(Character character, CharacterPosition characterPosition, Character parent = null)
        {
            Actor actor = ActorLoader.Instance.CreateActor(character.GetAppearance(), CharacterControlParent);

            EncounterFlowControl.Model.Characters.Add(character.Id, character);
            EncounterFlowControl.Model.Teams[character.TeamSide].Add(character);

            CharacterActorController actorController = actor.gameObject.AddComponent<CharacterActorController>();
            actorController.Actor = actor;
            BillboardEmitter emitter = EncounterPrefabLoader.LoadBillBoardEmitterPrefab();
            actorController.BillboardEmitter = Instantiate(emitter, actorController.transform);
            actorController.ActorController = actorController.gameObject.AddComponent<ActorController>();
            actorController.ActorController.MoveSpeed = 3;
            CharacterActorLookup.Add(character, actorController);
            actorController.Character = character;

            foreach (AuraType type in actorController.Character.GetAuras())
            {
                EncounterFlowControl.AuraDirector.RegisterAura(actorController.Character.GetAuraInfo(type), actorController, false);
            }

            EncounterFlowControl.MapControl.AddCharacterToMap(actorController, characterPosition);
            EncounterFlowControl.AnimationDirector.RegisterActor(actorController);

            if (parent != null)
            {
                CharacterToParentLookup.Add(character, parent);
            }

            return actorController;
        }

        public void RemoveCharacter()
        {

        }

        public Character GetCharacterParent(Character character)
        {
            Character parent = null;
            if (CharacterToParentLookup.ContainsKey(character))
            {
                parent = CharacterToParentLookup[character];
            }
            return parent;
        }

        public bool HasParent(Character character)
        {
            return CharacterToParentLookup.ContainsKey(character);
        }

        public CharacterActorController GetController(Character character)
        {
            return CharacterActorLookup[character];
        }

        public void UpdateCharacterPosition(Character character, CharacterPosition characterPosition)
        {
            EncounterFlowControl.MapControl.UpdateCharacterPosition(CharacterActorLookup[character], characterPosition);
        }

        public void ApplyStateChange(Character character, StateChange stateChange)
        {
            character.ApplyStateChange(stateChange);
            if (!character.IsAlive)
            {
                CharacterActorController controller = CharacterActorLookup[character];
                EncounterFlowControl.AnimationDirector.AnimateActor(controller, AnimationFactory.CheckoutAnimation(AnimationId.Faint));
                controller.GetComponent<AudioSource>().PlayOneShot(AudioManager.Instance.GetSFXClip(SFXId.MaleDeath));

                Messaging.MessageRouter.Instance.NotifyMessage(new EncounterMessage(EncounterMessage.EventType.CharacterKO, character));
            }
        }

        public void ApplyAura(Character character, StatusEffect auraEffect)
        {
            CharacterActorLookup[character].DisplayStatusApplication(auraEffect);
            character.ApplyStatusEffect(auraEffect);
        }

        public void RemoveAura(Character character, StatusEffect auraEffect)
        {
            CharacterActorLookup[character].DisplayStatusRemoval(auraEffect);
            character.RemoveStatusEffect(auraEffect);
        }

        public void IncrementStatusEffects()
        {
            foreach (Character character in EncounterFlowControl.Model.Characters.Values)
            {
                if (character.IsAlive)
                {
                    foreach (StatusEffect effect in character.ProgressStatusEffects())
                    {
                        CharacterActorLookup[character].DisplayStatusRemoval(effect);
                    }
                }
            }
        }

        public void ApplyStatusEffects()
        {
            foreach (Character character in EncounterFlowControl.Model.Characters.Values)
            {
                if (character.IsAlive)
                {
                    foreach (StateChange stateChange in character.GetTurnStartStateChanges())
                    {
                        CharacterActorLookup[character].DisplayStateChange(stateChange);
                        ApplyStateChange(character, stateChange);
                    }
                }
            }
        }

        // Update is called once per frame
        void Update()
        {

        }
    }
}


