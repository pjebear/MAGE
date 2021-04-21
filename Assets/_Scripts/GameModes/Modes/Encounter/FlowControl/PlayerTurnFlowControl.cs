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
            if (input != null)
            {
                Input.MouseInfo mouseInfo = input.GetMouseInfo();
                if (mouseInfo.IsOverWindow)
                {
                    if (mState == State.Idle)
                    {
                        Ray ray = Camera.main.ScreenPointToRay(mouseInfo.ScreenPosCurr);
                        RaycastHit hit;

                        int layerMask = 1 << (int)RayCastLayer.Terrain;
                        if (Physics.Raycast(ray, out hit, 100, layerMask))
                        {
                            mMovementRangeRenderer.enabled = true;

                            mCurrentMoveToPoint = hit.point;
                            if (mCurrentTarget != null && mCurrentTarget != mCurrentCharacter)
                            {
                                float range = 1.5f;
                                if (mCurrentTarget.GetComponent<CombatEntity>().TeamSide != mCurrentCharacter.GetComponent<CombatEntity>().TeamSide)
                                {
                                    AttackComposer attackComposer = new AttackComposer(mCurrentCharacter.GetComponent<CombatEntity>());

                                    range = attackComposer.ActionInfo.CastRange.MaxRange;
                                }

                                NavMeshPath path = new NavMeshPath();
                                NavMesh.CalculatePath(mCurrentCharacter.transform.position, mCurrentTarget.transform.position, NavMesh.AllAreas, path);
                                if (path.corners.Length > 0)
                                {
                                    mCurrentMoveToPoint = GetPointAlongPathInRangeOf(path.corners, mCurrentTarget.transform.position, range);
                                }
                                else
                                {
                                    mCurrentMoveToPoint = mCurrentTarget.transform.position;
                                }
                            }
                            DisplayMovementRange(mCurrentMoveToPoint);
                        }
                        else
                        {
                            mMovementRangeRenderer.enabled = false;
                        }
                    }
                    else if (mState == State.TargetSelect)
                    {
                        Vector3 cursorTerrainPos = Vector3.zero;
                        if (CursorToTerrainPos(ref cursorTerrainPos))
                        {
                            DisplayEffectRange(mCurrentCharacter.transform.position, cursorTerrainPos, mActionInfo.EffectRange);
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
            }
        }

        public override void OnMouseHoverChange(GameObject gameObject)
        {
            CombatCharacter previousTarget = mCurrentTarget;

            if (mCurrentTarget != null)
            {
                if (mState == State.Idle && mCurrentTarget != mCurrentCharacter)
                {
                    mMovementObstacles[mCurrentTarget].enabled = true;
                }

                mCurrentTarget = null;
            }

            if (gameObject != null)
            {
                mCurrentTarget = gameObject.GetComponent<CombatCharacter>();
                if (mCurrentTarget != null)
                {
                    if (mState == State.Idle && mCurrentTarget != mCurrentCharacter)
                    {
                        mMovementObstacles[mCurrentTarget].enabled = false;
                    }
                }
            }

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
            Ray ray = Camera.main.ScreenPointToRay(screenPos);
            RaycastHit hit;

            if (mState == State.TargetSelect)
            {
                if (Physics.Raycast(ray, out hit, 100))
                {
                    CombatTarget target = hit.collider.gameObject.GetComponent<CombatTarget>();

                    if (target == null && mActionInfo.EffectRange.AreaType != AreaType.Point)
                    {
                        ActionProposal proposal = new ActionProposal(mCurrentCharacter.GetComponent<CombatEntity>(), new Target(hit.point), mSelectedAction.ActionInfo.ActionId);
                        GameModel.Encounter.mActionQueue.Enqueue(proposal);
                        GameModel.Encounter.HasActed = true;
                        SendFlowMessage("actionChosen");
                    }
                    else if (target != null)
                    {
                        if (mActionInfo.EffectRange.TargetingType == TargetingType.Any
                            || mActionInfo.EffectRange.TargetingType == TargetingType.Allies && target.GetComponent<CombatEntity>().TeamSide == TeamSide.AllyHuman
                            || mActionInfo.EffectRange.TargetingType == TargetingType.Enemies && target.GetComponent<CombatEntity>().TeamSide == TeamSide.EnemyAI)
                        {
                            ActionProposal proposal = new ActionProposal(mCurrentCharacter.GetComponent<CombatEntity>(), new Target(target), mSelectedAction.ActionInfo.ActionId);
                            GameModel.Encounter.mActionQueue.Enqueue(proposal);
                            GameModel.Encounter.HasActed = true;
                            SendFlowMessage("actionChosen");
                        }
                            
                    }
                }
            }
            else if (mState == State.Idle)
            {
                // Move to character
                if (mCurrentTarget != null)
                {
                    GameModel.Encounter.HasMoved = true;

                    // Move to an ally
                    if (mCurrentTarget.GetComponent<CombatEntity>().TeamSide == mCurrentCharacter.GetComponent<CombatEntity>().TeamSide)
                    {
                        if ((mCurrentTarget.transform.position - mCurrentCharacter.transform.position).magnitude > 1.5f)
                        {
                            SetState(State.Moving);

                            GameModel.Encounter.HasMoved = true;
                            mCurrentCharacter.GetComponent<ActorMotor>().MoveToPoint(mCurrentMoveToPoint, () =>
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
                else
                {
                    SetState(State.Moving);
                    GameModel.Encounter.HasMoved = true;

                    mCurrentCharacter.GetComponent<ActorMotor>().MoveToPoint(mCurrentMoveToPoint, () =>
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
                        if (!GameModel.Encounter.HasActed)
                        {
                            foreach (ActionComposerBase action in mAvailableActions)
                            {
                                string actionName = action.ActionInfo.ActionId.ToString();
                                bool canCast = action.AreActionRequirementsMet();
                                actionList.Add(new UIButton.DataProvider(actionName, canCast));
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
                            if (GameModel.Encounter.HasActed || listInteractionInfo.ListIdx >= mAvailableActions.Count)
                            {
                                GameModel.Encounter.HasActed = true;
                                GameModel.Encounter.HasMoved = true;
                                SendFlowMessage("actionChosen");
                            }
                            else
                            {
                                mSelectedAction = mAvailableActions[listInteractionInfo.ListIdx];
                                mActionInfo = mSelectedAction.ActionInfo;
                                DisplayAbilityRange(mCurrentCharacter.transform.position, mActionInfo.CastRange);

                                SetState(State.TargetSelect);
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
                        if (!GameModel.Encounter.HasActed)
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