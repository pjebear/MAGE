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
        protected enum State
        {
            Idle,
            Moving,
            TargetSelect
        }
        protected State mState = State.Idle;

        // Private 
        protected CombatCharacter mCurrentCharacter;
        protected List<ActionId> mAvailableActions;
        protected CombatCharacter mCurrentTarget;
        protected Vector3 mCurrentMoveToPoint = Vector3.zero;

        protected LineRenderer mAbilityRangeRenderer = null;
        protected LineRenderer mAbilityEffectRenderer = null;
        protected LineRenderer mMovementRangeRenderer = null;

        protected int mCircleSegments = 32;

        protected Dictionary<CombatCharacter, NavMeshObstacle> mMovementObstacles = new Dictionary<CombatCharacter, NavMeshObstacle>();

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

            mMovementRangeRenderer = EncounterPrefabLoader.RangeRenderer;
            mMovementRangeRenderer.transform.SetParent(transform);
            mMovementRangeRenderer.gameObject.SetActive(false);
            mMovementRangeRenderer.useWorldSpace = true;

            foreach (CombatCharacter combatCharacter in GameModel.Encounter.Players)
            {
                if (combatCharacter.GetComponent<ResourcesControl>().IsAlive())
                {
                    NavMeshObstacle obstacle = EncounterPrefabLoader.Obstacle;
                    obstacle.transform.SetParent(combatCharacter.transform);
                    obstacle.transform.localPosition = Vector3.zero;
                    mMovementObstacles.Add(combatCharacter, obstacle);
                }
            }

            SetCurrentCharacter(GameModel.Encounter.CurrentTurn);

            SetState(State.Idle);
        }

        protected override void Cleanup()
        {
            foreach (var obstaclePair in mMovementObstacles)
            {
                Destroy(obstaclePair.Value.gameObject);
            }
            mMovementObstacles.Clear();
        }


        private void SetCurrentCharacter(CombatCharacter combatCharacter)
        {
            mCurrentCharacter = combatCharacter;
            mAvailableActions = combatCharacter.GetComponent<ActionsControl>().Actions;
            mMovementObstacles[combatCharacter].enabled = false;

            Camera.main.GetComponent<Cameras.CameraController>().SetTarget(mCurrentCharacter.transform, Cameras.CameraType.TopDown);
        }

        protected void MoveAttack()
        {
            bool attackQueued = false;
            if (!GameModel.Encounter.HasActed)
            {
                attackQueued = true;
                GameModel.Encounter.HasActed = true;

                ActionProposal proposal = new ActionProposal(
                mCurrentCharacter.GetComponent<CombatEntity>(),
                new Target(mCurrentTarget.GetComponent<CombatTarget>()),
                ActionId.MeeleWeaponAttack);

                GameModel.Encounter.mActionQueue.Enqueue(proposal);
            }

            if ((mCurrentTarget.transform.position - mCurrentCharacter.transform.position).magnitude > 1.5f)
            {
                SetState(State.Moving);

                GameModel.Encounter.HasMoved = true;
                mCurrentCharacter.GetComponent<ActorMotor>().MoveToPoint(mCurrentMoveToPoint, () =>
                {
                    SendFlowMessage("actionChosen");
                });
            }
            else if (attackQueued)
            {
                SendFlowMessage("actionChosen");
            }
        }

        protected virtual void SetState(State state)
        {
            mState = state;

            if (state == State.Idle)
            {
                mMovementRangeRenderer.gameObject.SetActive(!GameModel.Encounter.HasMoved);
                mAbilityRangeRenderer.gameObject.SetActive(false);
                mAbilityEffectRenderer.gameObject.SetActive(false);
            }
            else if (state == State.TargetSelect)
            {
                mMovementRangeRenderer.gameObject.SetActive(false);
                mAbilityRangeRenderer.gameObject.SetActive(true);
                mAbilityEffectRenderer.gameObject.SetActive(true);
            }
            else if (state == State.Moving)
            {
                mMovementRangeRenderer.gameObject.SetActive(false);
                mAbilityRangeRenderer.gameObject.SetActive(false);
                mAbilityEffectRenderer.gameObject.SetActive(false);
            }
        }


        protected IDataProvider PublishCharacterInfoPanelLeft()
        {
            CharacterInspector.DataProvider dp = new CharacterInspector.DataProvider();

            Character toPublish = mCurrentCharacter.Character;

            dp.IsAlly = toPublish.TeamSide == TeamSide.AllyHuman;
            dp.PortraitAsset = toPublish.GetAppearance().PortraitSpriteId.ToString();
            dp.Name = toPublish.Name;
            dp.Level = toPublish.Level;
            dp.Exp = toPublish.Experience;
            dp.Specialization = toPublish.CurrentSpecializationType.ToString();

            dp.CurrentHP    = mCurrentCharacter.GetComponent<ResourcesControl>().Resources[ResourceType.Health].Current;
            dp.MaxHP        = mCurrentCharacter.GetComponent<ResourcesControl>().Resources[ResourceType.Health].Max;
            dp.CurrentMP    = mCurrentCharacter.GetComponent<ResourcesControl>().Resources[ResourceType.Mana].Current;
            dp.MaxMP        = mCurrentCharacter.GetComponent<ResourcesControl>().Resources[ResourceType.Mana].Max;

            dp.Might        = (int)mCurrentCharacter.GetComponent<StatsControl>().Attributes[PrimaryStat.Might];
            dp.Finesse      = (int)mCurrentCharacter.GetComponent<StatsControl>().Attributes[PrimaryStat.Finese];
            dp.Magic        = (int)mCurrentCharacter.GetComponent<StatsControl>().Attributes[PrimaryStat.Magic];

            dp.Fortitude    = (int)mCurrentCharacter.GetComponent<StatsControl>().Attributes[SecondaryStat.Fortitude];
            dp.Attunement   = (int)mCurrentCharacter.GetComponent<StatsControl>().Attributes[SecondaryStat.Attunement];

            dp.Block        = (int)mCurrentCharacter.GetComponent<StatsControl>().Attributes[TertiaryStat.Block];
            dp.Dodge        = (int)mCurrentCharacter.GetComponent<StatsControl>().Attributes[TertiaryStat.Dodge];
            dp.Parry        = (int)mCurrentCharacter.GetComponent<StatsControl>().Attributes[TertiaryStat.Parry];

            List<IDataProvider> statusEffects = new List<IDataProvider>();
            foreach (StatusEffect effect in mCurrentCharacter.GetComponent<StatusEffectControl>().StatusEffects)
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

        protected void DisplayRange(LineRenderer rangeRender, Vector3 center, float radius)
        {
            rangeRender.gameObject.SetActive(true);

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

                rangeRender.SetPosition(i, circlePosition);

                currentAngle += 360f / mCircleSegments;
            }
        }

        protected void DisplayMovementRange(Vector3 destination)
        {
            mMovementRangeRenderer.gameObject.SetActive(true);

            NavMeshPath path = new NavMeshPath();
            NavMesh.CalculatePath(mCurrentCharacter.transform.position, destination, NavMesh.AllAreas, path);

            mMovementRangeRenderer.positionCount = path.corners.Length + 1;
            mMovementRangeRenderer.SetPosition(0, mCurrentCharacter.transform.position + Vector3.up * .1f);
            for (int i = 0; i < path.corners.Length; i++)
            {
                mMovementRangeRenderer.SetPosition(i + 1, path.corners[i]);
            }
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