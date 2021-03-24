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
    class TurnFlowControl
    : TurnFlowControlBase
    , IInputHandler
    {
        const int CONFIRM_BUTTON_IDX = 0;
        const int CANCEL_BUTTON_IDX = 1;

        const int ACT_BUTTON_IDX = 0;
        const int MOVE_BUTTON_IDX = 1;
        const int WAIT_BUTTON_IDX = 2;

        string Tag = "TurnFlowControl";

        
        TileControl mHoveredTile = null;
        List<TileControl> mValidHoverSelections = null;
        RangeInfo mHoverRangeInfo = RangeInfo.Unit;

        TileControl mSelectedTile = null;
        ActionInfoBase mSelectedAction = null;

        Character mTargetedCharacter = null;

        // movement
        MapPathFinder mMovementPathFinder = new MapPathFinder();
        ActionTileCalculator mActionCalculator;

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

        protected override void OnInit()
        {
            mActionCalculator = new ActionTileCalculator(EncounterFlowControl_Deprecated.MapControl.Map);
            mTeam = TeamSide.AllyHuman;
        }

        protected override void Cleanup()
        {
            Input.InputManager.Instance.RegisterHandler(this, false);
            UIManager.Instance.RemoveOverlay(UIContainerId.ActorActionsView);
        }

        protected override void ProgressTurn(Character character)
        {
            mCharacter = character;
            FocusCharacter(character);
            mState = TurnState.SelectAction;

            mMovementPathFinder.CalculatePaths(EncounterFlowControl_Deprecated.MapControl.Map
                , EncounterFlowControl_Deprecated.MapControl.Map.GetCharacterPosition(mCharacter).Location
                , mCharacter.TeamSide
                , mCharacter.CurrentAttributes[TertiaryStat.Movement]
                , mCharacter.CurrentAttributes[TertiaryStat.Jump]);

            Input.InputManager.Instance.RegisterHandler(this, false);
            UIManager.Instance.PostContainer(UIContainerId.ActorActionsView, this);

            ShowCharacterPanel(InfoPanelSide.LEFT, mCharacter);
        }


        // UIContainerControl
        public override void HandleComponentInteraction(int containerId, UIInteractionInfo interactionInfo)
        {
            base.HandleComponentInteraction(containerId, interactionInfo);

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
                    
                    mValidHoverSelections = 
                        EncounterFlowControl_Deprecated.MapControl.GetTiles(mMovementPathFinder.GetPossibleTiles());
                    AddTileSelection(mValidHoverSelections, TileControl.HighlightState.MovementSelect);

                    AddTileSelection(new List<TileControl>(), TileControl.HighlightState.AOESelect);
                    mHoverRangeInfo = RangeInfo.Unit;

                    UpdateHoveredTile(mHoveredTile);

                    ToggleSelectedTiles(true);

                    UIManager.Instance.Publish(UIContainerId.ActorActionsView);
                }

                break;

                case (WAIT_BUTTON_IDX):
                {
                    OnActionSelected();
                    Messaging.MessageRouter.Instance.NotifyMessage(new EncounterMessage(EncounterMessage.EventType.TurnFinished));
                }
                break;
            }
        }

        private void HandleSelectAbilityInput(int selectedAbility)
        {
            if (selectedAbility < mCharacter.GetActionIds().Count)
            {
                mSelectedAction = mCharacter.GetActionInfo(mCharacter.GetActionIds()[selectedAbility]);

                if (mSelectedAction.IsSelfCast)
                {
                    mState = TurnState.ConfirmAbilityTarget;
                    mHoveredTile = EncounterFlowControl_Deprecated.MapControl[EncounterFlowControl_Deprecated.MapControl.Map.GetCharacterPosition(mCharacter).Location];
                    mValidHoverSelections = new List<TileControl>() { mHoveredTile };

                    List<TileIdx> autoSelection = mActionCalculator.CalculateTilesInRange(
                        EncounterFlowControl_Deprecated.MapControl.Map.GetCharacterPosition(mCharacter).Location,
                        EncounterFlowControl_Deprecated.MapControl.Map.GetCharacterPosition(mCharacter).Location,
                        mSelectedAction.EffectRange
                        , mCharacter.TeamSide);

                    AddTileSelection(EncounterFlowControl_Deprecated.MapControl.GetTiles(autoSelection), TileControl.HighlightState.AOESelect);
                }
                else
                {
                    mState = TurnState.SelectAbilityTarget;
                    mHoverRangeInfo = mSelectedAction.EffectRange;

                    mValidHoverSelections = EncounterFlowControl_Deprecated.MapControl.GetTiles(
                        mActionCalculator.CalculateTilesInRange(
                        EncounterFlowControl_Deprecated.MapControl.Map.GetCharacterPosition(mCharacter).Location,
                        EncounterFlowControl_Deprecated.MapControl.Map.GetCharacterPosition(mCharacter).Location,
                        mSelectedAction.CastRange
                        , mCharacter.TeamSide));

                    AddTileSelection(mValidHoverSelections, TileControl.HighlightState.TargetSelect);

                    AddTileSelection(new List<TileControl>(), TileControl.HighlightState.AOESelect);

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
                mCharacter.UpdateOnMoved();

                //List<TileControl> tilePath = EncounterFlowControl_Deprecated.MapControl.GetTiles(mMovementPathFinder.GetPathTo(mSelectedTile.Idx));
                //List<Transform> route = new List<Transform>();
                //foreach (TileControl tile in tilePath) { route.Add(tile.transform); }

                //EncounterFlowControl_Deprecated.MovementDirector.DirectMovement(
                //    EncounterFlowControl_Deprecated.CharacterDirector.GetController(mCharacter).transform,
                //    route, () =>
                //    {
                //        Messaging.MessageRouter.Instance.NotifyMessage(new EncounterMessage(EncounterMessage.EventType.MoveResolved));
                //        // TODO: figure out orrientation
                //        CharacterPosition newPos = new CharacterPosition();
                //        newPos.Location = mSelectedTile.Idx;
                //        newPos.Orientation = EncounterFlowControl_Deprecated.CharacterDirector.GetController(mCharacter).GetOrientation();
                //        EncounterFlowControl_Deprecated.CharacterDirector.UpdateCharacterPosition(mCharacter, newPos);
                //    });

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
                mCharacter.UpdateOnActed();

                ActionProposal_Deprecated actionProposal = new ActionProposal_Deprecated(
                    mCharacter,
                    mSelectedAction.ActionId,
                    new TargetSelection(new Target(mSelectedTile.Idx), mHoverRangeInfo)); // GROSs

                OnActionSelected();

                EncounterFlowControl_Deprecated.ActionDirector.DirectAction(actionProposal);
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

        private void HandleAbilityTargetSelected()
        {
            mState = TurnState.ConfirmAbilityTarget;
            mSelectedTile = mHoveredTile;
            UIManager.Instance.Publish(UIContainerId.ActorActionsView);
        }

        private void HandleMovementTargetSelected()
        {
            mState = TurnState.ConfirmMovementTarget;
            mSelectedTile = mHoveredTile;
            UIManager.Instance.Publish(UIContainerId.ActorActionsView);
        }

        public override IDataProvider Publish(int containerId)
        {
            IDataProvider dataProvider = base.Publish(containerId);

            if (dataProvider == null)
            {
                switch ((UIContainerId)containerId)
                {
                    case UIContainerId.ActorActionsView:
                        dataProvider = PublishActorActions();
                        break;
                }
            }

            return dataProvider;
        }

        IDataProvider PublishActorActions()
        {
            List<IDataProvider> buttonsInState = new List<IDataProvider>();

            switch (mState)
            {
                case (TurnState.SelectAction):
                    buttonsInState.Add(new UIButton.DataProvider("Act", mCharacter.CanAct));
                    buttonsInState.Add(new UIButton.DataProvider("Move", mCharacter.CanMove));
                    buttonsInState.Add(new UIButton.DataProvider("Wait", true));
                    break;

                case (TurnState.SelectAbility):
                    foreach (ActionId action in mCharacter.GetActionIds())
                    {
                        ActionInfoBase info = mCharacter.GetActionInfo(action);
                        bool canCastAction = info.CanCast(mCharacter);
                        buttonsInState.Add(new UIButton.DataProvider(action.ToString(), canCastAction));
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
            TileControl tile = GetTileFromObj(mouseHover);

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
                    HandleAbilityTargetSelected();
                    
                }
                else if (mState == TurnState.SelectMovementLocation)
                {
                    HandleMovementTargetSelected();
                }
            }
        }

        public void OnMouseScrolled(float scrollDelta)
        {
            // empty
        }

        // Helpers
        private bool IsValidHoveredTile(TileControl hoveredTile)
        {
            return hoveredTile != null && mValidHoverSelections.Contains(hoveredTile);
        }

        private void UpdateTargetedCharacter(Character targeted)
        {
            if (mTargetedCharacter != null)
            {
                if (mTargetedCharacter != targeted)
                {
                    UIManager.Instance.Publish(UIContainerId.EncounterCharacterInfoRightView);
                }
            }
            else if (targeted != null)
            {
                UIManager.Instance.Publish(UIContainerId.EncounterCharacterInfoRightView);
            }
        }

        private void UpdateHoveredTile(TileControl hoveredTile)
        {
            if (IsValidHoveredTile(hoveredTile))
            {
                mHoveredTile = hoveredTile;
            }
            else
            {
                mHoveredTile = null;
            }

            if (mState == TurnState.SelectAbilityTarget)
            {
                if (mHoveredTile != null)
                {
                    Character onTile = EncounterFlowControl_Deprecated.MapControl.Map.TileAt(mHoveredTile.Idx).OnTile;
                    if (onTile != null)
                    {
                        ShowCharacterPanel(InfoPanelSide.RIGHT, onTile);
                    }
                    else
                    {
                        HideInfoPanel(InfoPanelSide.RIGHT);
                    }
                }
                else
                {
                    HideInfoPanel(InfoPanelSide.RIGHT);
                }
            }

            List<TileControl> hoverSelection = new List<TileControl>();
            if (mHoveredTile != null)
            {
                hoverSelection = EncounterFlowControl_Deprecated.MapControl.GetTiles(mActionCalculator.CalculateTilesInRange(
                    EncounterFlowControl_Deprecated.MapControl.Map.GetCharacterPosition(mCharacter).Location,
                     hoveredTile.Idx,
                    mHoverRangeInfo
                    , mCharacter.TeamSide));
            }

            mSelectionStack.UpdateLayer(mSelectionStack.Count - 1, hoverSelection);
        }

        private TileControl GetTileFromObj(GameObject obj)
        {
            TileControl tile = null;

            if (obj != null)
            {
                tile = obj.GetComponentInParent<TileControl>();
            }

            return tile;
        }

        private void OnActionSelected()
        {
            ClearTileSelections();
            Input.InputManager.Instance.ReleaseHandler(this);
            UIManager.Instance.RemoveOverlay(UIContainerId.ActorActionsView);
            HideInfoPanel(InfoPanelSide.LEFT);
            HideInfoPanel(InfoPanelSide.RIGHT);
        }
    }
}