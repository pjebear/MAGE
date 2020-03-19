﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

class Appearance
{
    public static int NO_ASSET = -1;
    private int[] mAssetIds;
    public Appearance()
    {
        mAssetIds = Enumerable.Repeat(NO_ASSET, (int)AppearanceType.NUM).ToArray();
    }

    public Appearance(Appearance toCopy)
        : this()
    {
        toCopy.mAssetIds.CopyTo(mAssetIds, 0);
    }

    public Appearance(int[] assetIds)
    {
        mAssetIds = assetIds;
    }

    public int this[AppearanceType type]
    {
        get { return mAssetIds[(int)type]; }
        set { mAssetIds[(int)type] = value; }
    }

    public string GetAssetName(AppearanceType type)
    {
        string assetName = "";
        int assetId = mAssetIds[(int)type];
        switch (type)
        {
            //case (AppearanceType.BodySprite):
            //    assetName = ((CharacterBodySpriteId)assetId).ToString();
            //    break;

            //case (AppearanceType.PortraitSprite):
            //    assetName = ((CharacterPortraitSpriteId)assetId).ToString();
            //    break;

            //case (AppearanceType.IconSprite):
            //    assetName = ((IconSpriteId)assetId).ToString();
            //    break;

            case (AppearanceType.Prefab):
                assetName = ((AppearancePrefabId)assetId).ToString();
                break;

            default:
                Debug.LogError("Appearance::GetAppearanceAssetName() - " + type.ToString() + " Not implemented");
                break;
        }
        return assetName;
    }
}
