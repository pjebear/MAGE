using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

class ActorLoader : MonoBehaviour
{
    private AssetLoader<GameObject> mActorLoader;
    public static ActorLoader Instance;

    private void Awake()
    {
        if (Instance != null)
        {
            DestroyImmediate(this);
            return;
        }

        Instance = this;

        mActorLoader = new AssetLoader<GameObject>("Props");
        mActorLoader.LoadAssets("Bodies");
        mActorLoader.LoadAssets("Apparel");
    }

    public Actor CreateActor(Appearance appearance, Transform parent)
    {
        Actor actor = Instantiate(mActorLoader.GetAsset(appearance.BodyType.ToString()), parent).GetComponent<Actor>();

        if (appearance.LeftHeldId != AppearancePrefabId.prefab_none)
        {
            Instantiate(mActorLoader.GetAsset(appearance.LeftHeldId.ToString()), actor.LeftHand);
        }

        if (appearance.RightHeldId != AppearancePrefabId.prefab_none)
        {
            Instantiate(mActorLoader.GetAsset(appearance.RightHeldId.ToString()), actor.RightHand);
        }

        if (appearance.ArmorId != AppearancePrefabId.prefab_none)
        {
            GameObject gameObject = Instantiate(mActorLoader.GetAsset(appearance.ArmorId.ToString()), actor.Body);
            if (gameObject.GetComponentInChildren<SkinnedMeshRenderer>() != null)
            {
                UpdateBoneMapping(
                    actor.gameObject.GetComponentInChildren<SkinnedMeshRenderer>(),
                    gameObject.GetComponentInChildren<SkinnedMeshRenderer>());
            }
        }

        return actor;
    }

    private void UpdateBoneMapping(SkinnedMeshRenderer actorMeshRenderer, SkinnedMeshRenderer skinnedMeshRenderer)
    {

        Debug.Assert(skinnedMeshRenderer.bones.Length == actorMeshRenderer.bones.Length);

        Dictionary<string, Transform> boneLookup = new Dictionary<string, Transform>();
        foreach (Transform bone in actorMeshRenderer.bones)
        {
            boneLookup.Add(bone.name, bone);
        }

        Transform[] newBoneArray = new Transform[skinnedMeshRenderer.bones.Length];
        for (int i = 0; i < skinnedMeshRenderer.bones.Length; ++i)
        {
            string boneName = skinnedMeshRenderer.bones[i].name;
            Debug.Assert(boneLookup.ContainsKey(boneName));
            if (boneLookup.ContainsKey(boneName))
            {
                newBoneArray[i] = boneLookup[boneName];
            }
        }
        skinnedMeshRenderer.bones = newBoneArray;

        skinnedMeshRenderer.rootBone = actorMeshRenderer.rootBone;
    }
}

