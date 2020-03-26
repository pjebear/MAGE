using System.Collections;
using System.Collections.Generic;
using UnityEngine;

class ActorDirector : MonoBehaviour
{
    

    private Dictionary<ActorController, TileIdx> mActorPositions = null;

    public Dictionary<EncounterCharacter, ActorController> ActorControllerLookup;

    public DebugStateRig StateRigPrefab;
    GameObject Canvas;


    protected void Awake()
    {
        
        Canvas = GameObject.Find("Canvas");
       
        ActorControllerLookup = new Dictionary<EncounterCharacter, ActorController>();
        mActorPositions = new Dictionary<ActorController, TileIdx>();
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    public ActorController AddActor(ActorSpawnParams actorParams, EncounterCharacter actor, TileIdx atIdx)
    {
        Transform actorParent = GameObject.Find("Actors").transform;
        ActorController actorController = GameModesModule.ActorLoader.CreateActor(actorParams, actorParent);
        actorController.Initialize(actor);
        ActorControllerLookup.Add(actor, actorController);

        foreach (AuraType type in actor.Auras)
        {
            EncounterModule.AuraDirector.RegisterAura(actor.GetAuraInfo(type), actorController, false);
        }

        Map.Instance.PlaceAtTile(atIdx, actorController);
        mActorPositions.Add(actorController, atIdx);
        EncounterModule.AnimationDirector.RegisterActor(actorController);

        return actorController;
    }

    public ActorController GetController(EncounterCharacter actor)
    {
        return ActorControllerLookup[actor];
    }

    public TileIdx GetActorPosition(EncounterCharacter actor)
    {
        return mActorPositions[GetController(actor)];
    }

    public void MoveActor(EncounterCharacter actor, TileIdx toLocation)
    {
        StartCoroutine(MoveCharacter(actor, toLocation));
    }

    IEnumerator MoveCharacter(EncounterCharacter actor, TileIdx toLocation)
    {
        yield return new WaitForEndOfFrame();
        mActorPositions[ActorControllerLookup[actor]] = toLocation;
        EncounterModule.Map.PlaceAtTile(toLocation, ActorControllerLookup[actor]);
        EncounterEventRouter.Instance.NotifyEvent(new EncounterEvent(EncounterEvent.EventType.MoveResolved));
    }

    public void ApplyStateChange(EncounterCharacter actor, StateChange stateChange)
    {
        actor.ApplyStateChange(stateChange);
        if (!actor.IsAlive)
        {
            EncounterModule.AnimationDirector.AnimateActor(ActorControllerLookup[actor], AnimationFactory.CheckoutAnimation(AnimationId.Faint));
            EncounterEventRouter.Instance.NotifyEvent(new EncounterEvent(EncounterEvent.EventType.CharacterKO, actor));
        }
    }

    public void IncrementStatusEffects()
    {
        foreach (EncounterCharacter actor in EncounterModule.Model.Actors.Values)
        {
            if (actor.IsAlive)
            {
                foreach (StatusEffect effect in actor.ProgressStatusEffects())
                {
                    ActorControllerLookup[actor].DisplayStatusRemoval(effect);
                }
            }
        }
    }

    public void ApplyStatusEffects()
    {
        foreach (EncounterCharacter actor in EncounterModule.Model.Actors.Values)
        {
            if (actor.IsAlive)
            {
                foreach(StateChange stateChange in actor.GetTurnStartStateChanges())
                {
                    ActorControllerLookup[actor].DisplayStateChange(stateChange);
                    ApplyStateChange(actor, stateChange);
                }
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
