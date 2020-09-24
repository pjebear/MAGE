using MAGE.GameModes.Encounter;
using MAGE.GameModes.SceneElements;
using MAGE.GameSystems;
using MAGE.GameSystems.Actions;
using MAGE.GameSystems.Characters;
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
        TileControl mHoveredTile = null;
        List<TileControl> mValidHoverSelections = null;
        RangeInfo mHoverRangeInfo = RangeInfo.Unit;

        TileControl mSelectedTile = null;
        ActionInfo mSelectedAction = null;

        // movement
        //MovementTileCalculator mMovementCalculator;
        MAGE.GameSystems.MapPathFinder mMovementPathFinder = new MapPathFinder();
        ActionTileCalculator mActionCalculator;

        private Character mCharacter;

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
            mActionCalculator = new ActionTileCalculator(EncounterFlowControl.MapControl.Map);
            Messaging.MessageRouter.Instance.RegisterHandler(this);
        }

        void OnDestroy()
        {
            Messaging.MessageRouter.Instance.UnRegisterHandler(this);
            Input.InputManager.Instance.RegisterHandler(this, false);

            UIManager.Instance.RemoveOverlay(UIContainerId.ActorActionsView);
            UIManager.Instance.RemoveOverlay(UIContainerId.EncounterCharacterInfoView);
        }

        public void ProgressTurn(Character character)
        {
            mCharacter = character;
            Transform actorTransform = EncounterFlowControl.CharacterDirector.GetController(character).transform;
            EncounterFlowControl.CameraDirector.FocusTarget(actorTransform);
            mState = TurnState.SelectAction;

            mMovementPathFinder.CalculatePaths(EncounterFlowControl.MapControl.Map
                , EncounterFlowControl.MapControl.Map.GetCharacterPosition(mCharacter).Location
                , mCharacter.TeamSide
                , mCharacter.CurrentAttributes[TertiaryStat.Movement]
                , mCharacter.CurrentAttributes[TertiaryStat.Jump]);

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
                    
                    mValidHoverSelections = 
                        EncounterFlowControl.MapControl.GetTiles(mMovementPathFinder.GetPossibleTiles());
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
                    mHoveredTile = EncounterFlowControl.MapControl[EncounterFlowControl.CharacterDirector.GetCharacterPosition(mCharacter)];
                    mValidHoverSelections = new List<TileControl>() { mHoveredTile };

                    List<Tile> autoSelection = mActionCalculator.CalculateTilesInRange(
                        EncounterFlowControl.CharacterDirector.GetCharacterPosition(mCharacter),
                        EncounterFlowControl.CharacterDirector.GetCharacterPosition(mCharacter),
                        mSelectedAction.EffectRange);

                    AddTileSelection(EncounterFlowControl.MapControl.GetTiles(autoSelection), TileControl.HighlightState.AOESelect);
                }
                else
                {
                    mState = TurnState.SelectAbilityTarget;
                    mHoverRangeInfo = mSelectedAction.EffectRange;

                    mValidHoverSelections = EncounterFlowControl.MapControl.GetTiles(
                        mActionCalculator.CalculateTilesInRange(
                        EncounterFlowControl.CharacterDirector.GetCharacterPosition(mCharacter),
                        EncounterFlowControl.CharacterDirector.GetCharacterPosition(mCharacter),
                        mSelectedAction.CastRange));

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


                List<TileControl> tilePath = EncounterFlowControl.MapControl.GetTiles(mMovementPathFinder.GetPathTo(mSelectedTile.Idx));
                List<Transform> route = new List<Transform>();
                foreach (TileControl tile in tilePath) { route.Add(tile.transform); }

                EncounterFlowControl.MovementDirector.DirectMovement(
                    EncounterFlowControl.CharacterDirector.GetController(mCharacter).ActorController,
                    route, () =>
                    {
                        Messaging.MessageRouter.Instance.NotifyMessage(new EncounterMessage(EncounterMessage.EventType.MoveResolved));
                        // TODO: figure out orrientation
                        CharacterPosition newPos = new CharacterPosition();
                        newPos.Location = mSelectedTile.Idx;
                        newPos.Orientation = EncounterFlowControl.CharacterDirector.GetController(mCharacter).GetOrientation();
                        EncounterFlowControl.CharacterDirector.UpdateCharacterPosition(mCharacter, newPos);
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
                mCharacter.UpdateOnActed();

                ActionProposal actionProposal = new ActionProposal(
                    mCharacter,
                    mSelectedAction.ActionId,
                    new TargetSelection(new Target(mSelectedTile.Idx), mHoverRangeInfo)); // GROSs

                OnActionSelected();

                EncounterFlowControl.ActionDirector.DirectAction(actionProposal);
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

            dp.IsAlly = mCharacter.TeamSide == TeamSide.AllyHuman;
            dp.PortraitAsset = mCharacter.GetAppearance().PortraitSpriteId.ToString();
            dp.Name = mCharacter.Name;
            dp.Level = mCharacter.Level;
            dp.Exp = mCharacter.Experience;
            dp.Specialization = mCharacter.CurrentSpecializationType.ToString();
            dp.CurrentHP = mCharacter.CurrentResources[ResourceType.Health].Current;
            dp.MaxHP = mCharacter.CurrentResources[ResourceType.Health].Max;
            dp.CurrentMP = mCharacter.CurrentResources[ResourceType.Mana].Current;
            dp.MaxMP = mCharacter.CurrentResources[ResourceType.Mana].Max;
            dp.Might = (int)mCharacter.CurrentAttributes[PrimaryStat.Might];
            dp.Finesse = (int)mCharacter.CurrentAttributes[PrimaryStat.Finese];
            dp.Magic = (int)mCharacter.CurrentAttributes[PrimaryStat.Magic];
            dp.Fortitude = (int)mCharacter.CurrentAttributes[SecondaryStat.Fortitude];
            dp.Attunement = (int)mCharacter.CurrentAttributes[SecondaryStat.Attunement];
            dp.Block = (int)mCharacter.CurrentAttributes[TertiaryStat.Block];
            dp.Dodge = (int)mCharacter.CurrentAttributes[TertiaryStat.Dodge];
            dp.Parry = (int)mCharacter.CurrentAttributes[TertiaryStat.Parry];

            List<IDataProvider> statusEffects = new List<IDataProvider>();
            foreach (StatusEffect effect in mCharacter.StatusEffects)
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
                    buttonsInState.Add(new UIButton.DataProvider("Act", mCharacter.CanAct));
                    buttonsInState.Add(new UIButton.DataProvider("Move", mCharacter.CanMove));
                    buttonsInState.Add(new UIButton.DataProvider("Wait", true));
                    break;

                case (TurnState.SelectAbility):
                    foreach (ActionId action in mCharacter.GetActionIds())
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

        private void AddTileSelection(List<TileControl> selection, TileControl.HighlightState highlight)
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

        private bool IsValidHoveredTile(TileControl hoveredTile)
        {
            return hoveredTile != null && mValidHoverSelections.Contains(hoveredTile);
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

            List<TileControl> hoverSelection = new List<TileControl>();
            if (mHoveredTile != null)
            {
                hoverSelection = EncounterFlowControl.MapControl.GetTiles(mActionCalculator.CalculateTilesInRange(
                    EncounterFlowControl.CharacterDirector.GetCharacterPosition(mCharacter),
                     hoveredTile.Idx,
                    mHoverRangeInfo));
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
                            ProgressTurn(message.Arg<Character>());
                            break;
                    }
                }
                break;
            }
        }
    }


}
