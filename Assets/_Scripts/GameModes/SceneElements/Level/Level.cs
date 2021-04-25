using MAGE.GameModes.SceneElements;
using MAGE.GameSystems;
using MAGE.GameSystems.Actions;
using MAGE.Utility.Enums;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace MAGE.GameModes.SceneElements
{
    class Level : MonoBehaviour
    {
        [SerializeField] string _LevelId;
        public LevelId LevelId { get { return EnumUtil.StringToEnum<LevelId>(_LevelId); } }

        [SerializeField] string _TrackId;       
        public TrackId TrackId { get { return EnumUtil.StringToEnum<TrackId>(_TrackId); } }

        public Terrain Terrain = null;
        public Transform SpawnPointContainer;
        public Transform CinematicContainer;
        public Transform EncounterContainer;
        public Transform NPCContainer;
        public Dictionary<ScenarioId, Scenario> Scenarios = new Dictionary<ScenarioId, Scenario>();
        public Actor Player;
      
        private void Awake()
        {
            
        }

        // Start is called before the first frame update
        void Start()
        {
           
        }

        // Update is called once per frame
        void Update()
        {

        }

        public List<EncounterContainer> GetActiveEncounters()
        {
            List<EncounterContainer> encounters = new List<EncounterContainer>();
            if (EncounterContainer != null)
            {
                encounters = EncounterContainer.GetComponentsInChildren<EncounterContainer>().Where(x => x.GetIsEncounterPending()).ToList();
            }
            return encounters;
        }

        public List<CinematicMoment> GetActiveCinematics()
        {
            List<CinematicMoment> cinematics = new List<CinematicMoment>();
            if (CinematicContainer != null)
            {
                cinematics = CinematicContainer.GetComponentsInChildren<CinematicMoment>().Where(x => x.IsCinematicReady()).ToList();
            }
            return cinematics;
        }

        public Transform GetSpawnPoint(int spawnPointIdx)
        {
            Transform spawnPoint = null;

            Debug.Assert(SpawnPointContainer.childCount > spawnPointIdx);
            if (spawnPointIdx < SpawnPointContainer.childCount)
            {
                spawnPoint = SpawnPointContainer.GetChild(spawnPointIdx);
            }

            return spawnPoint;
        }

        public EncounterContainer CreateEncounter()
        {
            EncounterContainer encounter = Instantiate(Resources.Load<EncounterContainer>("EncounterPrefabs/EncounterContainer"));
            encounter.transform.SetParent(EncounterContainer);
            return encounter;
        }

        public EncounterContainer GetActiveEncounter()
        {
            return EncounterContainer.GetComponentInChildren<EncounterContainer>();
        }

        public Combat.CombatCharacter CreateCombatCharacter(Vector3 position, Quaternion rotation, Transform parent)
        {
            return Instantiate(Resources.Load<Combat.CombatCharacter>("Props/ActorSpawner/CombatCharacter"), position, rotation, parent);
        }

        public List<Combat.CombatTarget> GetTargetsInRange(Combat.CombatEntity castPoint, TargetSelection targetSelection)
        {
            List<Combat.CombatTarget> combatTargets = new List<Combat.CombatTarget>();

            switch (targetSelection.SelectionRange.AreaType)
            {   
                case AreaType.Cone:
                {
                    Vector3 coneDirection = Vector3.zero;
                    if (targetSelection.FocalTarget.TargetType == TargetSelectionType.Point)
                    {
                        coneDirection = targetSelection.FocalTarget.PointTarget;
                    }
                    else if (targetSelection.FocalTarget.TargetType == TargetSelectionType.Focal)
                    {
                        coneDirection = targetSelection.FocalTarget.FocalTarget.transform.position;
                    }

                    coneDirection = coneDirection - castPoint.transform.position;

                    combatTargets = Physics.OverlapSphere(castPoint.transform.position, targetSelection.SelectionRange.MaxRange)
                        .Select(x => x.gameObject.GetComponent<Combat.CombatTarget>())
                        .Where(x => x != null).ToList();
                    combatTargets = FilterTargetsByTargetType(castPoint, targetSelection.SelectionRange.TargetingType, combatTargets);

                    combatTargets = combatTargets.Where(x =>
                    {
                        Vector3 toTarget = x.transform.position - castPoint.transform.position;
                        float angleBetween = Vector3.SignedAngle(coneDirection, toTarget, Vector3.up);
                        return angleBetween >= -22.5f && angleBetween <= 22.5f;
                    }).ToList();
                    
                }
                break;
                case AreaType.Circle:
                {
                    Vector3 centerPoint = Vector3.zero;
                    if (targetSelection.FocalTarget.TargetType == TargetSelectionType.Point)
                    {
                        centerPoint = targetSelection.FocalTarget.PointTarget;
                    }
                    else if (targetSelection.FocalTarget.TargetType == TargetSelectionType.Focal)
                    {
                        centerPoint = targetSelection.FocalTarget.FocalTarget.transform.position;
                    }

                    combatTargets = Physics.OverlapSphere(centerPoint, targetSelection.SelectionRange.MaxRange)
                        .Select(x => x.gameObject.GetComponent<Combat.CombatTarget>())
                        .Where(x => x != null).ToList();
                    combatTargets = FilterTargetsByTargetType(castPoint, targetSelection.SelectionRange.TargetingType, combatTargets);
                }
                break;

                case AreaType.Chain:
                {
                    if (targetSelection.FocalTarget.FocalTarget != null)
                    {
                        combatTargets.Add(targetSelection.FocalTarget.FocalTarget);

                        Transform currentLink = targetSelection.FocalTarget.FocalTarget.transform;
                        while (currentLink != null)
                        {
                            List<Combat.CombatTarget> targets = Physics.OverlapSphere(currentLink.position, targetSelection.SelectionRange.MaxRange)
                            .Select(x => x.gameObject.GetComponent<Combat.CombatTarget>())
                            .Where(x => x != null && !combatTargets.Contains(x) && x.GetComponent<Combat.ResourcesControl>().IsAlive())
                            .ToList();
                            targets = FilterTargetsByTargetType(castPoint, targetSelection.SelectionRange.TargetingType, targets);

                            targets.Sort((x, y) => Vector3.Distance(x.transform.position, currentLink.transform.position).CompareTo(Vector3.Distance(y.transform.position, currentLink.transform.position)));
                            currentLink = null;

                            foreach (Combat.CombatTarget target in targets)
                            {
                                if (!combatTargets.Contains(target))
                                {
                                    combatTargets.Add(target);
                                    currentLink = target.transform;
                                    break;
                                }
                            }
                        }
                    }
                    
                }
                break;
                default:
                    Debug.Assert(false);
                    break;
            }

            return combatTargets;
        }

        List<Combat.CombatTarget> FilterTargetsByTargetType(Combat.CombatEntity caster, TargetingType targetingType, List<Combat.CombatTarget> targets)
        {
            switch (targetingType)
            {
                case TargetingType.Enemies:
                    return targets.Where(x => x.GetComponent<Combat.CombatEntity>().TeamSide != caster.TeamSide).ToList();
                case TargetingType.Allies:
                    return targets.Where(x => x.GetComponent<Combat.CombatEntity>().TeamSide == caster.TeamSide).ToList();
                default:
                    return targets;
            }
        }
    }
}

