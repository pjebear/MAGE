using System.Collections;
using System.Collections.Generic;
using UnityEngine;

class CharacterDirector : MonoBehaviour
{
    public BillboardEmitter BillboardEmitterPrefab;

    private Dictionary<EncounterActorController, TileIdx> mActorPositions = null;

    public Dictionary<EncounterCharacter, EncounterActorController> CharacterActorLookup;

    GameObject Canvas;


    protected void Awake()
    {
        
        Canvas = GameObject.Find("Canvas");
       
        CharacterActorLookup = new Dictionary<EncounterCharacter, EncounterActorController>();
        mActorPositions = new Dictionary<EncounterActorController, TileIdx>();
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    public void CleanupCharacters()
    {
        foreach (EncounterCharacter encounterCharacter in CharacterActorLookup.Keys)
        {
            EncounterModule.AuraDirector.RemoveActor(CharacterActorLookup[encounterCharacter]);
            EncounterModule.AnimationDirector.UnRegisterActor(CharacterActorLookup[encounterCharacter]);

            Destroy(CharacterActorLookup[encounterCharacter].gameObject);
        }
    }

    public EncounterActorController AddCharacter(EncounterCharacter character, TileIdx atIdx)
    {
        Transform actorParent = GameObject.Find("EncounterContainer").transform;

        EncounterModule.Model.Characters.Add(character.Id, character);
        EncounterModule.Model.Teams[character.Team].Add(character);

        Actor actor = GameModesModule.ActorLoader.CreateActor(character.Appearance, actorParent);
        EncounterActorController actorController = actor.gameObject.AddComponent<EncounterActorController>();
        actorController.Actor = actor;

        actorController.BillboardEmitter = Instantiate(BillboardEmitterPrefab, actorController.transform);
        actorController.ActorController = actorController.gameObject.AddComponent<ActorController>();
        actorController.ActorController.MoveSpeed = 3;
        CharacterActorLookup.Add(character, actorController);
        actorController.EncounterCharacter = character;

        foreach (AuraType type in actorController.EncounterCharacter.Auras)
        {
            EncounterModule.AuraDirector.RegisterAura(actorController.EncounterCharacter.GetAuraInfo(type), actorController, false);
        }

        EncounterModule.Map.PlaceAtTile(atIdx, actorController);
        mActorPositions.Add(actorController, atIdx);
        EncounterModule.AnimationDirector.RegisterActor(actorController);

        return actorController;
    }

    public void RemoveCharacter()
    {

    }

    public EncounterActorController GetController(EncounterCharacter character)
    {
        return CharacterActorLookup[character];
    }

    public TileIdx GetActorPosition(EncounterCharacter character)
    {
        return mActorPositions[GetController(character)];
    }

    public void UpdateCharacterPosition(EncounterCharacter character, TileIdx toLocation)
    {
        mActorPositions[CharacterActorLookup[character]] = toLocation;
        EncounterModule.Map.PlaceAtTile(toLocation, CharacterActorLookup[character]);
    }

    public void ApplyStateChange(EncounterCharacter character, StateChange stateChange)
    {
        character.ApplyStateChange(stateChange);
        if (!character.IsAlive)
        {
            EncounterActorController controller = CharacterActorLookup[character];
            EncounterModule.AnimationDirector.AnimateActor(controller, AnimationFactory.CheckoutAnimation(AnimationId.Faint));
            controller.GetComponent<AudioSource>().PlayOneShot(GameModesModule.AudioManager.GetSFXClip(SFXId.MaleDeath));

            EncounterEventRouter.Instance.NotifyEvent(new EncounterEvent(EncounterEvent.EventType.CharacterKO, character));
        }
    }

    public void IncrementStatusEffects()
    {
        foreach (EncounterCharacter character in EncounterModule.Model.Characters.Values)
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
        foreach (EncounterCharacter character in EncounterModule.Model.Characters.Values)
        {
            if (character.IsAlive)
            {
                foreach(StateChange stateChange in character.GetTurnStartStateChanges())
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
