using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

class ActorLoader : IAssetManager<GameObject>
{

    protected override string GetAssetPath()
    {
        return "Props";
    }

    protected override void OnInitialize()
    {
        base.OnInitialize();

        LoadAssets("Bodies");
        LoadAssets("Apparel");  
    }

    public ActorController CreateActor(EncounterCharacter fromCharacter, Transform parent)
    {
        ActorController actorController = Instantiate(GetAsset(BodyType.DefaultBody.ToString()), parent).GetComponent<ActorController>();

        if (fromCharacter.Equipment[Equipment.Slot.Armor] != null)
        {
            string assetName = fromCharacter.Equipment[Equipment.Slot.Armor].Appearance.GetAssetName(AppearanceType.Prefab);
            Instantiate(GetAsset(assetName), actorController.Actor.Body);
        }

        if (fromCharacter.Equipment[Equipment.Slot.LeftHand] != null)
        {
            string assetName = fromCharacter.Equipment[Equipment.Slot.LeftHand].Appearance.GetAssetName(AppearanceType.Prefab);
            Instantiate(GetAsset(assetName), actorController.Actor.LeftHand);
        }

        if (fromCharacter.Equipment[Equipment.Slot.RightHand] != null)
        {
            string assetName = fromCharacter.Equipment[Equipment.Slot.RightHand].Appearance.GetAssetName(AppearanceType.Prefab);
            Instantiate(GetAsset(assetName), actorController.Actor.RightHand);
        }

        return actorController;
    }
}

