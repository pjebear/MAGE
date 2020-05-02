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

    public void CleanupCharacters()
    {
        foreach (EncounterCharacter encounterCharacter in CharacterActorLookup.Keys)
        {
            EncounterModule.AuraDirector.RemoveActor(CharacterActorLookup[encounterCharacter]);
            EncounterModule.AnimationDirector.UnRegisterActor(CharacterActorLookup[encounterCharacter]);

            Destroy(CharacterActorLookup[encounterCharacter].gameObject);
        }
    }

    public EncounterActorController AddCharacter(DB.DBCharacter dBCharacter, TeamSide team, TileIdx atIdx)
    {
        Transform actorParent = GameObject.Find("Actors").transform;

        EncounterCharacter character = new EncounterCharacter(team, CharacterLoader.LoadCharacter(dBCharacter));
        character.Team = team;
        EncounterModule.Model.Characters.Add(character.Id, character);
        EncounterModule.Model.Teams[team].Add(character);

        EncounterActorController actorController = GameModesModule.ActorLoader.CreateActor(CharacterUtil.ActorParamsForCharacter(dBCharacter), actorParent);

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
