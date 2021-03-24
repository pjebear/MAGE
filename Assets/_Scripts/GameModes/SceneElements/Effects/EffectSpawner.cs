using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MAGE.GameModes.SceneElements
{
    class EffectSpawner : MonoBehaviour
    {
        private AssetLoader<Effect> mEffectLoader = new AssetLoader<Effect>("VFX");

        private void Awake()
        {
            mEffectLoader.LoadAssets();
        }
        // Start is called before the first frame update
        void Start()
        {

        }

        // Update is called once per frame
        void Update()
        {

        }

        public EffectDurationInfo GetEffectDurationInfo(EffectType effectType)
        {
            return mEffectLoader.GetAsset(effectType.ToString()).DurationInfo;
        }

        public Effect SpawnEffect(EffectType effectType, Transform parent = null)
        {
            return Instantiate(mEffectLoader.GetAsset(effectType.ToString()), parent);   
        }
    }
}

