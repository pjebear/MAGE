using MAGE.GameModes.FlowControl;
using MAGE.GameModes.SceneElements;
using MAGE.GameSystems;
using MAGE.GameSystems.Actions;
using MAGE.GameSystems.AI;
using MAGE.GameSystems.Characters;
using MAGE.Messaging;
using MAGE.UI;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace MAGE.GameModes.Encounter
{
    class AITurnControl
        : TurnFlowControlBase
    {
        //    enum TurnAction
        //    {
        //        Move,
        //        Act,

        //        NUM
        //    }

        //    enum State
        //    {
        //        DisplayPossibleMoves,
        //        DisplaySelectedMove,
        //        DisplayPossibleTargets,
        //        DisplaySelectedTarget,

        //        NUM
        //    }

        //    float mVisualStatePauseSeconds = 1;

        //    State mState;

        //    private TurnProposal mCharacterTurnProposal = null;
        //    private MapPathFinder mMovementCalculator = new MapPathFinder();
        //    ActionTileCalculator mActionCalculator;

        //    protected override void OnInit()
        //    {
        //        mTeam = TeamSide.EnemyAI;
        //        mActionCalculator = new ActionTileCalculator(EncounterFlowControl_Deprecated.MapControl.Map);
        //    }

        //    protected override void Cleanup()
        //    {
        //        // empty
        //    }

        //    protected override void ProgressTurn(Character character)
        //    {
        //        if (mCharacterTurnProposal == null || character != mCharacter)
        //        {
        //            mCharacter = character;
        //            mCharacterTurnProposal = TurnProposalCalculator.CalculateTurnProposal(mCharacter, EncounterFlowControl_Deprecated.MapControl.Map);
        //        }

        //        bool actionPerformed = false;

        //        if (!mCharacter.HasActed )
        //        {
        //            if (mCharacter.CanAct)
        //            {
        //                if (mCharacterTurnProposal.ActionId != ActionId.INVALID)
        //                {
        //                    TileIdx characterPosition = EncounterFlowControl_Deprecated.MapControl.Map.GetCharacterPosition(mCharacter).Location;
        //                    if (characterPosition == mCharacterTurnProposal.CastTile)
        //                    {
        //                        FocusCharacter(character);
        //                        ShowCharacterPanel(InfoPanelSide.LEFT, mCharacter);
        //                        DisplayPossibleTargets();
        //                        actionPerformed = true;
        //                    }
        //                }
        //            }
        //        }

        //        if (!actionPerformed)
        //        {
        //            if (!mCharacter.HasMoved)
        //            {
        //                if (mCharacter.CanMove)
        //                {
        //                    if (mCharacterTurnProposal.MovementProposal.Count > 0)
        //                    {
        //                        FocusCharacter(character);
        //                        ShowCharacterPanel(InfoPanelSide.LEFT, mCharacter);
        //                        actionPerformed = true;
        //                        DisplayPossibleMoves();
        //                    }
        //                }
        //            }
        //        }

        //        if (!actionPerformed)
        //        {
        //            mCharacter = null;
        //            Messaging.MessageRouter.Instance.NotifyMessage(new EncounterMessage(EncounterMessage.EventType.TurnFinished));
        //        }
        //    }

        //    protected void SendActionProposal()
        //    {
        //        mCharacter.UpdateOnActed();
        //        ActionProposal_Deprecated actionProposal = new ActionProposal_Deprecated(mCharacter, mCharacterTurnProposal.ActionId, mCharacterTurnProposal.ActionTarget);
        //        EncounterFlowControl_Deprecated.ActionDirector.DirectAction(actionProposal);

        //        HideInfoPanel(InfoPanelSide.LEFT);
        //        HideInfoPanel(InfoPanelSide.RIGHT);
        //    }

        //    protected void SendMovementProposal()
        //    {
        //        //mCharacter.UpdateOnMoved();
        //        //List<TileControl> tilePath = EncounterFlowControl_Deprecated.MapControl.GetTiles(mCharacterTurnProposal.MovementProposal);
        //        //List<Transform> route = new List<Transform>();
        //        //foreach (TileControl tile in tilePath) { route.Add(tile.transform); }

        //        //EncounterFlowControl_Deprecated.MovementDirector.DirectMovement(
        //        //    EncounterFlowControl_Deprecated.CharacterDirector.GetController(mCharacter).transform,
        //        //    route, () =>
        //        //    {
        //        //        Messaging.MessageRouter.Instance.NotifyMessage(new EncounterMessage(EncounterMessage.EventType.MoveResolved));
        //        //                // TODO: figure out orrientation
        //        //                CharacterPosition newPos = new CharacterPosition();
        //        //        newPos.Location = mCharacterTurnProposal.MovementProposal.Last();
        //        //        newPos.Orientation = EncounterFlowControl_Deprecated.CharacterDirector.GetController(mCharacter).GetOrientation();
        //        //        EncounterFlowControl_Deprecated.CharacterDirector.UpdateCharacterPosition(mCharacter, newPos);
        //        //    });

        //        HideInfoPanel(InfoPanelSide.LEFT);
        //        HideInfoPanel(InfoPanelSide.RIGHT);
        //    }

        //    protected IEnumerator _WaitFor(float seconds)
        //    {
        //        while (seconds > 0)
        //        {
        //            seconds -= Time.deltaTime;
        //            yield return new WaitForFixedUpdate();
        //        }
        //        WaitComplete();
        //    }

        //    protected void DisplayPossibleMoves()
        //    {
        //        //mState = State.DisplayPossibleMoves;
        //        //ToggleSelectedTiles(true);

        //        //mMovementCalculator.CalculatePaths(EncounterFlowControl_Deprecated.MapControl.Map
        //        //     , EncounterFlowControl_Deprecated.MapControl.Map.GetCharacterPosition(mCharacter).Location
        //        //     , mCharacter.TeamSide
        //        //     , mCharacter.CurrentAttributes[TertiaryStat.Movement]
        //        //     , mCharacter.CurrentAttributes[TertiaryStat.Jump]);

        //        //AddTileSelection(EncounterFlowControl_Deprecated.MapControl.GetTiles(mMovementCalculator.GetPossibleTiles()), TileControl.HighlightState.MovementSelect);

        //        //StartCoroutine(_WaitFor(mVisualStatePauseSeconds));
        //    }

        //    protected void DisplayMovementSelection()
        //    {
        //        mState = State.DisplaySelectedMove;
        //        List<TileIdx> selection = new List<TileIdx>() { mCharacterTurnProposal.MovementProposal.Last() };
        //        AddTileSelection(EncounterFlowControl_Deprecated.MapControl.GetTiles(selection), TileControl.HighlightState.AOESelect);

        //        StartCoroutine(_WaitFor(mVisualStatePauseSeconds));
        //    }

        //    protected void DisplayPossibleTargets()
        //    {
        //        mState = State.DisplayPossibleTargets;
        //        ToggleSelectedTiles(true);

        //        ActionInfoBase actionInfo = mCharacter.GetActionInfo(mCharacterTurnProposal.ActionId);
        //        List<TileIdx> targetTiles = mActionCalculator.CalculateTilesInRange(
        //            EncounterFlowControl_Deprecated.MapControl.Map.GetCharacterPosition(mCharacter).Location,
        //            EncounterFlowControl_Deprecated.MapControl.Map.GetCharacterPosition(mCharacter).Location,
        //            actionInfo.CastRange,
        //            mCharacter.TeamSide);
        //        AddTileSelection(EncounterFlowControl_Deprecated.MapControl.GetTiles(targetTiles), TileControl.HighlightState.TargetSelect);

        //        StartCoroutine(_WaitFor(mVisualStatePauseSeconds));
        //    }

        //    protected void DisplayTargetSelection()
        //    {
        //        mState = State.DisplaySelectedTarget;
        //        List<TileIdx> targetTiles = EncounterFlowControl_Deprecated.MapControl.Map.GetTargetedTiles(
        //            EncounterFlowControl_Deprecated.MapControl.Map.GetCharacterPosition(mCharacter).Location, 
        //            mCharacterTurnProposal.ActionTarget);
        //        AddTileSelection(EncounterFlowControl_Deprecated.MapControl.GetTiles(targetTiles), TileControl.HighlightState.AOESelect);

        //        StartCoroutine(_WaitFor(mVisualStatePauseSeconds));

        //        Character focalTarget = null;
        //        if (mCharacterTurnProposal.ActionTarget.FocalTarget.TargetType == TargetSelectionType.Character)
        //        {
        //            focalTarget = mCharacterTurnProposal.ActionTarget.FocalTarget.CharacterTarget;
        //        }
        //        else
        //        {
        //            focalTarget = EncounterFlowControl_Deprecated.MapControl.Map.TileAt(mCharacterTurnProposal.ActionTarget.FocalTarget.TileTarget).OnTile;
        //        }
        //        if (focalTarget != null)
        //        {
        //            ShowCharacterPanel(InfoPanelSide.RIGHT, focalTarget);
        //        }
        //    }

        //    protected void WaitComplete()
        //    {
        //        switch (mState)
        //        {
        //            case State.DisplayPossibleMoves:
        //            {
        //                DisplayMovementSelection();
        //            }
        //            break;
        //            case State.DisplaySelectedMove:
        //            {
        //                ClearTileSelections();
        //                SendMovementProposal();
        //            }
        //            break;
        //            case State.DisplayPossibleTargets:
        //            {
        //                DisplayTargetSelection();
        //            }
        //            break;
        //            case State.DisplaySelectedTarget:
        //            {
        //                ClearTileSelections();
        //                SendActionProposal();
        //            }
        //            break;
        //        }
        //    }
        public override string Condition(string queryEvent)
        {
            return base.Condition(queryEvent);
        }

        public override FlowControlId GetFlowControlId()
        {
            throw new NotImplementedException();
        }

        public override void HandleMessage(MessageInfoBase eventInfoBase)
        {
            base.HandleMessage(eventInfoBase);
        }

        public override bool Notify(string notifyEvent)
        {
            return base.Notify(notifyEvent);
        }

        public override void OnKeyPressed(InputSource source, int key, InputState state)
        {
            base.OnKeyPressed(source, key, state);
        }

        public override void OnMouseHoverChange(GameObject mouseHover)
        {
            base.OnMouseHoverChange(mouseHover);
        }

        public override void OnMouseScrolled(float scrollDelta)
        {
            base.OnMouseScrolled(scrollDelta);
        }

        public override string Query(string queryEvent)
        {
            return base.Query(queryEvent);
        }

        protected override void Cleanup()
        {
            base.Cleanup();
        }

        protected override void SetState(State state)
        {
            base.SetState(state);
        }

        protected override void Setup()
        {
            base.Setup();
        }
    }
}
