using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

class TurnFlowControl 
    : MonoBehaviour
    , UIContainerControl
    , IInputHandler
    , IEventHandler<EncounterEvent>
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
    int mActionIdxSelected = 0;

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

        EncounterEventRouter.Instance.RegisterHandler(this);
    }

    void OnDestroy()
    {
        EncounterEventRouter.Instance.UnRegisterListener(this);
    }

    public void ProgressTurn(EncounterCharacter actor)
    {
        mActor = actor;
        Transform actorTransform = EncounterModule.CharacterDirector.GetController(actor).transform;
        EncounterModule.CameraDirector.FocusTarget(actorTransform);
        mState = TurnState.SelectAction;

        InputManager.Instance.RegisterHandler(this, false);
        UIManager.Instance.PostContainer(UIContainerId.ActorActionsView, this);
    }


    // UIContainerControl
    public void HandleComponentInteraction(int containerId, IUIInteractionInfo interactionInfo)
    {
        switch ((UIContainerId)containerId)
        {
            case (UIContainerId.ActorActionsView):
                HandleActionSelectInteraction(interactionInfo);
                break;
        }
    }

    private void HandleActionSelectInteraction(IUIInteractionInfo interactionInfo)
    {
        if ((ActorActions.ComponentId)interactionInfo.ComponentId == ActorActions.ComponentId.ButtonList
            && interactionInfo.InteractionType == UIInteractionType.Click)
        {
            UIButtonList.ButtonListInteractionInfo buttonListInfo = (UIButtonList.ButtonListInteractionInfo)interactionInfo;
            switch (mState)
            {
                // User is selection to Act, Move or Wait
                // No tiles are highlighted currently
                case (TurnState.SelectAction):
                {
                    HandleSelectActionInput(buttonListInfo.ButtonIdx);
                }
                break;
                // User is selection a specific action
                // No tiles are highlighted currently
                case (TurnState.SelectAbility):
                {
                    HandleSelectAbilityInput(buttonListInfo.ButtonIdx);
                }
                break;

                // Cancel movement selection
                case (TurnState.SelectMovementLocation):
                {
                    Logger.Assert(buttonListInfo.ButtonIdx == 0, LogTag.FlowControl, Tag, "Unkonwn SelectMovementLocation button list input");

                    mState = TurnState.SelectAction;
                    ClearTileSelections();
                    UIManager.Instance.Publish(UIContainerId.ActorActionsView);
                }
                break;

                // Cancel Ability selection
                case (TurnState.SelectAbilityTarget):
                {
                    Logger.Assert(buttonListInfo.ButtonIdx == 0, LogTag.FlowControl, Tag, "Unkonwn SelectMovementLocation button list input");

                    mState = TurnState.SelectAbility;

                    ClearTileSelections();
                    UIManager.Instance.Publish(UIContainerId.ActorActionsView);
                }
                break;

                case (TurnState.ConfirmAbilityTarget):
                {
                    HandleAbilityConfirmationInput(buttonListInfo.ButtonIdx == CONFIRM_BUTTON_IDX);
                }
                break;

                case (TurnState.ConfirmMovementTarget):
                {
                    HandleMovementConfirmationInput(buttonListInfo.ButtonIdx == CONFIRM_BUTTON_IDX);
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
                mValidHoverSelections = EncounterModule.Map.GetMovementTilesForActor(mActor);
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
                EncounterEventRouter.Instance.NotifyEvent(new EncounterEvent(EncounterEvent.EventType.TurnFinished));
            }
            break;
        }
    }

    private void HandleSelectAbilityInput(int selectedAbility)
    {
        if (selectedAbility < mActor.Actions.Count)
        {
            ActionInfo actionInfo = mActor.GetActionInfo(mActor.Actions[selectedAbility]);

            mState = TurnState.SelectAbilityTarget;
            mActionIdxSelected = selectedAbility;
            mHoverRangeInfo = actionInfo.EffectRange;

            mValidHoverSelections = EncounterModule.Map.GetTilesInRange(EncounterModule.CharacterDirector.GetActorPosition(mActor), actionInfo.CastRange);
            AddTileSelection(mValidHoverSelections, Tile.HighlightState.TargetSelect);

            AddTileSelection(new List<Tile>(), Tile.HighlightState.AOESelect);

            UpdateHoveredTile(mHoveredTile);

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
            EncounterModule.CharacterDirector.MoveActor(mActor, mSelectedTile.Idx);
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
                mActor.Actions[mActionIdxSelected],
                new TargetSelection(new Target(mSelectedTile.Idx), mHoverRangeInfo)); // GROSs

            OnActionSelected();

            EncounterModule.ActionDirector.DirectAction(actionProposal);
        }
        else
        {
            mState = TurnState.SelectAbilityTarget;

            UIManager.Instance.Publish(UIContainerId.ActorActionsView);
        }
    }

    public IDataProvider Publish()
    {
        List<UIButton.DataProvider> buttonsInState = new List<UIButton.DataProvider>();

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

        ActorActions.DataProvider dp = new ActorActions.DataProvider(
            new UIButtonList.DataProvider(buttonsInState));

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
            hoverSelection = EncounterModule.Map.GetTilesInRange(hoveredTile.Idx, mHoverRangeInfo);
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
        mState = TurnState.NUM;
        mActor = null;
        ClearTileSelections();
        InputManager.Instance.ReleaseHandler(this);
        UIManager.Instance.RemoveOverlay(UIContainerId.ActorActionsView);
    }

    // IEventHandler<EncounterEvent>
    public void HandleEvent(EncounterEvent eventInfo)
    {
        switch (eventInfo.Type)
        {
            case EncounterEvent.EventType.TurnBegun:
                ProgressTurn(eventInfo.Arg<EncounterCharacter>());
                break;
        }
    }
}

