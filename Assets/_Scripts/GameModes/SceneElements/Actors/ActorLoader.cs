using MAGE.GameModes.SceneElements;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

class ActorLoader : MonoBehaviour
{
    private AssetLoader<Actor> mActorLoader;
    private AssetLoader<ActorSpawner> mActorSpawnerLoader;
    public static ActorLoader Instance;

    private void Awake()
    {
        if (Instance != null)
        {
            DestroyImmediate(this);
            return;
        }

        Instance = this;

        mActorLoader = new AssetLoader<Actor>("Props/Bodies");
        mActorLoader.LoadAssets();
        mActorSpawnerLoader = new AssetLoader<ActorSpawner>("Props/ActorSpawner");
        mActorSpawnerLoader.LoadAssets();
    }

    public ActorSpawner CreateActorSpawner()
    {
        return Instantiate(mActorSpawnerLoader.GetAsset("CharacterSpawner"));
    }

    public Actor LoadActor(BodyType bodyType, Transform parent)
    {
       return Instantiate(mActorLoader.GetAsset(bodyType.ToString()), parent);
    }

    public ActorController LoadActor()
    {
        return Instantiate(mActorSpawnerLoader.GetAsset("Actor")).GetComponent<ActorController>();
    }

    //private void UpdateBoneMapping(SkinnedMeshRenderer actorMeshRenderer, SkinnedMeshRenderer skinnedMeshRenderer)
    //{
    //    Debug.Assert(skinnedMeshRenderer.bones.Length == actorMeshRenderer.bones.Length);

    //    Dictionary<string, Transform> boneLookup = new Dictionary<string, Transform>();
    //    foreach (Transform bone in actorMeshRenderer.bones)
    //    {
    //        boneLookup.Add(bone.name, bone);
    //    }

    //    Transform[] newBoneArray = new Transform[skinnedMeshRenderer.bones.Length];
    //    for (int i = 0; i < skinnedMeshRenderer.bones.Length; ++i)
    //    {
    //        string boneName = skinnedMeshRenderer.bones[i].name;
    //        Debug.Assert(boneLookup.ContainsKey(boneName));
    //        if (boneLookup.ContainsKey(boneName))
    //        {
    //            newBoneArray[i] = boneLookup[boneName];
    //        }
    //    }
    //    skinnedMeshRenderer.bones = newBoneArray;

    //    skinnedMeshRenderer.rootBone = actorMeshRenderer.rootBone;
    //}

    //private void UpdateBoneMapping(Transform actorTransforms, SkinnedMeshRenderer skinnedMeshRenderer)
    //{

    //    Dictionary<string, Transform> boneLookup = new Dictionary<string, Transform>();
    //    foreach (Transform bone in actorMeshRenderer.bones)
    //    {
    //        boneLookup.Add(bone.name, bone);
    //    }

    //    Transform[] newBoneArray = new Transform[skinnedMeshRenderer.bones.Length];
    //    for (int i = 0; i < skinnedMeshRenderer.bones.Length; ++i)
    //    {
    //        string boneName = skinnedMeshRenderer.bones[i].name;
    //        Debug.Assert(boneLookup.ContainsKey(boneName));
    //        if (boneLookup.ContainsKey(boneName))
    //        {
    //            newBoneArray[i] = boneLookup[boneName];
    //        }
    //    }
    //    skinnedMeshRenderer.bones = newBoneArray;

    //    skinnedMeshRenderer.rootBone = actorMeshRenderer.rootBone;
    //}

    //void GetBones(Transform rootBone, ref Dictionary<string, Transform> boneMap)
    //{

    //}
}

