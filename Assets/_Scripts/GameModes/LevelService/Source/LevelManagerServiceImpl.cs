using MAGE.GameModes.FlowControl;
using MAGE.GameModes.SceneElements;
using MAGE.GameSystems;
using MAGE.GameSystems.Appearances;
using MAGE.GameSystems.Characters;
using MAGE.GameSystems.World;
using MAGE.Messaging;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace MAGE.GameModes.LevelManagement.Internal
{
    class LevelManagerServiceImpl 
        : FlowControl.FlowControlBase
        , ILevelManagerService
        
    {
        private readonly string TAG = "LevelManagerServiceImpl";

        private Level mLoadedLevel = null;
        private AssetLoader<Level> mLevelLoader = null;

        // Flow Control
        public override FlowControlId GetFlowControlId()
        {
            return FlowControlId.Level;
        }

        protected override void Setup()
        {
            LevelManagementService.Register(this);

            DBService.Get().RegisterForAppearanceUpdates(this, OnAppearanceDBUpdated);
            DBService.Get().RegisterForCinematicUpdates(this, OnCinematicDBUpdated);
            DBService.Get().RegisterForEncounterUpdates(this, OnEncounterDBUpdated);
            DBService.Get().RegisterForPropUpdates(this, OnPropDBUpdated);
        }

        protected override void Cleanup()
        {
            DBService.Get().UnRegisterForAppearanceUpdates(this);
            DBService.Get().UnRegisterForCinematicUpdates(this);
            DBService.Get().UnRegisterForEncounterUpdates(this);
            DBService.Get().UnRegisterForPropUpdates(this);

            LevelManagementService.UnRegister();
        }

        public override bool Notify(string notifyEvent)
        {
            bool handled = false;
            switch (notifyEvent)
            {
                case "loadLevel":
                {
                    PartyLocation location = WorldService.Get().GetPartyLocation();
                    LoadLevel(location.Level);

                    handled = true;
                }
                break;
            }

            return handled;
        }

        public override string Query(string queryEvent)
        { 
            string queryResult = "";

            switch (queryEvent)
            {
                case "levelFlowType":
                {
                    if (mLoadedLevel.LevelId != WorldService.Get().GetPartyLocation().Level)
                    {
                        queryResult = "travel";
                    }
                    else if (mLoadedLevel.GetActiveCinematics().Count > 0)
                    {
                        queryResult = "cinematic";
                    }
                    else if (mLoadedLevel.GetActiveEncounters().Count > 0)
                    {
                        queryResult = "encounter";
                    }
                    else
                    {
                        queryResult = "explore";
                    }
                }
                break;
            }

            return queryResult;
        }


        // IMessageHandler
        public override void HandleMessage(MessageInfoBase eventInfoBase)
        {
            switch (eventInfoBase.MessageId)
            {
                case (LevelMessage.Id):
                {
                    LevelMessage levelMessage = eventInfoBase as LevelMessage;
                    switch (levelMessage.Type)
                    {
                        case (MessageType.CinematicAvailable):
                        {
                            SendFlowMessage("cinematicAvailable");
                        }
                        break;

                        case (MessageType.EncounterAvailable):
                        {
                            SendFlowMessage("encounterAvailable");
                        }
                        break;

                        case (MessageType.CinematicComplete):
                        {
                            CinematicId completedCinematic = levelMessage.Arg<CinematicMoment>().CinematicId;

                            DB.DBCinematicInfo info = DBService.Get().LoadCinematicInfo((int)completedCinematic);
                            info.IsActive = false;
                            DBService.Get().WriteCinematicInfo((int)completedCinematic, info);

                            StoryService.Get().NotifyStoryEvent(new GameSystems.Story.StoryEventBase(completedCinematic));
                        }
                        break;

                        case (MessageType.EncounterComplete):
                        {
                            EncounterScenarioId completedEncounter = levelMessage.Arg<EncounterContainer>().EncounterScenarioId;

                            DB.DBEncounterInfo info = DBService.Get().LoadEncounterInfo((int)completedEncounter);
                            info.IsActive = false;
                            DBService.Get().WriteEncounterInfo((int)completedEncounter, info);

                            StoryService.Get().NotifyStoryEvent(new GameSystems.Story.StoryEventBase(completedEncounter));
                        }
                        break;
                    }

                }
                break;
            }

        }

        // DB Updates
        public void OnAppearanceDBUpdated(int appearanceId)
        {
            Messaging.MessageRouter.Instance.NotifyMessage(new LevelMessage(MessageType.AppearanceUpdated, appearanceId));
        }

        public void OnCinematicDBUpdated(int cinematicId)
        {
            Messaging.MessageRouter.Instance.NotifyMessage(new LevelMessage(MessageType.CinematicUpdated, cinematicId));
        }

        public void OnEncounterDBUpdated(int encounterId)
        {
            Messaging.MessageRouter.Instance.NotifyMessage(new LevelMessage(MessageType.EncounterUpdated, encounterId));
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
            return AppearanceUtil.FromDB(DBService.Get().LoadAppearance(apperanceId));
        }

        public CinematicInfo GetCinematicInfo(int cinematicId)
        {
            return CinematicUtil.FromDB(DBService.Get().LoadCinematicInfo(cinematicId));
        }

        public EncounterInfo GetEncounterInfo(int encounterId)
        {
            return EncounterUtil.FromDB(DBService.Get().LoadEncounterInfo(encounterId));
        }

        public Level GetLoadedLevel()
        {
            return mLoadedLevel;
        }

        public Appearance GetNPCAppearance(NPCPropId npcId)
        {
            // I don't like this..
            int appearanceId = GetPropInfo((int)npcId).AppearanceId;
            if (CharacterUtil.GetCharacterTypeFromId(appearanceId) == CharacterType.Story)
            {
                return CharacterService.Get().GetCharacter(appearanceId).GetAppearance();
            }
            else
            {
                return AppearanceUtil.FromDB(DBService.Get().LoadAppearance(appearanceId));
            }
        }

        public PropInfo GetPropInfo(int propId)
        {
            return PropUtil.FromDB(DBService.Get().LoadPropInfo(propId));
        }

        public void LoadLevel(LevelId levelId)
        {
            if (SceneManager.GetActiveScene().name != levelId.ToString())
            {
                StartCoroutine(_LoadLevel(SceneManager.LoadSceneAsync(levelId.ToString(), LoadSceneMode.Single)));
            }
            else
            {
                OnLevelLoaded();
            }
        }

        private IEnumerator _LoadLevel(AsyncOperation asyncOp)
        {
            yield return asyncOp;

            OnLevelLoaded();
        }

        private void OnLevelLoaded()
        {
            mLoadedLevel = FindObjectOfType<Level>();
            AudioManager.Instance.PlayTrack(mLoadedLevel.TrackId);

            Messaging.MessageRouter.Instance.NotifyMessage(new LevelMessage(MessageType.LevelLoaded));
            SendFlowMessage("levelLoaded");
        }

        public void UnloadLevel()
        {
            if (mLoadedLevel != null)
            {
                AudioManager.Instance.StopAllTracks();

                Destroy(mLoadedLevel.gameObject);
                mLoadedLevel = null;
            }
        }

        public void UpdateEncounterInfo(EncounterInfo updatedInfo)
        {
            DBService.Get().WriteEncounterInfo((int)updatedInfo.EncounterScenarioId, EncounterUtil.ToDB(updatedInfo));
        }

        public void UpdatePropInfo(SceneElements.PropInfo updatedInfo)
        {
            DBService.Get().WritePropInfo(updatedInfo.Tag.Id, PropUtil.ToDB(updatedInfo));
        }

        
    }
}
