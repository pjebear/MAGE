using MAGE.GameModes.Combat;
using MAGE.GameModes.Encounter;
using MAGE.GameModes.FlowControl;
using MAGE.GameModes.SceneElements;
using MAGE.GameModes.SceneElements.Navigation;
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
            Action,
            NUM
        }
        AIFlowState mFlowState = AIFlowState.CharacterDisplay;

        public override FlowControlId GetFlowControlId()
        {
            return FlowControlId.EncounterAITurnFlowControl;
        }

        protected override void Setup()
        {
            base.Setup();

            if (GameModel.Encounter.HasActed)
            {
                GameModel.Encounter.HasMoved = true;
                SendFlowMessage("actionChosen");
                return;
            }

            List<CombatTarget> enemies = GameModel.Encounter.Players
                .Select(x => x.GetComponent<CombatEntity>())
                .Where(x => x.TeamSide != mCurrentCharacter.GetComponent<CombatEntity>().TeamSide)
                .Select(x => x.GetComponent<ResourcesControl>())
                .Where(x => x.IsAlive())
                .Select(x => x.GetComponent<CombatTarget>()).ToList();
            enemies.Sort((x, y) => Vector3.Distance(x.transform.position, mCurrentCharacter.transform.position)
                .CompareTo(Vector3.Distance(y.transform.position, mCurrentCharacter.transform.position)));

            mCurrentTarget = enemies[0].GetComponent<CombatCharacter >();

            mMovementObstacles[mCurrentTarget].gameObject.SetActive(false);

            mCurrentMoveToPoint = mCurrentTarget.transform.position;

            mSelectedAction = ActionId.MeeleWeaponAttack;
            mActionInfo = ActionComposer.GetAction(mCurrentCharacter.GetComponent<CombatEntity>(), mSelectedAction).ActionInfo;

            if (mCurrentTarget != null && mCurrentTarget != mCurrentCharacter)
            {
                NavMeshPath path = new NavMeshPath();
                NavMesh.CalculatePath(mCurrentCharacter.transform.position, mCurrentTarget.transform.position, NavMesh.AllAreas, path);
                if (path.corners.Length > 0)
                {
                    mCurrentMoveToPoint = GetPointAlongPathInRangeOf(path.corners, mCurrentTarget.transform.position, mActionInfo.CastRange.MaxRange);
                }
                else
                {
                    mCurrentMoveToPoint = mCurrentTarget.transform.position;
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
                    UIManager.Instance.PostContainer(UIContainerId.EncounterCharacterInfoLeftView, this);

                    mFlowState = AIFlowState.MovementDisplay;
                    
                    Invoke("ProgressFlow", 1f);
                }
                break;
                case AIFlowState.MovementDisplay:
                {
                    UIManager.Instance.RemoveOverlay(UIContainerId.EncounterCharacterInfoLeftView);

                    DisplayMovementRange(mCurrentMoveToPoint);

                    mFlowState = AIFlowState.Action;
                    Invoke("ProgressFlow", .5f);
                }
                break;
                case AIFlowState.Action:
                {
                    mMovementRangeRenderer.gameObject.SetActive(false);
                    MoveAttack();
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
                break;
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