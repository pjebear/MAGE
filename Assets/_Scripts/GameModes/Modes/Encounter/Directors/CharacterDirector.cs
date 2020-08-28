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
        public BillboardEmitter BillboardEmitterPrefab;

        private Dictionary<CharacterActorController, TileIdx> mActorPositions = null;

        public Dictionary<Character, CharacterActorController> CharacterActorLookup;

        public Transform CharacterControlParent;

        protected void Awake()
        {

            CharacterActorLookup = new Dictionary<Character, CharacterActorController>();
            mActorPositions = new Dictionary<CharacterActorController, TileIdx>();
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
                EncounterModule.AuraDirector.RemoveActor(CharacterActorLookup[character]);
                EncounterModule.AnimationDirector.UnRegisterActor(CharacterActorLookup[character]);

                Destroy(CharacterActorLookup[character].gameObject);
            }
        }

        public CharacterActorController AddCharacter(Character character, CharacterPosition characterPosition)
        {
            EncounterModule.Model.Characters.Add(character.Id, character);
            EncounterModule.Model.Teams[character.TeamSide].Add(character);

            Actor actor = GameModesModule.ActorLoader.CreateActor(character.GetAppearance(), CharacterControlParent);
            CharacterActorController actorController = actor.gameObject.AddComponent<CharacterActorController>();
            actorController.Actor = actor;

            actorController.BillboardEmitter = Instantiate(BillboardEmitterPrefab, actorController.transform);
            actorController.ActorController = actorController.gameObject.AddComponent<ActorController>();
            actorController.ActorController.MoveSpeed = 3;
            CharacterActorLookup.Add(character, actorController);
            actorController.Character = character;

            foreach (AuraType type in actorController.Character.GetAuras())
            {
                EncounterModule.AuraDirector.RegisterAura(actorController.Character.GetAuraInfo(type), actorController, false);
            }

            EncounterModule.MapControl.AddCharacterToMap(actorController, characterPosition);
            mActorPositions.Add(actorController, characterPosition.Location);
            EncounterModule.AnimationDirector.RegisterActor(actorController);

            return actorController;
        }

        public void RemoveCharacter()
        {

        }

        public CharacterActorController GetController(Character character)
        {
            return CharacterActorLookup[character];
        }

        public TileIdx GetCharacterPosition(Character character)
        {
            return mActorPositions[GetController(character)];
        }

        public void UpdateCharacterPosition(Character character, CharacterPosition characterPosition)
        {
            mActorPositions[CharacterActorLookup[character]] = characterPosition.Location;
            EncounterModule.MapControl.UpdateCharacterPosition(CharacterActorLookup[character], characterPosition);
        }

        public void ApplyStateChange(Character character, StateChange stateChange)
        {
            character.ApplyStateChange(stateChange);
            if (!character.IsAlive)
            {
                CharacterActorController controller = CharacterActorLookup[character];
                EncounterModule.AnimationDirector.AnimateActor(controller, AnimationFactory.CheckoutAnimation(AnimationId.Faint));
                controller.GetComponent<AudioSource>().PlayOneShot(GameModesModule.AudioManager.GetSFXClip(SFXId.MaleDeath));

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
            foreach (Character character in EncounterModule.Model.Characters.Values)
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
            foreach (Character character in EncounterModule.Model.Characters.Values)
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


