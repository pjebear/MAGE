﻿using MAGE.GameModes.SceneElements;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace MAGE.GameModes.LevelManagement.Internal
{
    class LevelManagerServiceImpl : MonoBehaviour, ILevelManagerService
    {
        private readonly string TAG = "LevelManagerServiceImpl";

        private Level mLoadedLevel = null;
        private AssetLoader<Level> mLevelLoader = null;

        public void Init()
        {
            GameServices.DBService.Get().RegisterForAppearanceUpdates(this, OnAppearanceDBUpdated);
            GameServices.DBService.Get().RegisterForPropUpdates(this, OnPropDBUpdated);
        }

        public void Takedown()
        {
            GameServices.DBService.Get().UnRegisterForAppearanceUpdates(this);
            GameServices.DBService.Get().UnRegisterForPropUpdates(this);
        }

        // DB Updates
        public void OnAppearanceDBUpdated(int appearanceId)
        {
            Messaging.MessageRouter.Instance.NotifyMessage(new LevelMessage(MessageType.AppearanceUpdated, appearanceId));
        }

        public void OnPropDBUpdated(int propId)
        {
            Messaging.MessageRouter.Instance.NotifyMessage(new LevelMessage(MessageType.PropUpdated, propId));
        }

        private void Awake()
        {
            mLevelLoader = new AssetLoader<Level>("Levels");
            mLevelLoader.LoadAssets();
        }

        public Appearance GetAppearance(int apperanceId)
        {
            return GameServices.AppearanceUtil.FromDB(GameServices.DBService.Get().LoadAppearance(apperanceId));
        }

        public Level GetLoadedLevel()
        {
            return mLoadedLevel;
        }

        public SceneElements.PropInfo GetPropInfo(int propId)
        {
            return SceneElements.PropUtil.FromDB(GameServices.DBService.Get().LoadPropInfo(propId));
        }

        public void LoadLevel(GameServices.LevelId levelId)
        {
            mLoadedLevel = Instantiate(mLevelLoader.GetAsset(levelId.ToString()));
        }

        public void NotifyLevelLoaded(Level level)
        {
            mLoadedLevel = level;
        }

        public void UnloadLevel()
        {
            if (mLoadedLevel != null)
            {
                Destroy(mLoadedLevel.gameObject);
                mLoadedLevel = null;
            }
        }

        public void UpdatePropInfo(SceneElements.PropInfo updatedInfo)
        {
            GameServices.DBService.Get().WritePropInfo(updatedInfo.Tag.Id, PropUtil.ToDB(updatedInfo));
        }
    }
}
