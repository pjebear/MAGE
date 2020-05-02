using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

class LevelManager : IAssetManager<Level>
{
    private Level mLoadedLevel = null;

    public static LevelManager Instance;

    private void Awake()
    {
        InitializeAssets();   
    }

    protected override void OnInitializeAssets()
    {
        base.OnInitializeAssets();

        LoadAssets("");
    }

    protected override string GetAssetPath()
    {
        return "Levels";
    }

    public void LoadLevel(LevelId levelId)
    {
        mLoadedLevel = Instantiate(GetAsset(levelId.ToString()));
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

