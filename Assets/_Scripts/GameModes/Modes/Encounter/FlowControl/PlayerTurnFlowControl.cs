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
    class PlayerTurnFlowControl 
        : TurnFlowControlBase
        , UIContainerControl

    {
       

        public override FlowControlId GetFlowControlId()
        {
            return FlowControlId.EncounterPlayerTurnFlowControl;
        }

        protected override void Setup()
        {
            base.Setup();

            UIManager.Instance.PostContainer(UIContainerId.ActorActionsView, this);
            UIManager.Instance.PostContainer(UIContainerId.EncounterCharacterInfoLeftView, this);
        }

        protected override void Cleanup()
        {
            base.Cleanup();

            UIManager.Instance.RemoveOverlay(UIContainerId.EncounterCharacterInfoRightView);
            UIManager.Instance.RemoveOverlay(UIContainerId.EncounterCharacterInfoLeftView);
            UIManager.Instance.RemoveOverlay(UIContainerId.ActorActionsView);
        }

        private void Update()
        {
            Input.InputManager input = Input.InputManager.Instance;
            if (input == null) return;

            Input.MouseInfo mouseInfo = input.GetMouseInfo();
            if (!mouseInfo.IsOverWindow) return;

            UpdateHoverInfo(mouseInfo);

            if (mState == State.Idle)
            {
                UpdateIdleState();
            }
            else if (mState == State.TargetSelect)
            {
                Vector3 cursorTerrainPos = Vector3.zero;
                if (CursorToTerrainPos(ref cursorTerrainPos))
                {
                    Vector3 castPoint = cursorTerrainPos;
                    if (mHoverInfo.mHoveredEntity != null)
                    {
                        castPoint = mHoverInfo.mHoveredEntity.transform.position;
                    }
                    DisplayEffectRange(mCurrentCharacter.transform.position, castPoint, mActionInfo.EffectRange);
                }
                else
                {
                    HideEffectRange();
                }
                        
            }

            if (mouseInfo.KeyStates[(int)MouseKey.Left] == InputState.Down)
            {
                OnLeftClick(mouseInfo.ScreenPosCurr);
            }
            else if (mouseInfo.KeyStates[(int)MouseKey.Left] == InputState.Held)
            {
                Vector2 dragDelta = mouseInfo.ScreenPosCurr - mouseInfo.ScreenPosPrev;
                OnLeftClickDrag(dragDelta);
            }

            if (mouseInfo.KeyStates[(int)MouseKey.Right] == InputState.Down)
            {
                OnRightClick(mouseInfo.ScreenPosCurr);
            }
            else if (mouseInfo.KeyStates[(int)MouseKey.Right] == InputState.Held)
            {
                Vector2 dragDelta = mouseInfo.ScreenPosCurr - mouseInfo.ScreenPosPrev;
                OnRightClickDrag(dragDelta);
            }

            if (mouseInfo.ScrollDelta != 0)
            {
                OnMouseScroll(mouseInfo.ScrollDelta);
            }

            //if (mouseInfo.KeyStates[(int)MouseKey.Middle] == InputState.Down)
            //{
            //    OnMiddleClick(mouseInfo.ScreenPosCurr);
            //}
            //else if (mouseInfo.KeyStates[(int)MouseKey.Middle] == InputState.Held)
            //{
            //    Vector2 dragDelta = mouseInfo.ScreenPosCurr - mouseInfo.ScreenPosPrev;
            //    OnMiddleClickDrag(dragDelta);
            //}
                
            
        }

        protected void UpdateIdleState()
        {
            if (mCurrentTarget != null
                   && mCurrentTarget.GetComponent<CombatEntity>().TeamSide != mCurrentCharacter.GetComponent<CombatEntity>().TeamSide)
            {
                float rangeToTarget = Vector3.Distance(mCurrentCharacter.transform.position, mCurrentTarget.transform.position);
                if (rangeToTarget < mWeaponAttackInfo.CastRange.MaxRange)
                {
                    mHoverInfo.mMoveToPath.Clear();
                    mHoverInfo.mIsMoveToInRange = true;
                    mMovementPathRenderer.enabled = false;
                    return;
                }
            }

            float movementRange = mCurrentCharacter.GetComponent<ResourcesControl>().GetAvailableMovementRange();
            if (movementRange > 0)
            {
                UpdateMovementPath();
                if (mMovementPathRenderer.enabled = mHoverInfo.mMoveToPath.Count > 0)
                {
                    DisplayMovementRange(mHoverInfo.mMoveToPath[mHoverInfo.mMoveToPath.Count - 1]);
                    mMovementPathRenderer.enabled = true;
                }
                else
                {
                    mMovementPathRenderer.enabled = false;
                }
            }
            else
            {
                mMovementPathRenderer.enabled = false;
            }
        }

        public override void OnMouseHoverChange(GameObject gameObject)
        {
            CombatCharacter previousTarget = mCurrentTarget;

            CombatCharacter updatedTarget = null;
            // Highlighting a character
            if (gameObject != null)
            {
                if (gameObject.GetComponent<CombatCharacter>() != null)
                {
                    updatedTarget = gameObject.GetComponent<CombatCharacter>();
                }
            }

            if (updatedTarget == mCurrentTarget)
            {
                return;
            }

            mCurrentTarget = updatedTarget;

            if (mCurrentTarget != null)
            {
                if (previousTarget == null)
                {
                    UIManager.Instance.PostContainer(UIContainerId.EncounterCharacterInfoRightView, this);
                }
                else if (previousTarget != mCurrentTarget)
                {
                    UIManager.Instance.Publish(UIContainerId.EncounterCharacterInfoRightView);
                }
            }
            else if (previousTarget != null)
            {
                UIManager.Instance.RemoveOverlay(UIContainerId.EncounterCharacterInfoRightView);
            }

            UpdateCursor();

            Debug.Log(string.Format("HoverTargetChangedTo - {0}", mCurrentTarget != null ? mCurrentTarget.Character.Name : "NONE"));
        }

        void OnLeftClick(Vector2 screenPos)
        {
            // empty
        }

        void OnLeftClickDrag(Vector2 direction)
        {
            // empty
        }

        void OnRightClick(Vector2 screenPos)
        {
            if (mState == State.TargetSelect)
            {
                if (mCurrentTarget == null && mActionInfo.CanGroundTarget)
                {
                    ActionProposal proposal = new ActionProposal(mCurrentCharacter.GetComponent<CombatEntity>(), new Target(mHoverInfo.mHoveredTerrainPos), mSelectedAction);
                    QueueAction(proposal);
                    SendFlowMessage("actionChosen");
                }
                else if (mCurrentTarget != null)
                {
                    if (mActionInfo.EffectRange.TargetingType == TargetingType.Any
                        || mActionInfo.EffectRange.TargetingType == TargetingType.Allies && mCurrentTarget.GetComponent<CombatEntity>().TeamSide == TeamSide.AllyHuman
                        || mActionInfo.EffectRange.TargetingType == TargetingType.Enemies && mCurrentTarget.GetComponent<CombatEntity>().TeamSide == TeamSide.EnemyAI)
                    {
                        ActionProposal proposal = new ActionProposal(mCurrentCharacter.GetComponent<CombatEntity>(), new Target(mCurrentTarget.GetComponent<CombatTarget>()), mSelectedAction);
                        QueueAction(proposal);
                        SendFlowMessage("actionChosen");
                    }           
                }
            }
            else if (mState == State.Idle)
            {
                // Move to character
                if (mCurrentTarget != null)
                {
                    // Move to an ally
                    if (mCurrentTarget.GetComponent<CombatEntity>().TeamSide == mCurrentCharacter.GetComponent<CombatEntity>().TeamSide)
                    {
                        if (mHoverInfo.mMoveToPath.Count > 0)
                        {
                            SetState(State.Moving);

                            mCurrentCharacter.GetComponent<ResourcesControl>().OnMovementPerformed(Mathf.CeilToInt(GetPathLength(mHoverInfo.mMoveToPath)));
                            mCurrentCharacter.GetComponent<ActorMotor>().MoveToPoint(mHoverInfo.mMoveToPath[mHoverInfo.mMoveToPath.Count - 1], () =>
                            {
                                SendFlowMessage("actionChosen");
                            });
                        }
                    }
                    // Move to an attack an enemy
                    else
                    {
                        MoveAttack();
                    }
                }
                else if (mHoverInfo.mMoveToPath.Count > 0)
                {
                    SetState(State.Moving);
                    mCurrentCharacter.GetComponent<ResourcesControl>().OnMovementPerformed(Mathf.RoundToInt(GetPathLength(mHoverInfo.mMoveToPath)));

                    mCurrentCharacter.GetComponent<ActorMotor>().MoveToPoint(mHoverInfo.mMoveToPath[mHoverInfo.mMoveToPath.Count - 1], () =>
                    {
                        SendFlowMessage("actionChosen");
                    });
                }
            }
        }

        void OnRightClickDrag(Vector2 direction)
        {
            // empty
        }

        void OnMouseScroll(float delta)
        {
            // empty
        }

        void UpdateHoverInfo(Input.MouseInfo mouseInfo)
        {
            if (mHoverInfo.mHoveredObstacle != null)
            {
                mHoverInfo.mHoveredObstacle.enabled = true;
                mHoverInfo.mHoveredObstacle = null;
            }

            mHoverInfo.Reset();

            Ray ray = Camera.main.ScreenPointToRay(mouseInfo.ScreenPosCurr);
            RaycastHit[] hits = Physics.RaycastAll(ray, 100);
            if (hits.Length > 0)
            {
                mMovementPathRenderer.enabled = true;

                for (int i = hits.Length - 1; i >= 0; --i)
                {
                    RaycastHit hit = hits[i];

                    if (hit.collider.GetComponent<Terrain>())
                    {
                        mHoverInfo.mHoveredTerrainPos = hit.point;
                        break;
                    }
                    else if (mHoverInfo.mHoveredEntity == null && hit.collider.GetComponent<CombatEntity>())
                    {
                        mHoverInfo.mHoveredEntity = hit.collider.GetComponent<CombatEntity>();
                    }
                    else if (mHoverInfo.mHoveredObstacle == null 
                        && hit.collider.GetComponentInParent<NavMeshObstacle>()
                        && hit.collider.GetComponentInParent<CombatCharacter>() != mCurrentCharacter)
                    {
                        mHoverInfo.mHoveredObstacle = hit.collider.GetComponentInParent<NavMeshObstacle>();
                    }
                }

                if (mHoverInfo.mHoveredEntity != null)
                {
                    Debug.LogFormat("Hovering {0}", mHoverInfo.mHoveredEntity.gameObject.name);

                    if (mHoverInfo.mHoveredEntity != mCurrentCharacter.GetComponent<CombatEntity>())
                    {
                        mHoverInfo.mHoveredObstacle = mHoverInfo.mHoveredEntity.GetComponentInChildren<NavMeshObstacle>();
                        mHoverInfo.mHoveredObstacle.enabled = false;
                    }
                }
            }
        }

        private void UpdateMovementPath()
        {
            Vector3 moveToPoint = Vector3.zero;
            if (mHoverInfo.mHoveredEntity != null)
            {
                moveToPoint = mHoverInfo.mHoveredEntity.transform.position;

                NavMeshPath path = new NavMeshPath();
                NavMesh.CalculatePath(mCurrentCharacter.transform.position, moveToPoint, NavMesh.AllAreas, path);
                if (path.corners.Length > 0)
                {
                    if (mHoverInfo.mHoveredEntity.TeamSide != mCurrentCharacter.GetComponent<CombatEntity>().TeamSide)
                    {
                        float weaponRange = mWeaponAttackInfo.CastRange.MaxRange;
                        if (weaponRange < mCharacterEmptyRadius)
                        {
                            weaponRange = mCharacterEmptyRadius;
                        }
                        moveToPoint = GetPointAlongPathInRangeOf(path.corners, moveToPoint, weaponRange);
                    }
                    else
                    {
                        moveToPoint = GetPointAlongPathInRangeOf(path.corners, moveToPoint, mCharacterEmptyRadius);
                    }
                }
            }
            else if (mHoverInfo.mHoveredObstacle != null && mHoverInfo.mHoveredObstacle.GetComponentInParent<CombatCharacter>() != mCurrentCharacter)
            {
                Vector3 currTargetToMovePoint = mHoverInfo.mHoveredTerrainPos - mHoverInfo.mHoveredObstacle.transform.position;
                moveToPoint = mHoverInfo.mHoveredObstacle.transform.position + (currTargetToMovePoint.normalized * mCharacterEmptyRadius);
            }
            else
            {
                moveToPoint = mHoverInfo.mHoveredTerrainPos;
            }

            if (moveToPoint != Vector3.zero)
            {
                NavMeshPath pathToPoint = new NavMeshPath();
                NavMesh.CalculatePath(mCurrentCharacter.transform.position, moveToPoint, NavMesh.AllAreas, pathToPoint);
                if (pathToPoint.corners.Length > 0)
                {
                    float pathLength = GetPathLength(pathToPoint.corners);
                    float availableMovement = mCurrentCharacter.GetComponent<ResourcesControl>().Resources[ResourceType.MovementRange].Current;
                    if (pathLength > availableMovement)
                    {
                        mHoverInfo.mMoveToPath = TrimPathToSize(pathToPoint.corners, availableMovement).ToList();
                        mHoverInfo.mIsMoveToInRange = false;
                    }
                    else
                    {
                        mHoverInfo.mMoveToPath = pathToPoint.corners.ToList();
                        mHoverInfo.mIsMoveToInRange = true;
                    }
                }
            }
        }

        public IDataProvider Publish(int containerId)
        {
            switch (containerId)
            {
                case (int)UIContainerId.ActorActionsView:
                {
                    ActorActions.DataProvider dataProvider = new ActorActions.DataProvider();
                    List<IDataProvider> actionList = new List<IDataProvider>();

                    if (mState == State.Idle)
                    {
                        if (mCurrentCharacter.GetComponent<ResourcesControl>().GetNumAvailableActions() > 0)
                        {
                            if (GameModel.Encounter.mChargingActions.ContainsKey(mCurrentCharacter))
                            {
                                actionList.Add(new UIButton.DataProvider("Cancel Charging Action", true));
                            }
                            else
                            {
                                foreach (ActionComposerBase action in mAvailableActions)
                                {
                                    string actionName = action.ActionInfo.ActionId.ToString();
                                    actionList.Add(new UIButton.DataProvider(actionName 
                                        + (action.AreResourceRequirementsMet() ? "" : "[Charge]"),
                                        action.AreActionRequirementsMet()));
                                }
                            }
                        }

                        actionList.Add(new UIButton.DataProvider("Wait", true));
                    }
                    else if (mState == State.TargetSelect)
                    {
                        actionList.Add(new UIButton.DataProvider("Cancel", true));
                    }
                    
                    dataProvider.ButtonListDP = new UIList.DataProvider(actionList);

                    return dataProvider;
                }
                case (int)UIContainerId.EncounterCharacterInfoLeftView:
                {
                    return PublishCharacterInfoPanelLeft();
                }
                case (int)UIContainerId.EncounterCharacterInfoRightView:
                {
                    return PublishCharacterInfoPanelRight();
                }
            }

            return null;
        }

        public void HandleComponentInteraction(int containerId, UIInteractionInfo interactionInfo)
        {
            switch (containerId)
            {
                case (int)UIContainerId.ActorActionsView:
                {
                    if (interactionInfo.InteractionType == UIInteractionType.Click)
                    {
                        if (mState == State.Idle)
                        {
                            ListInteractionInfo listInteractionInfo = interactionInfo as ListInteractionInfo;
                            if (GameModel.Encounter.mChargingActions.ContainsKey(mCurrentCharacter))
                            {
                                if (listInteractionInfo.ListIdx == 0)
                                {
                                    GameModel.Encounter.mChargingActions.Remove(mCurrentCharacter);
                                    Publish((int)UIContainerId.ActorActionsView);
                                }
                                else
                                {
                                    GameModel.Encounter.TurnComplete = true;
                                    SendFlowMessage("actionChosen");
                                }
                            }
                            else if (mCurrentCharacter.GetComponent<ResourcesControl>().Resources[ResourceType.Actions].Current == 0 
                                || listInteractionInfo.ListIdx >= mAvailableActions.Count)
                            {
                                // Wait selected
                                GameModel.Encounter.TurnComplete = true;
                                SendFlowMessage("actionChosen");
                            }
                            else 
                            {
                                mSelectedAction = mAvailableActions[listInteractionInfo.ListIdx];
                                mActionInfo = mSelectedAction.ActionInfo;

                                // TODO: Move this when a 'Confirm Target' prompt is added
                                if (mActionInfo.IsSelfCast && mActionInfo.EffectRange.AreaType == AreaType.Point)
                                {
                                    ActionProposal proposal = new ActionProposal(
                                        mCurrentCharacter.GetComponent<CombatEntity>(),
                                        new Target(mCurrentCharacter.GetComponent<CombatTarget>()),
                                        mSelectedAction);
                                    QueueAction(proposal);
                                    SendFlowMessage("actionChosen");
                                }
                                else
                                {
                                    DisplayAbilityRange(mCurrentCharacter.transform.position, mActionInfo.CastRange);

                                    SetState(State.TargetSelect);
                                }
                            }
                        }
                        else if (mState == State.TargetSelect)
                        {
                            // Back
                            SetState(State.Idle);
                        }
                    }
                }
                break;
            }
        }

        protected override void SetState(State state)
        {
            base.SetState(state);

            UpdateCursor();

            UIManager.Instance.Publish(UIContainerId.ActorActionsView);
        }


        public string Name()
        {
            return "PlayerTurnFlowControl";
        }

        void UpdateCursor()
        {
            CursorControl.CursorType updatedCursorType = CursorControl.CursorType.Default;
            // Update Cursor
            if (mState == State.Idle)
            {
                if (mCurrentTarget != null)
                {
                    if (mCurrentTarget.GetComponent<CombatEntity>().TeamSide == mCurrentCharacter.GetComponent<CombatEntity>().TeamSide)
                    {
                        if ((mCurrentTarget.transform.position - mCurrentCharacter.transform.position).magnitude > 1.5f)
                        {
                            updatedCursorType = CursorControl.CursorType.Interact_Near;
                        }
                    }
                    else
                    {
                        if (mCurrentCharacter.GetComponent<ResourcesControl>().GetNumAvailableActions() > 0)
                        {
                            updatedCursorType = CursorControl.CursorType.Combat_Near;
                        }
                        else if ((mCurrentTarget.transform.position - mCurrentCharacter.transform.position).magnitude > 1.5f)
                        {
                            updatedCursorType = CursorControl.CursorType.Interact_Near;
                        }
                    }
                }
                else
                {
                    updatedCursorType = CursorControl.CursorType.Interact_Near;
                }
            }
            else if (mState == State.TargetSelect)
            {
                updatedCursorType = CursorControl.CursorType.Interact_Near;
            }


            UIManager.Instance.SetCursor(updatedCursorType);
        }

        bool CursorToTerrainPos(ref Vector3 out_terrainPos)
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.InputManager.Instance.GetMouseInfo().ScreenPosCurr);
            int layerMask = 1 << (int)RayCastLayer.Terrain;

            RaycastHit hit;
            bool terrainHit = Physics.Raycast(ray, out hit, 100, layerMask);
            out_terrainPos = hit.point;

            return terrainHit;
        }

    }
}