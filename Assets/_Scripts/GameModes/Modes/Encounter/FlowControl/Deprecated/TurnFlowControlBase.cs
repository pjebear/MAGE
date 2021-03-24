using MAGE.GameModes.Encounter;
using MAGE.GameModes.SceneElements;
using MAGE.GameSystems;
using MAGE.GameSystems.Actions;
using MAGE.GameSystems.Characters;
using MAGE.GameSystems.Stats;
using MAGE.UI;
using MAGE.UI.Views;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Events;

namespace MAGE.GameModes.FlowControl
{
    abstract class TurnFlowControlBase
        : MonoBehaviour
        , UIContainerControl
        , Messaging.IMessageHandler
    {
        protected enum InfoPanelSide
        {
            LEFT,
            RIGHT,

            NUM
        }


        string Tag = "TurnFlowControlBase";

        protected bool mDisplaySelections = false;
        protected TileSelectionStack mSelectionStack;

        protected Character mCharacter;
        protected Character mTargetedCharacter;
        protected TeamSide mTeam;

        private Character mLeftPanelCharacter;
        private Character mRightPanelCharacter;


        public virtual void Init()
        {
            mSelectionStack = new TileSelectionStack();
            Messaging.MessageRouter.Instance.RegisterHandler(this);

            OnInit();
        }
        protected abstract void OnInit();

        void OnDestroy()
        {
            Messaging.MessageRouter.Instance.UnRegisterHandler(this);
            UIManager.Instance.RemoveOverlay(UIContainerId.EncounterCharacterInfoLeftView);
            UIManager.Instance.RemoveOverlay(UIContainerId.EncounterCharacterInfoRightView);

            Cleanup();
        }
        protected abstract void Cleanup();

        protected abstract void ProgressTurn(Character character);

        protected void FocusCharacter(Character character)
        {
            //Transform actorTransform = EncounterFlowControl_Deprecated.CharacterDirector.GetController(character).transform;
            //EncounterFlowControl_Deprecated.CameraDirector.FocusTarget(actorTransform);
        }

        // UIContainerControl
        public virtual void HandleComponentInteraction(int containerId, UIInteractionInfo interactionInfo)
        {
            // empty
        }

        public virtual IDataProvider Publish(int containerId)
        {
            IDataProvider dataProvider = null;

            switch ((UIContainerId)containerId)
            {
                case UIContainerId.EncounterCharacterInfoLeftView:
                    dataProvider = PublishCharacterInfo(mLeftPanelCharacter);
                    break;
                case UIContainerId.EncounterCharacterInfoRightView:
                    dataProvider = PublishCharacterInfo(mRightPanelCharacter);
                    break;
            }

            return dataProvider;
        }

        protected void ShowCharacterPanel(InfoPanelSide side, Character toShow)
        {
            switch (side)
            {
                case InfoPanelSide.LEFT:
                {
                    if (mLeftPanelCharacter != null)
                    {
                        UIManager.Instance.Publish(UIContainerId.EncounterCharacterInfoLeftView);
                    }
                    else
                    {
                        UIManager.Instance.PostContainer(UIContainerId.EncounterCharacterInfoLeftView, this);
                    }
                    mLeftPanelCharacter = toShow;
                }
                break;
                case InfoPanelSide.RIGHT:
                {
                    if (mRightPanelCharacter != null)
                    {
                        UIManager.Instance.Publish(UIContainerId.EncounterCharacterInfoRightView);
                    }
                    else
                    {
                        UIManager.Instance.PostContainer(UIContainerId.EncounterCharacterInfoRightView, this);
                    }
                    mRightPanelCharacter = toShow;
                }
                break;
            }
        }

        protected void HideInfoPanel(InfoPanelSide side)
        {
            switch (side)
            {
                case InfoPanelSide.LEFT: UIManager.Instance.RemoveOverlay(UIContainerId.EncounterCharacterInfoLeftView); mLeftPanelCharacter = null; break;
                case InfoPanelSide.RIGHT: UIManager.Instance.RemoveOverlay(UIContainerId.EncounterCharacterInfoRightView); mRightPanelCharacter = null; break;
            }
        }

        IDataProvider PublishCharacterInfo(Character toPublish)
        {
            CharacterInspector.DataProvider dp = new CharacterInspector.DataProvider();

            dp.IsAlly = toPublish.TeamSide == TeamSide.AllyHuman;
            dp.PortraitAsset = toPublish.GetAppearance().PortraitSpriteId.ToString();
            dp.Name = toPublish.Name;
            dp.Level = toPublish.Level;
            dp.Exp = toPublish.Experience;
            dp.Specialization = toPublish.CurrentSpecializationType.ToString();
            dp.CurrentHP = toPublish.CurrentResources[ResourceType.Health].Current;
            dp.MaxHP = toPublish.CurrentResources[ResourceType.Health].Max;
            dp.CurrentMP = toPublish.CurrentResources[ResourceType.Mana].Current;
            dp.MaxMP = toPublish.CurrentResources[ResourceType.Mana].Max;
            dp.Might = (int)toPublish.CurrentAttributes[PrimaryStat.Might];
            dp.Finesse = (int)toPublish.CurrentAttributes[PrimaryStat.Finese];
            dp.Magic = (int)toPublish.CurrentAttributes[PrimaryStat.Magic];
            dp.Fortitude = (int)toPublish.CurrentAttributes[SecondaryStat.Fortitude];
            dp.Attunement = (int)toPublish.CurrentAttributes[SecondaryStat.Attunement];
            dp.Block = (int)toPublish.CurrentAttributes[TertiaryStat.Block];
            dp.Dodge = (int)toPublish.CurrentAttributes[TertiaryStat.Dodge];
            dp.Parry = (int)toPublish.CurrentAttributes[TertiaryStat.Parry];

            List<IDataProvider> statusEffects = new List<IDataProvider>();
            foreach (StatusEffect effect in toPublish.StatusEffects)
            {
                StatusIcon.DataProvider statusDp = new StatusIcon.DataProvider();
                statusDp.Count = effect.StackCount;
                statusDp.IsBeneficial = effect.Beneficial;
                statusDp.AssetName = effect.SpriteId.ToString();
                statusEffects.Add(statusDp);
            }
            dp.StatusEffects = new UIList.DataProvider(statusEffects);

            return dp;
        }


        public string Name()
        {
            return Tag;
        }

        // Helpers
        protected void ToggleSelectedTiles(bool visible)
        {
            mDisplaySelections = visible;
            if (mDisplaySelections)
            {
                mSelectionStack.DisplayTiles();
            }
            else
            {
                mSelectionStack.HideTiles();
            }
        }

        protected void AddTileSelection(List<TileControl> selection, TileControl.HighlightState highlight)
        {
            mSelectionStack.AddLayer(selection, highlight);
            if (mDisplaySelections)
            {
                mSelectionStack.RefreshDisplay();
            }
        }

        protected void PopTileSelection()
        {
            mSelectionStack.RemoveLayer();
            if (mDisplaySelections)
            {
                mSelectionStack.RefreshDisplay();
            }
        }

        protected void ClearTileSelections()
        {
            mSelectionStack.Reset();
        }

        // Messaging.IMessageHandler
        public void HandleMessage(Messaging.MessageInfoBase messageInfoBase)
        {
            switch (messageInfoBase.MessageId)
            {
                case EncounterMessage.Id:
                {
                    EncounterMessage message = messageInfoBase as EncounterMessage;

                    switch (message.Type)
                    {   
                        case EncounterMessage.EventType.TurnBegun:
                        {
                            Character character = message.Arg<Character>();
                            if (character.TeamSide == mTeam)
                            {
                                ProgressTurn(character);
                            }
                        }
                        break;
                    }
                }
                break;
            }
        }
    }


}
