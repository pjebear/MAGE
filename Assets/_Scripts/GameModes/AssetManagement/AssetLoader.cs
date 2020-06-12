using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

class AssetLoader<T> where T : UnityEngine.Object
{
    private Dictionary<string, List<string>> mLoadedDirectories;
    private Dictionary<string, T> mAssetLookup;
    private string mBaseAssetPath;

    protected string mManagerName;

    public AssetLoader(string rootFolder)
    {
        mAssetLookup = new Dictionary<string, T>();
        mLoadedDirectories = new Dictionary<string, List<string>>();
        mManagerName = rootFolder + "AssetLoader";
        mBaseAssetPath = rootFolder;
    }

    public void LoadAssets()
    {
        LoadAssets("");
    }

    public void LoadAssets(string folderPath)
    {
        Logger.Log(LogTag.Assets, mManagerName, "Loading " + folderPath);

        string fullPath = Path.Combine(mBaseAssetPath, folderPath);
        T[] assets = UnityEngine.Resources.LoadAll<T>(fullPath);
        List<string> newAssets = new List<string>();
        foreach (T asset in assets)
        {
            if (asset != null)
            {   
                if (!mAssetLookup.ContainsKey(asset.name))
                {
                    mAssetLookup.Add(asset.name, asset);
                    newAssets.Add(asset.name);
                    Logger.Log(LogTag.Assets, mManagerName, "::LoadAssets() Asset Loaded " + asset.name);
                }
                else
                {
                    Logger.Log(LogTag.Assets, mManagerName, "::LoadAssets() Already contains an entry at " + asset.name, LogLevel.Warning);
                }
            }
            else
            {
                Logger.Log(LogTag.Assets, mManagerName, "::LoadAssets() found asset not of type in folder - " + asset.name, LogLevel.Warning);
            }

        }

        if (newAssets.Count == 0)
        {
            Logger.Log(LogTag.Assets, mManagerName, "::LoadAssets() " + folderPath + " contained no asset files of type.", LogLevel.Warning);
        }
        else
        {
            Logger.Log(LogTag.Assets, mManagerName, "::LoadAssets() Loaded " + newAssets.Count + " assets");
        }

        mLoadedDirectories.Add(folderPath, newAssets);
    }

    public void UnloadAssets(string folderPath)
    {
        Logger.Log(LogTag.Assets, mManagerName, "::UnloadAssets() " + folderPath);
        if (mLoadedDirectories.ContainsKey(folderPath))
        {
            List<string> loadedAssets = mLoadedDirectories[folderPath];
            mLoadedDirectories.Remove(folderPath);

            foreach (string loadedAsset in loadedAssets)
            {
                T asset = mAssetLookup[loadedAsset];
                mAssetLookup.Remove(loadedAsset);
                UnityEngine.Resources.UnloadAsset(asset);
            }
            Logger.Log(LogTag.Assets, mManagerName, "::UnloadAssets() Unloaded " + loadedAssets.Count + " assets");
        }
        else
        {
            Logger.Log(LogTag.Assets, mManagerName, "::UnloadAssets() no assets were loaded from " + folderPath);
        }
    }

    public virtual T GetAsset(string assetName)
    {
        Logger.Log(LogTag.Assets, mManagerName, "::GetAssetAt() " + assetName);

        T asset = null;

        if (mAssetLookup.ContainsKey(assetName))
        {
            asset = mAssetLookup[assetName];
        }
        else
        {
            Logger.Log(LogTag.Assets, mManagerName, "::GetAssetAt() - Attempting to access asset that isn't loaded - " + assetName, LogLevel.Error);
        }
        
        return asset;
    }
}
