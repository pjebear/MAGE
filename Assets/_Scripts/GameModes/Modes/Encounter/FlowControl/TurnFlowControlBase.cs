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
    abstract class TurnFlowControlBase 
        : FlowControl.FlowControlBase
        
    {
        protected class HoverInfo
        {
            public List<Vector3> mMoveToPath = new List<Vector3>();
            public bool mIsMoveToInRange = false;
            public Vector3 mHoveredTerrainPos = Vector3.zero;
            public CombatEntity mHoveredEntity = null;
            public NavMeshObstacle mHoveredObstacle = null;

            public void Reset()
            {
                mMoveToPath.Clear();
                mIsMoveToInRange = false;
                mHoveredTerrainPos = Vector3.zero;
                mHoveredEntity = null;
                mHoveredObstacle = null;
            }
        }
        protected HoverInfo mHoverInfo = new HoverInfo();

        protected enum State
        {
            Idle,
            Moving,
            TargetSelect
        }
        protected State mState = State.Idle;
        protected ActionComposerBase mSelectedAction = null;
        protected ActionInfo mActionInfo = null;
        protected ActionInfo mWeaponAttackInfo = null;
        // Private 
        protected CombatCharacter mCurrentCharacter;
        protected List<ActionComposerBase> mAvailableActions;
        protected CombatCharacter mCurrentTarget;

        protected LineRenderer mAbilityRangeRenderer = null;
        protected LineRenderer mAbilityEffectRenderer = null;
        protected LineRenderer mMovementPathRenderer = null;
        protected LineRenderer mMovementRangeRenderer = null;
        protected LineRenderer mTargetBoundaryRenderer = null;

        protected int mCircleSegments = 32;
        protected float mCharacterEmptyRadius = 1f;

        protected HashSet<NavMeshObstacle> mMovementObstacles = new HashSet<NavMeshObstacle>();

        protected override void Setup()
        {
            mAbilityRangeRenderer = EncounterPrefabLoader.RangeRenderer;
            mAbilityRangeRenderer.transform.SetParent(transform);
            mAbilityRangeRenderer.gameObject.SetActive(false);
            mAbilityRangeRenderer.positionCount = mCircleSegments + 1;
            mAbilityRangeRenderer.useWorldSpace = true;

            mAbilityEffectRenderer = EncounterPrefabLoader.RangeRenderer;
            mAbilityEffectRenderer.transform.SetParent(transform);
            mAbilityEffectRenderer.gameObject.SetActive(false);
            mAbilityEffectRenderer.positionCount = mCircleSegments + 1;
            mAbilityEffectRenderer.useWorldSpace = true;

            mTargetBoundaryRenderer = EncounterPrefabLoader.RangeRenderer;
            mTargetBoundaryRenderer.transform.SetParent(transform);
            mTargetBoundaryRenderer.gameObject.SetActive(false);
            mTargetBoundaryRenderer.positionCount = mCircleSegments + 1;
            mTargetBoundaryRenderer.useWorldSpace = true;

            mMovementRangeRenderer = EncounterPrefabLoader.RangeRenderer;
            mMovementRangeRenderer.transform.SetParent(transform);
            mMovementRangeRenderer.gameObject.SetActive(false);
            mMovementRangeRenderer.positionCount = mCircleSegments + 1;
            mMovementRangeRenderer.useWorldSpace = true;

            mMovementPathRenderer = EncounterPrefabLoader.RangeRenderer;
            mMovementPathRenderer.transform.SetParent(transform);
            mMovementPathRenderer.gameObject.SetActive(false);
            mMovementPathRenderer.useWorldSpace = true;

            foreach (CombatCharacter combatCharacter in GameModel.Encounter.AlivePlayers)
            {
                NavMeshObstacle obstacle = EncounterPrefabLoader.Obstacle;
                obstacle.transform.SetParent(combatCharacter.transform);
                obstacle.transform.localPosition = Vector3.zero;
                mCharacterEmptyRadius = obstacle.radius * 2;
                mMovementObstacles.Add(obstacle);
            }

            SetCurrentCharacter(GameModel.Encounter.CurrentTurn);

            SetState(State.Idle);
        }

        protected override void Cleanup()
        {
            foreach (var obstacle in mMovementObstacles)
            {
                Destroy(obstacle.gameObject);
            }
            mMovementObstacles.Clear();
        }


        private void SetCurrentCharacter(CombatCharacter combatCharacter)
        {
            mCurrentCharacter = combatCharacter;
            mAvailableActions = new List<ActionComposerBase>();
            foreach (ActionId actionId in combatCharacter.GetComponent<ActionsControl>().Actions)
            {
                mAvailableActions.Add(ActionComposerFactory.CheckoutAction(mCurrentCharacter.GetComponent<CombatEntity>(), actionId));
            }
            combatCharacter.GetComponentInChildren<NavMeshObstacle>().enabled = false;

            mWeaponAttackInfo = ActionComposerFactory.CheckoutAction(mCurrentCharacter.GetComponent<CombatEntity>(), ActionId.WeaponAttack).ActionInfo;

            Camera.main.GetComponent<Cameras.CameraController>().SetTarget(mCurrentCharacter.transform, Cameras.CameraType.TopDown);
        }

        protected void MoveAttack()
        {
            bool attackQueued = false;
            if (mCurrentCharacter.GetComponent<ResourcesControl>().GetNumAvailableActions() > 0)
            {
                attackQueued = true;
                QueueAttack();
            }
            
            if (mHoverInfo.mMoveToPath.Count > 0)
            {
                SetState(State.Moving);

                float pathLength = mHoverInfo.mMoveToPath.Count;
                mCurrentCharacter.GetComponent<ResourcesControl>().OnMovementPerformed(Mathf.CeilToInt(pathLength));
                mCurrentCharacter.GetComponent<ActorMotor>().MoveToPoint(mHoverInfo.mMoveToPath[mHoverInfo.mMoveToPath.Count - 1], () =>
                {
                    SendFlowMessage("actionChosen");
                });
            }
            else if (attackQueued)
            {
                SendFlowMessage("actionChosen");
            }
        }

        protected void QueueAttack()
        {
            mCurrentCharacter.GetComponent<ResourcesControl>().OnActionPerformed(StateChange.Empty);

            ActionProposal proposal = new ActionProposal(
            mCurrentCharacter.GetComponent<CombatEntity>(),
            new Target(mCurrentTarget.GetComponent<CombatTarget>()),
            ActionId.WeaponAttack);

            GameModel.Encounter.mActionQueue.Enqueue(proposal);
        }

        protected virtual void SetState(State state)
        {
            mState = state;

            if (state == State.Idle)
            {
                mMovementPathRenderer.gameObject.SetActive(mCurrentCharacter.GetComponent<ResourcesControl>().GetAvailableMovementRange() > 0);
                mAbilityRangeRenderer.gameObject.SetActive(false);
                mAbilityEffectRenderer.gameObject.SetActive(false);
            }
            else if (state == State.TargetSelect)
            {
                mMovementPathRenderer.gameObject.SetActive(false);
                mAbilityRangeRenderer.gameObject.SetActive(true);
                mAbilityEffectRenderer.gameObject.SetActive(true);
            }
            else if (state == State.Moving)
            {
                mMovementPathRenderer.gameObject.SetActive(false);
                mAbilityRangeRenderer.gameObject.SetActive(false);
                mAbilityEffectRenderer.gameObject.SetActive(false);
            }
        }

        protected IDataProvider PublishCharacterInfoPanelRight()
        {
            if (mCurrentTarget != null)
            {
                return PublishCharacterInfoPanel(mCurrentTarget);
            }
            else
            {
                Debug.LogWarning("CurrentTarget is null");
                return new CharacterInspector.DataProvider();
            }
        }

        protected IDataProvider PublishCharacterInfoPanelLeft()
        {
            return PublishCharacterInfoPanel(mCurrentCharacter);
        }

        protected IDataProvider PublishCharacterInfoPanel(CombatCharacter combatCharacter)
        {
            CharacterInspector.DataProvider dp = new CharacterInspector.DataProvider();
            
            Character toPublish = combatCharacter.Character;

            dp.IsAlly = combatCharacter.GetComponent<CombatEntity>().TeamSide == TeamSide.AllyHuman;
            dp.PortraitAsset = toPublish.GetAppearance().PortraitSpriteId.ToString();
            dp.Name = toPublish.Name;
            dp.Level = toPublish.Level;
            dp.Exp = toPublish.Experience;
            dp.Specialization = toPublish.CurrentSpecializationType.ToString();

            dp.CurrentHP    = combatCharacter.GetComponent<ResourcesControl>().Resources[ResourceType.Health].Current;
            dp.MaxHP        = combatCharacter.GetComponent<ResourcesControl>().Resources[ResourceType.Health].Max;
            dp.CurrentMP    = combatCharacter.GetComponent<ResourcesControl>().Resources[ResourceType.Mana].Current;
            dp.MaxMP        = combatCharacter.GetComponent<ResourcesControl>().Resources[ResourceType.Mana].Max;
            dp.CurrentClock = combatCharacter.GetComponent<ResourcesControl>().Resources[ResourceType.Clock].Current;

            dp.Might        = (int)combatCharacter.GetComponent<StatsControl>().Attributes[PrimaryStat.Might];
            dp.Finesse      = (int)combatCharacter.GetComponent<StatsControl>().Attributes[PrimaryStat.Finese];
            dp.Magic        = (int)combatCharacter.GetComponent<StatsControl>().Attributes[PrimaryStat.Magic];

            dp.Fortitude    = (int)combatCharacter.GetComponent<StatsControl>().Attributes[SecondaryStat.Fortitude];
            dp.Attunement   = (int)combatCharacter.GetComponent<StatsControl>().Attributes[SecondaryStat.Attunement];

            float block, dodge, parry;
            InteractionUtil.GetAvoidanceAttributesForCharacter(combatCharacter, out dodge, out block, out parry);

            dp.Block        = (int)block;
            dp.Dodge        = (int)dodge;
            dp.Parry        = (int)parry;

            List<IDataProvider> statusEffects = new List<IDataProvider>();
            foreach (StatusEffect effect in combatCharacter.GetComponent<StatusEffectControl>().StatusEffects)
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

        protected void DisplayAbilityRange(Vector3 center, RangeInfo rangeInfo)
        {
            mAbilityRangeRenderer.gameObject.SetActive(true);
            switch (rangeInfo.AreaType)
            {
                case AreaType.Circle:
                {
                    DisplayCircleRange(mAbilityRangeRenderer, center, rangeInfo.MaxRange);
                }
                break;
                case AreaType.Point:
                {
                    mAbilityEffectRenderer.gameObject.SetActive(false);
                }
                break;
                default:
                    Debug.Assert(false);
                    break;
            }
        }

        protected void HideAbilityRange()
        {
            mAbilityRangeRenderer.gameObject.SetActive(false);
        }
        

        protected void DisplayEffectRange(Vector3 center, Vector3 castPoint, RangeInfo rangeInfo)
        {
            mAbilityEffectRenderer.gameObject.SetActive(true);
           
            switch (rangeInfo.AreaType)
            {
                case AreaType.Circle:
                case AreaType.Chain:
                {
                    DisplayCircleRange(mAbilityEffectRenderer, castPoint, rangeInfo.MaxRange);
                }
                break;
                case AreaType.Cone:
                {
                    DisplayConeRange(mAbilityEffectRenderer, center, castPoint, rangeInfo.MaxRange);
                }
                break;
                case AreaType.Point:
                {
                    mAbilityEffectRenderer.gameObject.SetActive(false);
                }
                break;
                default:
                    Debug.Assert(false);
                    break;
            }
        }

        protected void HideEffectRange()
        {
            mAbilityRangeRenderer.gameObject.SetActive(false);
        }

        protected void DisplayConeRange(LineRenderer rangeRender, Vector3 center, Vector3 castPoint, float radius)
        {
            int numCurvaturePoints = 5;
            int numPoints =
                2 // 2 points for center point
                + numCurvaturePoints; // points for radius
            
            Vector3[] points = new Vector3[numPoints];

            points[0] = center;
            points[numPoints - 1] = center;

            Vector3 direction = (castPoint - center).normalized * radius;

            float startAngle = -22.5f;
            float totalAngle = 45f;
            float angleSegmentSize = totalAngle / (float)numCurvaturePoints;

            for (int i = 0; i < numCurvaturePoints + 1; ++i)
            {
                float nextAngle = startAngle + angleSegmentSize * i;

                points[1 + i] = center + Quaternion.Euler(0, nextAngle, 0) * direction;
            }

            rangeRender.positionCount = numPoints;
            rangeRender.SetPositions(points);
        }

        protected void DisplayCircleRange(LineRenderer rangeRender, Vector3 center, float radius)
        {
            rangeRender.positionCount = mCircleSegments + 1;
            Vector3[] points = new Vector3[mCircleSegments + 1];
            float currentAngle = 0f;

            for (int i = 0; i < mCircleSegments + 1; ++i)
            {
                float xOffset = Mathf.Sin(Mathf.Deg2Rad * currentAngle) * radius + center.x;
                float zOffset = Mathf.Cos(Mathf.Deg2Rad * currentAngle) * radius + center.z;

                Vector3 circlePosition = new Vector3(xOffset, center.y, zOffset);
                Ray ray = new Ray(circlePosition + Vector3.up * 100, Vector3.down);
                RaycastHit hit;
                int layerMask = 1 << (int)RayCastLayer.Terrain;
                if (Physics.Raycast(ray, out hit, 500, layerMask))
                {
                    circlePosition.y = hit.point.y;
                }

                points[i] = circlePosition;

                currentAngle += 360f / mCircleSegments;
            }

            rangeRender.SetPositions(points);
        }

        protected void DisplayMovementRange(Vector3 destination)
        {
            mMovementPathRenderer.gameObject.SetActive(true);

            NavMeshPath path = new NavMeshPath();
            NavMesh.CalculatePath(mCurrentCharacter.transform.position, destination, NavMesh.AllAreas, path);
            if (path.corners.Length > 0)
            {  
                mMovementPathRenderer.positionCount = path.corners.Length + 1;
                mMovementPathRenderer.SetPosition(0, mCurrentCharacter.transform.position + Vector3.up * .1f);
                for (int i = 0; i < path.corners.Length; i++)
                {
                    mMovementPathRenderer.SetPosition(i + 1, path.corners[i]);
                }
            }
        }

        protected Vector3 GetPointAlongPathAtMaxRange(Vector3[] path, float maxRange)
        {
            return Vector3.zero;
        }


        protected float GetPathLength(List<Vector3> path)
        {
            return GetPathLength(path.ToArray());
        }
        protected float GetPathLength(Vector3[] path)
        {
            float length = 0;
            Debug.Assert(path != null && path.Length > 0);
            if (path != null && path.Length > 0)
            {
                for (int i = 1; i < path.Length; ++i)
                {
                    Vector3 lineSegmentStart = path[i - 1];
                    Vector3 lineSegmentEnd = path[i];

                    length += Vector3.Distance(lineSegmentStart, lineSegmentEnd);
                }
            }

            return length;
        }

        protected Vector3[] TrimPathToSize(Vector3[] path, float size)
        {
            List<Vector3> updatedPath = new List<Vector3>();

            if (path.Length > 0)
            {
                updatedPath.Add(path[0]);
                if (size > 0)
                {
                    float pathLength = 0;
                    for (int i = 1; i < path.Length; ++i)
                    {
                        Vector3 lineSegmentStart = path[i - 1];
                        Vector3 lineSegmentEnd = path[i];

                        float segmentLength = Vector3.Distance(lineSegmentStart, lineSegmentEnd);
                        if (pathLength + segmentLength > size)
                        {
                            float remainingLength = size - pathLength;

                            Vector3 finalPoint = lineSegmentStart + (lineSegmentEnd - lineSegmentStart).normalized * remainingLength;

                            updatedPath.Add(finalPoint);
                            break;
                        }
                        else
                        {
                            pathLength += segmentLength;
                            updatedPath.Add(lineSegmentEnd);
                        }
                    }
                }
            }

            return updatedPath.ToArray();
        }

        protected Vector3 GetPointAlongPathInRangeOf(Vector3[] path, Vector3 destination, float range)
        {
            float lineSegmentSize = 0.1f;
            for (int i = 1; i < path.Length; ++i)
            {
                Vector3 lineSegmentStart = path[i - 1];
                Vector3 lineSegmentEnd = path[i];

                Vector3 lineSegment = lineSegmentEnd - lineSegmentStart;
                Vector3 startToDestination = destination - lineSegmentStart;
                Vector3 endToDestination = destination - lineSegmentEnd;

                //double projectionDistance = Vector3.Dot(toDestination, lineSegment);
                //if (projectionDistance > lineSegment.magnitude)
                //{
                //    continue;
                //}
                //else if (projectionDistance )


                if (endToDestination.magnitude < range)
                {
                    float segmentLength = lineSegment.magnitude;
                    int numSegments = Mathf.CeilToInt(segmentLength / lineSegmentSize);
                    for (int j = 0; j < numSegments; ++j)
                    {
                        Vector3 pointOnSegment = lineSegmentStart + lineSegment * (j * lineSegmentSize) / segmentLength;
                        if ((destination - pointOnSegment).magnitude < range)
                        {
                            return pointOnSegment;
                        }
                    }
                }
            }

            return path[path.Length - 1];
        }
    }
}