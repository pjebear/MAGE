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
    string Tag = "TurnFlowControl";

    List<TileSelection> mSelectionStack;
    bool mDisplaySelections = false;
    Tile mHoveredTile = null;

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

    void Awake()
    {
        mSelectionStack = new List<TileSelection>();
        EncounterEventRouter.Instance.RegisterHandler(this);
    }

    void OnDestroy()
    {
        EncounterEventRouter.Instance.UnRegisterListener(this);
    }

    public void ProgressTurn(EncounterCharacter actor)
    {
        mActor = actor;
        Transform actorTransform = EncounterModule.ActorDirector.GetController(actor).transform;
        EncounterModule.CameraDirector.FocusTarget(actorTransform);
        mState = TurnState.SelectAction;

        InputManager.Instance.RegisterHandler(this, true);
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
        switch ((ActorActions.ComponentId)interactionInfo.ComponentId)
        {
            case (ActorActions.ComponentId.ButtonList):
                if (interactionInfo.InteractionType == UIInteractionType.Click)
                {
                    UIButtonList.ButtonListInteractionInfo buttonListInfo = (UIButtonList.ButtonListInteractionInfo)interactionInfo;
                    switch (mState)
                    {
                        // User is selection to Act, Move or Wait
                        // No tiles are highlighted currently
                        case (TurnState.SelectAction):
                            switch (buttonListInfo.ButtonIdx)
                            {
                                case (0):
                                    {
                                        mState = TurnState.SelectAbility;

                                        UIManager.Instance.Publish(UIContainerId.ActorActionsView);
                                    }
                                   
                                    break;

                                case (1):
                                    {
                                        mState = TurnState.SelectMovementLocation;

                                        TileSelection selection = Map.Instance.GetMovementTilesForActor(mActor);
                                        selection.SelectionType = Tile.HighlightState.MovementSelect;
                                        AddTileSelection(selection);

                                        TileSelection hoverSelection = TileSelection.EmptySelection;
                                        hoverSelection.SelectionType = Tile.HighlightState.AOESelect;
                                        AddTileSelection(hoverSelection);
                                        SetSelectedTile(mHoveredTile);

                                        ToggleSelectedTiles(true);

                                        UIManager.Instance.Publish(UIContainerId.ActorActionsView);
                                    }
                                   
                                    break;

                                case (2):
                                    mActor.DEBUG_IsTurnComplete = true;

                                    OnActionSelected();
                                    EncounterEventRouter.Instance.NotifyEvent(new EncounterEvent(EncounterEvent.EventType.TurnFinished));
                                    break;
                            }
                            break;
                        // User is selection a specific action
                        // No tiles are highlighted currently
                        case (TurnState.SelectAbility):
                            if (buttonListInfo.ButtonIdx < mActor.Actions.Count)
                            {
                                mState = TurnState.SelectAbilityTarget;
                                mActionIdxSelected = buttonListInfo.ButtonIdx;

                                ActionInfo actionInfo = mActor.GetActionInfo(mActor.Actions[mActionIdxSelected]);
                                TileSelection selection = Map.Instance.GetTilesInRange(EncounterModule.ActorDirector.GetActorPosition(mActor), actionInfo.CastRange.MaxRange);
                                selection.SelectionType = Tile.HighlightState.TargetSelect;
                                AddTileSelection(selection);

                                TileSelection hoverSelection = TileSelection.EmptySelection;
                                hoverSelection.SelectionType = Tile.HighlightState.AOESelect;
                                AddTileSelection(hoverSelection);
                                SetSelectedTile(mHoveredTile);

                                ToggleSelectedTiles(true);

                                UIManager.Instance.Publish(UIContainerId.ActorActionsView);
                            }
                            else
                            {
                                mState = TurnState.SelectAction;

                                UIManager.Instance.Publish(UIContainerId.ActorActionsView);

                            }
                            break;

                        case (TurnState.SelectMovementLocation):
                            switch (buttonListInfo.ButtonIdx)
                            {
                                case (0): // back
                                    {
                                        mState = TurnState.SelectAction;

                                        ClearTileSelections();

                                        UIManager.Instance.Publish(UIContainerId.ActorActionsView);
                                    }
                                    break;
                            }
                            break;
                        case (TurnState.SelectAbilityTarget):
                            switch (buttonListInfo.ButtonIdx)
                            {
                                case (0): // back
                                    {
                                        mState = TurnState.SelectAbility;

                                        ClearTileSelections();
                                        UIManager.Instance.Publish(UIContainerId.ActorActionsView);
                                    }
                                    break;
                            }
                            break;
                        case (TurnState.ConfirmAbilityTarget):
                            switch (buttonListInfo.ButtonIdx)
                            {
                                case (0): // yes
                                    {
                                        mState = TurnState.SelectAction;
                                        mActor.DEBUG_HasActed = true;

                                        ActionProposal actionProposal = new ActionProposal(
                                            mActor,
                                            mActor.Actions[mActionIdxSelected],
                                            new TargetSelection(new Target(mSelectedTile.Idx))); // GROSs

                                        OnActionSelected();

                                        EncounterModule.ActionDirector.DirectAction(actionProposal);
                                    }
                                    break;
                                case (1): // no
                                    {
                                        mState = TurnState.SelectAbilityTarget;

                                        UIManager.Instance.Publish(UIContainerId.ActorActionsView);
                                    }
                                    break;
                            }
                            break;
                        case (TurnState.ConfirmMovementTarget):
                            switch (buttonListInfo.ButtonIdx)
                            {
                                case (0): // yes
                                    {
                                        mActor.DEBUG_HasMoved = true;
                                        EncounterModule.ActorDirector.MoveActor(mActor, mSelectedTile.Idx);
                                        OnActionSelected();
                                    }
                                    break;
                                case (1):
                                    {
                                        mState = TurnState.SelectMovementLocation;

                                        UIManager.Instance.Publish(UIContainerId.ActorActionsView);
                                    }
                                    break;
                            }
                            break;
                    }
                }
            break;

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
        mHoveredTile = tile;

        if (mState == TurnState.SelectAbilityTarget
               || mState == TurnState.SelectMovementLocation)
        {
            SetSelectedTile(mHoveredTile);
        }
    }

    public void OnKeyPressed(InputSource source, int key, InputState state)
    {
        if (mState == TurnState.SelectAbilityTarget && mHoveredTile != null)
        {
            mState = TurnState.ConfirmAbilityTarget;
            mSelectedTile = mHoveredTile;
            UIManager.Instance.Publish(UIContainerId.ActorActionsView);
        }
        else if (mState == TurnState.SelectMovementLocation && mHoveredTile != null)
        {
            mState = TurnState.ConfirmMovementTarget;
            mSelectedTile = mHoveredTile;
            UIManager.Instance.Publish(UIContainerId.ActorActionsView);
        }
    }

    // Helpers
    private void ToggleSelectedTiles(bool visible)
    {
        mDisplaySelections = visible;
        VisualizeTileSelections();
    }

    private void VisualizeTileSelections()
    {
        foreach (TileSelection selection in mSelectionStack)
        {
            selection.ClearSelection();
        }

        if (mDisplaySelections)
        {
            foreach (TileSelection selection in mSelectionStack)
            {
                selection.HighlightSelection();
            }
        }
    }

    private void AddTileSelection(TileSelection selection)
    {
        mSelectionStack.Add(selection);
        VisualizeTileSelections();
    }

    private bool PopTileSelection()
    {
        if (mSelectionStack.Count > 0)
        {
            mSelectionStack[mSelectionStack.Count - 1].ClearSelection();
            mSelectionStack.RemoveAt(mSelectionStack.Count - 1);
            VisualizeTileSelections();
            return true;
        }
        return false;
    }

    private void ClearTileSelections()
    {
        foreach (TileSelection selection in mSelectionStack)
        {
            selection.ClearSelection();
        }
        mSelectionStack.Clear();
    }

    private void SetSelectedTile(Tile selection)
    {
        mSelectionStack[mSelectionStack.Count - 1].Selection.Clear();
        if (selection != null && IsValidHoverTile(selection))
        {
            mSelectionStack[mSelectionStack.Count - 1].Selection.Add(selection);
        }

        VisualizeTileSelections();
    }

    private bool IsValidHoverTile(Tile tile)
    {
        return tile.State != Tile.HighlightState.None;
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

