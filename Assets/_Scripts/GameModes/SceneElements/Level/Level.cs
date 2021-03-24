using MAGE.GameModes.SceneElements;
using MAGE.GameSystems;
using MAGE.GameSystems.Actions;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace MAGE.GameModes.SceneElements
{
    class Level : MonoBehaviour
    {
        public LevelId LevelId;

        public Terrain Terrain = null;
        public Transform SpawnPointContainer;
        public Transform CinematicContainer;
        public Transform EncounterContainer;
        public Transform NPCContainer;
        public Dictionary<ScenarioId, Scenario> Scenarios = new Dictionary<ScenarioId, Scenario>();
        public Actor Player;
        public TileContainerGenerator TileContainerGenerator;
        public GameObject TreeColliderPrefab;
        public List<CapsuleCollider> TreeColliders = new List<CapsuleCollider>();

        public ActorLoader ActorLoader;
        //public Dictionary<NPCId, GameO> Scenarios = new Dictionary<ScenarioId, Scenario>();

        private void Awake()
        {
            ActorLoader = gameObject.AddComponent<ActorLoader>();
        }

        // Start is called before the first frame update
        void Start()
        {
            if (Terrain != null)
            {
                //foreach (TreeInstance tree in Terrain.terrainData.treeInstances)
                //{
                //    int protypeIndex = tree.prototypeIndex;
                //    TreePrototype prototype = Terrain.terrainData.treePrototypes[protypeIndex];
                //    float baseTreeRadius = prototype.prefab.GetComponent<CapsuleCollider>().radius;
                //    float scaledTreeRadius = tree.widthScale * baseTreeRadius;
                //    float xzScale = scaledTreeRadius / 0.5f;

                //    GameObject capsule = Instantiate(TreeColliderPrefab, Terrain.transform);
                //    capsule.transform.localPosition = Vector3.Scale(tree.position, Terrain.terrainData.size);
                //    capsule.transform.localScale = new Vector3(xzScale, 1, xzScale);
                //    TreeColliders.Add(capsule.GetComponentInChildren<CapsuleCollider>());
                //}
            }
        }

        public void GenerateTilesAtPosition(Transform parent)
        {
             TileContainerGenerator.GenerateTiles(parent.position, 15, 15, parent);
        }

        public TileContainer GenerateTiles(Vector3 center, int width, int height, Transform parent)
        {
            return TileContainerGenerator.GenerateTiles(center, width, height, parent);
        }

        public void ToggleTreeColliders(bool on)
        {
            foreach (CapsuleCollider collider in TreeColliders)
            {
                collider.isTrigger = !on;
            }
        }

        // Update is called once per frame
        void Update()
        {

        }

        public List<EncounterContainer_Deprecated> GetActiveEncounters()
        {
            List<EncounterContainer_Deprecated> encounters = new List<EncounterContainer_Deprecated>();
            if (EncounterContainer != null)
            {
                encounters = EncounterContainer.GetComponentsInChildren<EncounterContainer_Deprecated>().Where(x => x.IsEncounterPending).ToList();
            }
            return encounters;
        }

        public List<CinematicMoment> GetActiveCinematics()
        {
            List<CinematicMoment> cinematics = new List<CinematicMoment>();
            if (CinematicContainer != null)
            {
                cinematics = CinematicContainer.GetComponentsInChildren<CinematicMoment>().Where(x => x.CinematicReady).ToList();
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

        public Combat.CombatCharacter CreateCombatCharacter()
        {
            return Instantiate(Resources.Load<Combat.CombatCharacter>("Props/ActorSpawner/CombatCharacter"));
        }

        public List<Combat.CombatTarget> GetTargetsInRange(Combat.CombatEntity castPoint, TargetSelection targetSelection)
        {
            List<Combat.CombatTarget> combatTargets = new List<Combat.CombatTarget>();

            switch (targetSelection.SelectionRange.AreaType)
            {   
                case AreaType.Circle:
                {
                    Vector3 centerPoint = Vector3.zero;
                    if (targetSelection.FocalTarget.TargetType == TargetSelectionType.Point)
                    {
                        centerPoint = targetSelection.FocalTarget.PointTarget.position;
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

