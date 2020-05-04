using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

class ActorLoader : MonoBehaviour
{
    private AssetLoader<GameObject> mActorLoader;

    private void Awake()
    {
        mActorLoader = new AssetLoader<GameObject>("Props");
        mActorLoader.LoadAssets("Bodies");
        mActorLoader.LoadAssets("Apparel");
    }

    public EncounterActorController CreateActor(ActorSpawnParams actorParams, Transform parent)
    {
        EncounterActorController actorController = Instantiate(mActorLoader.GetAsset(actorParams.BodyType.GetAssetName(AppearanceType.Prefab)), parent).GetComponent<EncounterActorController>();

        if (actorParams.HeldLeftHand[AppearanceType.Prefab] != Appearance.NO_ASSET)
        {
            Instantiate(mActorLoader.GetAsset(actorParams.HeldLeftHand.GetAssetName(AppearanceType.Prefab)), actorController.Actor.LeftHand);
        }

        if (actorParams.HeldRightHand[AppearanceType.Prefab] != Appearance.NO_ASSET)
        {
            Instantiate(mActorLoader.GetAsset(actorParams.HeldRightHand.GetAssetName(AppearanceType.Prefab)), actorController.Actor.RightHand);
        }

        if (actorParams.Worn[AppearanceType.Prefab] != Appearance.NO_ASSET)
        {
            Instantiate(mActorLoader.GetAsset(actorParams.Worn.GetAssetName(AppearanceType.Prefab)), actorController.Actor.Body);
        }

        return actorController;
    }
}

