using MAGE.GameModes.FlowControl;
using MAGE.GameModes.SceneElements;
using MAGE.GameSystems;
using MAGE.GameSystems.Actions;
using MAGE.GameSystems.AI;
using MAGE.GameSystems.Characters;
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
        enum TurnAction
        {
            Move,
            Act,

            NUM
        }

        enum State
        {
            DisplayPossibleMoves,
            DisplaySelectedMove,
            DisplayPossibleTargets,
            DisplaySelectedTarget,

            NUM
        }

        float mVisualStatePauseSeconds = 1;

        State mState;

        private TurnProposal mCharacterTurnProposal = null;
        private HashSet<TurnAction> mTurnActions = new HashSet<TurnAction>();
        private MapPathFinder mMovementCalculator = new MapPathFinder();
        ActionTileCalculator mActionCalculator;

        protected override void OnInit()
        {
            mTeam = TeamSide.EnemyAI;
            mActionCalculator = new ActionTileCalculator(EncounterFlowControl.MapControl.Map);
        }

        protected override void Cleanup()
        {
            // empty
        }

        protected override void ProgressTurn(Character character)
        {
            if (mCharacterTurnProposal == null || mTurnActions.Count == 0
                || character != mCharacter)
            {
                mTurnActions.Clear();
                mCharacter = character;
                mCharacterTurnProposal = TurnProposalCalculator.CalculateTurnProposal(mCharacter, EncounterFlowControl.MapControl.Map);
                
                if (mCharacterTurnProposal.ActionId != ActionId.INVALID)
                {
                    mTurnActions.Add(TurnAction.Act);
                }
                if (mCharacterTurnProposal.MovementProposal.Count > 0)
                {
                    mTurnActions.Add(TurnAction.Move);
                }
            }

            bool actionPerformed = false;

            TileIdx characterPosition = EncounterFlowControl.MapControl.Map.GetCharacterPosition(mCharacter).Location;
            if (!mCharacter.HasActed )
            {
                if (mCharacter.CanAct)
                {
                    if (mTurnActions.Contains(TurnAction.Act))
                    {
                        if (characterPosition == mCharacterTurnProposal.CastTile)
                        {
                            FocusCharacter(character);
                            ShowCharacterPanel(InfoPanelSide.LEFT, mCharacter);
                            DisplayPossibleTargets();
                            actionPerformed = true;
                            mTurnActions.Remove(TurnAction.Act);
                        }
                    }
                }
                else
                {
                    mTurnActions.Remove(TurnAction.Act);
                }
            }
            
            if (!actionPerformed)
            {
                if (!mCharacter.HasMoved)
                {
                    if (mCharacter.CanMove)
                    {
                        if (mTurnActions.Contains(TurnAction.Move))
                        {
                            FocusCharacter(character);
                            ShowCharacterPanel(InfoPanelSide.LEFT, mCharacter);
                            actionPerformed = true;
                            mTurnActions.Remove(TurnAction.Move);
                            DisplayPossibleMoves();
                        }
                    }
                    else
                    {
                        mTurnActions.Remove(TurnAction.Move);
                    }
                }
            }

            if (!actionPerformed)
            {
                mCharacter = null;
                Messaging.MessageRouter.Instance.NotifyMessage(new EncounterMessage(EncounterMessage.EventType.TurnFinished));
            }
        }

        protected void SendActionProposal()
        {
            mCharacter.UpdateOnActed();
            ActionProposal actionProposal = new ActionProposal(mCharacter, mCharacterTurnProposal.ActionId, mCharacterTurnProposal.ActionTarget);
            EncounterFlowControl.ActionDirector.DirectAction(actionProposal);

            HideInfoPanel(InfoPanelSide.LEFT);
            HideInfoPanel(InfoPanelSide.RIGHT);
        }

        protected void SendMovementProposal()
        {
            mCharacter.UpdateOnMoved();
            List<TileControl> tilePath = EncounterFlowControl.MapControl.GetTiles(mCharacterTurnProposal.MovementProposal);
            List<Transform> route = new List<Transform>();
            foreach (TileControl tile in tilePath) { route.Add(tile.transform); }

            EncounterFlowControl.MovementDirector.DirectMovement(
                EncounterFlowControl.CharacterDirector.GetController(mCharacter).ActorController,
                route, () =>
                {
                    Messaging.MessageRouter.Instance.NotifyMessage(new EncounterMessage(EncounterMessage.EventType.MoveResolved));
                            // TODO: figure out orrientation
                            CharacterPosition newPos = new CharacterPosition();
                    newPos.Location = mCharacterTurnProposal.MovementProposal.Last();
                    newPos.Orientation = EncounterFlowControl.CharacterDirector.GetController(mCharacter).GetOrientation();
                    EncounterFlowControl.CharacterDirector.UpdateCharacterPosition(mCharacter, newPos);
                });

            HideInfoPanel(InfoPanelSide.LEFT);
            HideInfoPanel(InfoPanelSide.RIGHT);
        }

        protected IEnumerator _WaitFor(float seconds)
        {
            while (seconds > 0)
            {
                seconds -= Time.deltaTime;
                yield return new WaitForFixedUpdate();
            }
            WaitComplete();
        }

        protected void DisplayPossibleMoves()
        {
            mState = State.DisplayPossibleMoves;
            ToggleSelectedTiles(true);

            mMovementCalculator.CalculatePaths(EncounterFlowControl.MapControl.Map
                 , EncounterFlowControl.MapControl.Map.GetCharacterPosition(mCharacter).Location
                 , mCharacter.TeamSide
                 , mCharacter.CurrentAttributes[TertiaryStat.Movement]
                 , mCharacter.CurrentAttributes[TertiaryStat.Jump]);

            AddTileSelection(EncounterFlowControl.MapControl.GetTiles(mMovementCalculator.GetPossibleTiles()), TileControl.HighlightState.MovementSelect);

            StartCoroutine(_WaitFor(mVisualStatePauseSeconds));
        }

        protected void DisplayMovementSelection()
        {
            mState = State.DisplaySelectedMove;
            List<TileIdx> selection = new List<TileIdx>() { mCharacterTurnProposal.MovementProposal.Last() };
            AddTileSelection(EncounterFlowControl.MapControl.GetTiles(selection), TileControl.HighlightState.AOESelect);

            StartCoroutine(_WaitFor(mVisualStatePauseSeconds));
        }

        protected void DisplayPossibleTargets()
        {
            mState = State.DisplayPossibleTargets;
            ToggleSelectedTiles(true);

            ActionInfo actionInfo = mCharacter.GetActionInfo(mCharacterTurnProposal.ActionId);
            List<TileIdx> targetTiles = mActionCalculator.CalculateTilesInRange(
                EncounterFlowControl.MapControl.Map.GetCharacterPosition(mCharacter).Location,
                EncounterFlowControl.MapControl.Map.GetCharacterPosition(mCharacter).Location,
                actionInfo.CastRange);
            AddTileSelection(EncounterFlowControl.MapControl.GetTiles(targetTiles), TileControl.HighlightState.TargetSelect);

            StartCoroutine(_WaitFor(mVisualStatePauseSeconds));
        }

        protected void DisplayTargetSelection()
        {
            mState = State.DisplaySelectedTarget;
            List<TileIdx> targetTiles = EncounterFlowControl.MapControl.Map.GetTargetedTiles(
                EncounterFlowControl.MapControl.Map.GetCharacterPosition(mCharacter).Location, 
                mCharacterTurnProposal.ActionTarget);
            AddTileSelection(EncounterFlowControl.MapControl.GetTiles(targetTiles), TileControl.HighlightState.AOESelect);

            StartCoroutine(_WaitFor(mVisualStatePauseSeconds));

            Character focalTarget = null;
            if (mCharacterTurnProposal.ActionTarget.FocalTarget.TargetType == TargetSelectionType.Character)
            {
                focalTarget = mCharacterTurnProposal.ActionTarget.FocalTarget.CharacterTarget;
            }
            else
            {
                focalTarget = EncounterFlowControl.MapControl.Map.TileAt(mCharacterTurnProposal.ActionTarget.FocalTarget.TileTarget).OnTile;
            }
            if (focalTarget != null)
            {
                ShowCharacterPanel(InfoPanelSide.RIGHT, focalTarget);
            }
        }

        protected void WaitComplete()
        {
            switch (mState)
            {
                case State.DisplayPossibleMoves:
                {
                    DisplayMovementSelection();
                }
                break;
                case State.DisplaySelectedMove:
                {
                    ClearTileSelections();
                    SendMovementProposal();
                }
                break;
                case State.DisplayPossibleTargets:
                {
                    DisplayTargetSelection();
                }
                break;
                case State.DisplaySelectedTarget:
                {
                    ClearTileSelections();
                    SendActionProposal();
                }
                break;
            }
        }
    }
}
