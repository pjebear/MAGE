using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

class ActorLoader : IAssetManager<GameObject>
{
    private void Awake()
    {
        // IAssetManager
        InitializeAssets();
    }

    protected override string GetAssetPath()
    {
        return "Props";
    }

    protected override void OnInitializeAssets()
    {
        base.OnInitializeAssets();

        LoadAssets("Bodies");
        LoadAssets("Apparel");  
    }

    public EncounterActorController CreateActor(ActorSpawnParams actorParams, Transform parent)
    {
        EncounterActorController actorController = Instantiate(GetAsset(actorParams.BodyType.GetAssetName(AppearanceType.Prefab)), parent).GetComponent<EncounterActorController>();

        if (actorParams.HeldLeftHand[AppearanceType.Prefab] != Appearance.NO_ASSET)
        {
            Instantiate(GetAsset(actorParams.HeldLeftHand.GetAssetName(AppearanceType.Prefab)), actorController.Actor.LeftHand);
        }

        if (actorParams.HeldRightHand[AppearanceType.Prefab] != Appearance.NO_ASSET)
        {
            Instantiate(GetAsset(actorParams.HeldRightHand.GetAssetName(AppearanceType.Prefab)), actorController.Actor.RightHand);
        }

        if (actorParams.Worn[AppearanceType.Prefab] != Appearance.NO_ASSET)
        {
            Instantiate(GetAsset(actorParams.Worn.GetAssetName(AppearanceType.Prefab)), actorController.Actor.Body);
        }

        return actorController;
    }
}

