using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

class LevelManager : MonoBehaviour
{
    private Level mLoadedLevel = null;
    private AssetLoader<Level> mLevelLoader = null;

    public static LevelManager Instance;

    private void Awake()
    {
        mLevelLoader = new AssetLoader<Level>("Levels");
        mLevelLoader.LoadAssets();
    }

    public void LoadLevel(LevelId levelId)
    {
        mLoadedLevel = Instantiate(mLevelLoader.GetAsset(levelId.ToString()));
    }

    public Level GetLoadedLevel()
    {
        return mLoadedLevel;
    }

    public void UnloadLevel()
    {
        Destroy(mLoadedLevel.gameObject);
        mLoadedLevel = null;
    }

    public void NotifyLevelLoaded(Level level)
    {
        mLoadedLevel = level;
    }
}

