using MAGE.GameModes.Encounter;
using MAGE.GameModes.SceneElements;
using MAGE.GameServices;

using MAGE.GameServices.Character;
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
    class TurnFlowControl
    : MonoBehaviour
    , UIContainerControl
    , IInputHandler
    , Messaging.IMessageHandler
    {
        const int CONFIRM_BUTTON_IDX = 0;
        const int CANCEL_BUTTON_IDX = 1;

        const int ACT_BUTTON_IDX = 0;
        const int MOVE_BUTTON_IDX = 1;
        const int WAIT_BUTTON_IDX = 2;

        string Tag = "TurnFlowControl";

        TileSelectionStack mSelectionStack;

        bool mDisplaySelections = false;
        Tile mHoveredTile = null;
        List<Tile> mValidHoverSelections = null;
        RangeInfo mHoverRangeInfo = RangeInfo.Unit;

        Tile mSelectedTile = null;
        ActionInfo mSelectedAction = null;

        // movement
        MovementTileCalculator mMovementCalculator;
        ActionTileCalculator mActionCalculator;

        private EncounterCharacter mActor;

        enum TurnState
        {
            SelectAction,
            SelectAbility,
            SelectAbilityTarget,
            ConfirmAbilityTarget,
            SelectMovementLocation,
            ConfirmMovementTarget,

            NUM
        }
        TurnState mState;

        public void Init()
        {
            mSelectionStack = new TileSelectionStack();
            mMovementCalculator = new MovementTileCalculator(EncounterModule.Map);
            mActionCalculator = new ActionTileCalculator(EncounterModule.Map);
            Messaging.MessageRouter.Instance.RegisterHandler(this);
        }

        void OnDestroy()
        {
            Messaging.MessageRouter.Instance.UnRegisterHandler(this);
        }

        public void ProgressTurn(EncounterCharacter actor)
        {
            mActor = actor;
            Transform actorTransform = EncounterModule.CharacterDirector.GetController(actor).transform;
            EncounterModule.CameraDirector.FocusTarget(actorTransform);
            mState = TurnState.SelectAction;

            mMovementCalculator.CalculatePathTiles(
                EncounterModule.Map[EncounterModule.CharacterDirector.GetActorPosition(mActor)],
                (int)mActor.Attributes[TertiaryStat.Movement],
                (int)mActor.Attributes[TertiaryStat.Jump],
                mActor.Team);

            Input.InputManager.Instance.RegisterHandler(this, false);
            UIManager.Instance.PostContainer(UIContainerId.ActorActionsView, this);
            UIManager.Instance.PostContainer(UIContainerId.EncounterCharacterInfoView, this);
        }


        // UIContainerControl
        public void HandleComponentInteraction(int containerId, UIInteractionInfo interactionInfo)
        {
            switch ((UIContainerId)containerId)
            {
                case (UIContainerId.ActorActionsView):
                    HandleActionSelectInteraction(interactionInfo);
                    break;
            }
        }

        private void HandleActionSelectInteraction(UIInteractionInfo interactionInfo)
        {
            if ((ActorActions.ComponentId)interactionInfo.ComponentId == ActorActions.ComponentId.ActionList
                && interactionInfo.InteractionType == UIInteractionType.Click)
            {
                ListInteractionInfo buttonListInfo = (ListInteractionInfo)interactionInfo;
                switch (mState)
                {
                    // User is selection to Act, Move or Wait
                    // No tiles are highlighted currently
                    case (TurnState.SelectAction):
                    {
                        HandleSelectActionInput(buttonListInfo.ListIdx);
                    }
                    break;
                    // User is selection a specific action
                    // No tiles are highlighted currently
                    case (TurnState.SelectAbility):
                    {
                        HandleSelectAbilityInput(buttonListInfo.ListIdx);
                    }
                    break;

                    // Cancel movement selection
                    case (TurnState.SelectMovementLocation):
                    {
                        Logger.Assert(buttonListInfo.ListIdx == 0, LogTag.FlowControl, Tag, "Unkonwn SelectMovementLocation button list input");

                        mState = TurnState.SelectAction;
                        ClearTileSelections();
                        UIManager.Instance.Publish(UIContainerId.ActorActionsView);
                    }
                    break;

                    // Cancel Ability selection
                    case (TurnState.SelectAbilityTarget):
                    {
                        Logger.Assert(buttonListInfo.ListIdx == 0, LogTag.FlowControl, Tag, "Unkonwn SelectMovementLocation button list input");

                        mState = TurnState.SelectAbility;

                        ClearTileSelections();
                        UIManager.Instance.Publish(UIContainerId.ActorActionsView);
                    }
                    break;

                    case (TurnState.ConfirmAbilityTarget):
                    {
                        HandleAbilityConfirmationInput(buttonListInfo.ListIdx == CONFIRM_BUTTON_IDX);
                    }
                    break;

                    case (TurnState.ConfirmMovementTarget):
                    {
                        HandleMovementConfirmationInput(buttonListInfo.ListIdx == CONFIRM_BUTTON_IDX);
                    }
                    break;
                }
            }
        }

        private void HandleSelectActionInput(int selectedAction)
        {
            switch (selectedAction)
            {
                case (ACT_BUTTON_IDX):
                {
                    mState = TurnState.SelectAbility;

                    UIManager.Instance.Publish(UIContainerId.ActorActionsView);
                }

                break;

                case (MOVE_BUTTON_IDX):
                {
                    mState = TurnState.SelectMovementLocation;
                    mValidHoverSelections = mMovementCalculator.GetValidMovementTiles();
                    AddTileSelection(mValidHoverSelections, Tile.HighlightState.MovementSelect);

                    AddTileSelection(new List<Tile>(), Tile.HighlightState.AOESelect);
                    mHoverRangeInfo = RangeInfo.Unit;

                    UpdateHoveredTile(mHoveredTile);

                    ToggleSelectedTiles(true);

                    UIManager.Instance.Publish(UIContainerId.ActorActionsView);
                }

                break;

                case (WAIT_BUTTON_IDX):
                {
                    mActor.DEBUG_IsTurnComplete = true;

                    OnActionSelected();
                    Messaging.MessageRouter.Instance.NotifyMessage(new EncounterMessage(EncounterMessage.EventType.TurnFinished));
                }
                break;
            }
        }

        private void HandleSelectAbilityInput(int selectedAbility)
        {
            if (selectedAbility < mActor.Actions.Count)
            {
                mSelectedAction = mActor.GetActionInfo(mActor.Actions[selectedAbility]);

                if (mSelectedAction.IsSelfCast)
                {
                    mState = TurnState.ConfirmAbilityTarget;
                    mHoveredTile = EncounterModule.Map[EncounterModule.CharacterDirector.GetActorPosition(mActor)];
                    mValidHoverSelections = new List<Tile>() { mHoveredTile };

                    List<Tile> autoSelection = mActionCalculator.CalculateTilesInRange(
                        EncounterModule.CharacterDirector.GetActorPosition(mActor),
                        EncounterModule.CharacterDirector.GetActorPosition(mActor),
                        mSelectedAction.EffectRange);

                    AddTileSelection(autoSelection, Tile.HighlightState.AOESelect);
                }
                else
                {
                    mState = TurnState.SelectAbilityTarget;
                    mHoverRangeInfo = mSelectedAction.EffectRange;

                    mValidHoverSelections = mActionCalculator.CalculateTilesInRange(
                        EncounterModule.CharacterDirector.GetActorPosition(mActor),
                        EncounterModule.CharacterDirector.GetActorPosition(mActor),
                        mSelectedAction.CastRange);

                    AddTileSelection(mValidHoverSelections, Tile.HighlightState.TargetSelect);

                    AddTileSelection(new List<Tile>(), Tile.HighlightState.AOESelect);

                    UpdateHoveredTile(mHoveredTile);
                }


                ToggleSelectedTiles(true);

                UIManager.Instance.Publish(UIContainerId.ActorActionsView);
            }
            else
            {
                mState = TurnState.SelectAction;

                UIManager.Instance.Publish(UIContainerId.ActorActionsView);
            }
        }

        private void HandleMovementConfirmationInput(bool confirmed)
        {
            if (confirmed)
            {
                mActor.DEBUG_HasMoved = true;

                List<Tile> tilePath = mMovementCalculator.GetPathTo(mSelectedTile);
                List<Transform> route = new List<Transform>();
                foreach (Tile tile in tilePath) { route.Add(tile.transform); }

                EncounterModule.MovementDirector.DirectMovement(
                    EncounterModule.CharacterDirector.GetController(mActor).ActorController,
                    route, () =>
                    {
                        Messaging.MessageRouter.Instance.NotifyMessage(new EncounterMessage(EncounterMessage.EventType.MoveResolved));
                        EncounterModule.CharacterDirector.UpdateCharacterPosition(mActor, mSelectedTile.Idx);
                    });

                OnActionSelected();
            }
            else
            {
                mState = TurnState.SelectMovementLocation;

                UIManager.Instance.Publish(UIContainerId.ActorActionsView);
            }
        }

        private void HandleAbilityConfirmationInput(bool confirmed)
        {
            if (confirmed)
            {
                mState = TurnState.SelectAction;
                mActor.DEBUG_HasActed = true;

                ActionProposal actionProposal = new ActionProposal(
                    mActor,
                    mSelectedAction.ActionId,
                    new TargetSelection(new Target(mSelectedTile.Idx), mHoverRangeInfo)); // GROSs

                OnActionSelected();

                EncounterModule.ActionDirector.DirectAction(actionProposal);
            }
            else
            {
                if (mSelectedAction.IsSelfCast)
                {
                    mState = TurnState.SelectAbility;
                    ClearTileSelections();
                }
                else
                {
                    mState = TurnState.SelectAbilityTarget;
                }

                UIManager.Instance.Publish(UIContainerId.ActorActionsView);
            }
        }

        public IDataProvider Publish(int containerId)
        {
            IDataProvider dataProvider = null;

            switch ((UIContainerId)containerId)
            {
                case UIContainerId.ActorActionsView:
                    dataProvider = PublishActorActions();
                    break;
                case UIContainerId.EncounterCharacterInfoView:
                    dataProvider = PublishCharacterInfo();
                    break;
            }

            return dataProvider;
        }

        IDataProvider PublishCharacterInfo()
        {
            EncounterCharacterInfoView.DataProvider dp = new EncounterCharacterInfoView.DataProvider();

            dp.IsAlly = mActor.Team == TeamSide.AllyHuman;
            dp.PortraitAsset = GameModes.LevelManagementService.Get().GetAppearance(mActor.AppearanceId).PortraitSpriteId.ToString();
            dp.Name = mActor.Name;
            dp.Level = mActor.Level;
            dp.Exp = mActor.Exp;
            dp.Specialization = mActor.Specialization.ToString();
            dp.CurrentHP = mActor.Resources[ResourceType.Health].Current;
            dp.MaxHP = mActor.Resources[ResourceType.Health].Max;
            dp.CurrentMP = mActor.Resources[ResourceType.Mana].Current;
            dp.MaxMP = mActor.Resources[ResourceType.Mana].Max;
            dp.Might = (int)mActor.Attributes[PrimaryStat.Might];
            dp.Finesse = (int)mActor.Attributes[PrimaryStat.Finese];
            dp.Magic = (int)mActor.Attributes[PrimaryStat.Magic];
            dp.Fortitude = (int)mActor.Attributes[SecondaryStat.Fortitude];
            dp.Attunement = (int)mActor.Attributes[SecondaryStat.Attunement];
            dp.Block = (int)mActor.Attributes[TertiaryStat.Block];
            dp.Dodge = (int)mActor.Attributes[TertiaryStat.Dodge];
            dp.Parry = (int)mActor.Attributes[TertiaryStat.Parry];

            List<IDataProvider> statusEffects = new List<IDataProvider>();
            foreach (StatusEffect effect in mActor.StatusEffects)
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

        IDataProvider PublishActorActions()
        {
            List<IDataProvider> buttonsInState = new List<IDataProvider>();

            switch (mState)
            {
                case (TurnState.SelectAction):
                    buttonsInState.Add(new UIButton.DataProvider("Act", !mActor.DEBUG_HasActed));
                    buttonsInState.Add(new UIButton.DataProvider("Move", !mActor.DEBUG_HasMoved));
                    buttonsInState.Add(new UIButton.DataProvider("Wait", true));
                    break;

                case (TurnState.SelectAbility):
                    foreach (ActionId action in mActor.Actions)
                    {
                        buttonsInState.Add(new UIButton.DataProvider(action.ToString(), true));
                    }

                    buttonsInState.Add(new UIButton.DataProvider("Back", true));
                    break;

                case (TurnState.SelectAbilityTarget):
                    buttonsInState.Add(new UIButton.DataProvider("Back", true));
                    break;
                case (TurnState.ConfirmAbilityTarget):
                    buttonsInState.Add(new UIButton.DataProvider("Yes", true));
                    buttonsInState.Add(new UIButton.DataProvider("No", true));
                    break;

                case (TurnState.SelectMovementLocation):
                    buttonsInState.Add(new UIButton.DataProvider("Back", true));
                    break;
                case (TurnState.ConfirmMovementTarget):
                    buttonsInState.Add(new UIButton.DataProvider("Yes", true));
                    buttonsInState.Add(new UIButton.DataProvider("No", true));
                    break;

            }

            ActorActions.DataProvider dp = new ActorActions.DataProvider();
            dp.ButtonListDP = new UIList.DataProvider(buttonsInState);

            return dp;
        }

        public string Name()
        {
            return Tag;
        }

        // IInputHandler
        public void OnMouseHoverChange(GameObject mouseHover)
        {
            Tile tile = GetTileFromObj(mouseHover);

            if (mState == TurnState.SelectAbilityTarget
                   || mState == TurnState.SelectMovementLocation)
            {
                UpdateHoveredTile(tile);
            }
        }

        public void OnKeyPressed(InputSource source, int key, InputState state)
        {
            if (IsValidHoveredTile(mHoveredTile)
                && source == InputSource.Mouse
                && (MouseKey)key == MouseKey.Left
                && state == InputState.Down)
            {
                if (mState == TurnState.SelectAbilityTarget)
                {
                    mState = TurnState.ConfirmAbilityTarget;
                    mSelectedTile = mHoveredTile;
                    UIManager.Instance.Publish(UIContainerId.ActorActionsView);
                }
                else if (mState == TurnState.SelectMovementLocation)
                {
                    mState = TurnState.ConfirmMovementTarget;
                    mSelectedTile = mHoveredTile;
                    UIManager.Instance.Publish(UIContainerId.ActorActionsView);
                }
            }
        }

        public void OnMouseScrolled(float scrollDelta)
        {
            // empty
        }

        // Helpers
        private void ToggleSelectedTiles(bool visible)
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

        private void AddTileSelection(List<Tile> selection, Tile.HighlightState highlight)
        {
            mSelectionStack.AddLayer(selection, highlight);
            if (mDisplaySelections)
            {
                mSelectionStack.RefreshDisplay();
            }
        }

        private void PopTileSelection()
        {
            mSelectionStack.RemoveLayer();
            if (mDisplaySelections)
            {
                mSelectionStack.RefreshDisplay();
            }
        }

        private void ClearTileSelections()
        {
            mSelectionStack.Reset();
        }

        private bool IsValidHoveredTile(Tile hoveredTile)
        {
            return hoveredTile != null && mValidHoverSelections.Contains(hoveredTile);
        }

        private void UpdateHoveredTile(Tile hoveredTile)
        {
            if (IsValidHoveredTile(hoveredTile))
            {
                mHoveredTile = hoveredTile;
            }
            else
            {
                mHoveredTile = null;
            }

            List<Tile> hoverSelection = new List<Tile>();
            if (mHoveredTile != null)
            {
                hoverSelection = mActionCalculator.CalculateTilesInRange(
                    EncounterModule.CharacterDirector.GetActorPosition(mActor),
                     hoveredTile.Idx,
                    mHoverRangeInfo);
            }

            mSelectionStack.UpdateLayer(mSelectionStack.Count - 1, hoverSelection);
        }

        private Tile GetTileFromObj(GameObject obj)
        {
            Tile tile = null;

            if (obj != null)
            {
                tile = obj.GetComponentInParent<Tile>();
            }

            return tile;
        }

        private void OnActionSelected()
        {
            ClearTileSelections();
            Input.InputManager.Instance.ReleaseHandler(this);
            UIManager.Instance.RemoveOverlay(UIContainerId.ActorActionsView);
            UIManager.Instance.RemoveOverlay(UIContainerId.EncounterCharacterInfoView);
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
                            ProgressTurn(message.Arg<EncounterCharacter>());
                            break;
                    }
                }
                break;
            }
        }
    }


}
