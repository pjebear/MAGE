using System.Collections;
using System.Collections.Generic;
using UnityEngine;

class CharacterDirector : MonoBehaviour
{
    

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

    public EncounterActorController AddCharacter(EncounterCharacter encounterCharacter, ActorSpawnParams actorParams, TileIdx atIdx)
    {
        Transform actorParent = GameObject.Find("Actors").transform;
        EncounterActorController actorController = GameModesModule.ActorLoader.CreateActor(actorParams, actorParent);

        CharacterActorLookup.Add(encounterCharacter, actorController);
        actorController.EncounterCharacter = encounterCharacter;

        foreach (AuraType type in encounterCharacter.Auras)
        {
            EncounterModule.AuraDirector.RegisterAura(encounterCharacter.GetAuraInfo(type), actorController, false);
        }

        Map.Instance.PlaceAtTile(atIdx, actorController);
        mActorPositions.Add(actorController, atIdx);
        EncounterModule.AnimationDirector.RegisterActor(actorController);

        return actorController;
    }

    public EncounterActorController GetController(EncounterCharacter character)
    {
        return CharacterActorLookup[character];
    }

    public TileIdx GetActorPosition(EncounterCharacter character)
    {
        return mActorPositions[GetController(character)];
    }

    public void MoveActor(EncounterCharacter character, TileIdx toLocation)
    {
        StartCoroutine(MoveCharacter(character, toLocation));
    }

    IEnumerator MoveCharacter(EncounterCharacter character, TileIdx toLocation)
    {
        yield return new WaitForEndOfFrame();
        mActorPositions[CharacterActorLookup[character]] = toLocation;
        EncounterModule.Map.PlaceAtTile(toLocation, CharacterActorLookup[character]);
        EncounterEventRouter.Instance.NotifyEvent(new EncounterEvent(EncounterEvent.EventType.MoveResolved));
    }

    public void ApplyStateChange(EncounterCharacter character, StateChange stateChange)
    {
        character.ApplyStateChange(stateChange);
        if (!character.IsAlive)
        {
            EncounterModule.AnimationDirector.AnimateActor(CharacterActorLookup[character], AnimationFactory.CheckoutAnimation(AnimationId.Faint));
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
