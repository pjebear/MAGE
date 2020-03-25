using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

class AuraDirector : MonoBehaviour
    , IEventHandler<EncounterEvent>
{
    public Aura AuraPrefab;

    private Dictionary<ActorController, List<Aura>> mAuras = new Dictionary<ActorController, List<Aura>>();

    private void Awake()
    {
        EncounterEventRouter.Instance.RegisterHandler(this);
    }

    private void OnDestroy()
    {
        EncounterEventRouter.Instance.UnRegisterListener(this);
    }

    public void RegisterAura(AuraInfo auraInfo, ActorController actorController, bool activateImmediately)
    {
        Aura aura = Instantiate(AuraPrefab, actorController.transform);
        aura.Initialize(auraInfo, actorController);

        if (!mAuras.ContainsKey(actorController)) { mAuras.Add(actorController, new List<Aura>()); }

        mAuras[actorController].Add(aura);

        aura.SetActive(activateImmediately);
    }

    public void HandleEvent(EncounterEvent eventInfo)
    {
        switch (eventInfo.Type)
        {
            case EncounterEvent.EventType.EncounterBegun:
                {
                    foreach (List<Aura> auras in mAuras.Values)
                    {
                        foreach (Aura aura in auras)
                        {
                            aura.SetActive(true);
                        }
                    }
                }
                break;

            case EncounterEvent.EventType.CharacterKO:
                {
                    ActorController controller = EncounterModule.ActorDirector.ActorControllerLookup[eventInfo.Arg<EncounterCharacter>()];
                    if (mAuras.ContainsKey(controller))
                    {
                        foreach (Aura aura in mAuras[controller])
                        {
                            aura.SetActive(false);
                        }
                    }

                }
                break;
        }
    }

}

