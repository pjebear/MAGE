using MAGE.GameModes.Combat;
using MAGE.GameModes.Encounter;
using MAGE.GameModes.FlowControl;
using MAGE.GameModes.SceneElements;
using MAGE.GameModes.SceneElements.Navigation;
using MAGE.GameSystems;
using MAGE.GameSystems.Actions;
using MAGE.GameSystems.Characters;
using MAGE.GameSystems.Stats;
using MAGE.GameSystems.World;
using MAGE.UI;
using MAGE.UI.Views;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Events;

namespace MAGE.GameModes.Encounter
{
    class AITurnFlowControl : TurnFlowControlBase, UIContainerControl
    {
        enum AIFlowState
        {
            CharacterDisplay,
            MovementDisplay,
            Move,
            Action,
            NUM
        }
        AIFlowState mFlowState = AIFlowState.CharacterDisplay;

        public override FlowControlId GetFlowControlId()
        {
            return FlowControlId.EncounterAITurnFlowControl;
        }

        protected override void Cleanup()
        {
            base.Cleanup();

            UIManager.Instance.RemoveOverlay(UIContainerId.EncounterCharacterInfoLeftView);
        }

        protected override void Setup()
        {
            base.Setup();

            UIManager.Instance.PostContainer(UIContainerId.EncounterCharacterInfoLeftView, this);

            if (mCurrentTurn.GetComponent<ActionsControl>().GetNumAvailableActions() == 0)
            {
                GameModel.Encounter.TurnComplete = true;
                SendFlowMessage("actionChosen");
                return;
            }

            TeamSide teamSide = mCurrentTurn.GetComponent<CombatEntity>().TeamSide;
            List<CombatTarget> enemies = GameModel.Encounter.AlivePlayers
                .Where(x => x.GetComponent<CombatEntity>().TeamSide != teamSide)
                .Select(x => x.GetComponent<CombatTarget>()).ToList();

            enemies.Sort((x, y) => Vector3.Distance(x.transform.position, mCurrentTurn.transform.position)
                .CompareTo(Vector3.Distance(y.transform.position, mCurrentTurn.transform.position)));

            mCurrentTarget = enemies[0].GetComponent<CombatTarget>();
            mCurrentTarget.GetComponentInChildren<NavMeshObstacle>(true).gameObject.SetActive(false);

            mSelectedAction = ActionComposerFactory.CheckoutAction(mCurrentTurn.GetComponent<CombatEntity>(), ActionId.WeaponAttack);
            mActionInfo = mSelectedAction.ActionInfo;

            Invoke("FinalizeTarget", .5f);
        }

        private void FinalizeTarget()
        {
            if (mCurrentTarget != null 
                && mCurrentTarget != mCurrentTurn
                && mSelectedAction.AreActionRequirementsMet())
            {
                float weaponRange = mWeaponAttackInfo.CastRange.MaxRange;
                if (weaponRange < mCharacterEmptyRadius)
                {
                    weaponRange = mCharacterEmptyRadius;
                }

                if (Vector3.Distance(mCurrentTarget.transform.position, mCurrentTurn.transform.position) > weaponRange)
                {
                    NavMeshPath path = new NavMeshPath();
                    NavMesh.CalculatePath(mCurrentTurn.transform.position, mCurrentTarget.transform.position, NavMesh.AllAreas, path);
                    if (path.corners.Length > 0)
                    {
                        Vector3 updatedPosition = GetPointAlongPathInRangeOf(path.corners, mCurrentTarget.transform.position, weaponRange);
                        NavMesh.CalculatePath(mCurrentTurn.transform.position, updatedPosition, NavMesh.AllAreas, path);

                        Vector3[] corners = path.corners;
                        float pathLength = GetPathLength(path.corners);
                        float availableRange = mCurrentTurn.GetComponent<ActionsControl>().GetAvailableMovementRange();
                        if (pathLength > availableRange)
                        {
                            mHoverInfo.mMoveToPath = TrimPathToSize(corners, availableRange).ToList();
                            mHoverInfo.mIsMoveToInRange = false;
                        }
                        else
                        {
                            mHoverInfo.mIsMoveToInRange = true;
                            mHoverInfo.mMoveToPath = corners.ToList();
                        }
                    }
                }
                else
                {
                    mHoverInfo.mIsMoveToInRange = true;
                }
            }

            ProgressFlow();
        }

        public override bool Notify(string notifyEvent)
        {
            bool handled = false;

            switch (notifyEvent)
            {
                case "progressFlow":
                {
                    handled = true;
                }
                break;
            }

            return handled;
        }

        public void ProgressFlow()
        {
            switch (mFlowState)
            {
                case AIFlowState.CharacterDisplay:
                {
                    if (mHoverInfo.mMoveToPath.Count > 0)
                    {
                        mFlowState = AIFlowState.MovementDisplay;
                    }
                    else
                    {
                        mFlowState = AIFlowState.Action;
                    }
                    
                    Invoke("ProgressFlow", 1f);
                }
                break;
                case AIFlowState.MovementDisplay:
                {
                    UIManager.Instance.RemoveOverlay(UIContainerId.EncounterCharacterInfoLeftView);

                    DisplayMovementRange(mHoverInfo.mMoveToPath[mHoverInfo.mMoveToPath.Count -1]);

                    mFlowState = AIFlowState.Move;
                    Invoke("ProgressFlow", .5f);
                }
                break;
                case AIFlowState.Move:
                {
                    mMovementPathRenderer.gameObject.SetActive(false);
                    mCurrentTurn.GetComponent<ActionsControl>().OnMovementPerformed(Mathf.CeilToInt(GetPathLength(mHoverInfo.mMoveToPath)));
                    mCurrentTurn.GetComponent<ActorMotor>().MoveToPoint(mHoverInfo.mMoveToPath[mHoverInfo.mMoveToPath.Count - 1], () =>
                    {
                        mFlowState = AIFlowState.Action;
                        Invoke("ProgressFlow", 0);
                    });
                }
                break;
                case AIFlowState.Action:
                {
                    if (mHoverInfo.mIsMoveToInRange)
                    {
                        QueueAttack();
                    }
                    else 
                    {
                        GameModel.Encounter.TurnComplete = true;
                    }

                    SendFlowMessage("actionChosen");
                }
                break;
                default:
                    Debug.Assert(false);
                    break;
            }
        }

        public IDataProvider Publish(int containerId)
        {
            switch (containerId)
            {
                case (int)UIContainerId.EncounterCharacterInfoLeftView:
                {
                    return PublishCharacterInfoPanelLeft();
                }
            }

            return null;
        }

        public void HandleComponentInteraction(int containerId, UIInteractionInfo interactionInfo)
        {
             // empty
        }

        public string Name()
        {
            return "AITurnFlowControl";
        }
    }
}