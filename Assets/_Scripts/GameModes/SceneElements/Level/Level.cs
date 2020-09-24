using MAGE.GameModes.SceneElements;
using MAGE.GameSystems;
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
        public Transform ScenarioContainer;
        public Transform CinematicContainer;
        public Transform EncounterContainer;
        public Transform NPCContainer;
        public Dictionary<ScenarioId, Scenario> Scenarios = new Dictionary<ScenarioId, Scenario>();

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
            for (int i = 0; i < ScenarioContainer.childCount; ++i)
            {
                Scenario scenario = ScenarioContainer.GetChild(i).GetComponent<Scenario>();
                Logger.Assert(scenario != null, LogTag.Level, LevelId.ToString(), string.Format("Failed to find Scenario script on scenario object [{0}]", ScenarioContainer.GetChild(i).name));
                if (scenario != null)
                {
                    Logger.Assert(scenario.ScenarioId != ScenarioId.INVALID, LogTag.Level, LevelId.ToString(), string.Format("ScenarioId not set for scenario object [{0}]", scenario.gameObject.name));
                    if (scenario.ScenarioId != ScenarioId.INVALID)
                    {
                        Logger.Assert(!Scenarios.ContainsKey(scenario.ScenarioId), LogTag.Level, LevelId.ToString(), string.Format("Duplicate ScenarioId [{1}] found for scenario object [{0}] ", scenario.gameObject.name, scenario.ScenarioId.ToString()));
                        if (!Scenarios.ContainsKey(scenario.ScenarioId))
                        {
                            Scenarios.Add(scenario.ScenarioId, scenario);
                            scenario.Init();
                        }
                    }
                }
            }

            if (Terrain != null)
            {
                foreach (TreeInstance tree in Terrain.terrainData.treeInstances)
                {
                    int protypeIndex = tree.prototypeIndex;
                    TreePrototype prototype = Terrain.terrainData.treePrototypes[protypeIndex];
                    float baseTreeRadius = prototype.prefab.GetComponent<CapsuleCollider>().radius;
                    float scaledTreeRadius = tree.widthScale * baseTreeRadius;
                    float xzScale = scaledTreeRadius / 0.5f;

                    GameObject capsule = Instantiate(TreeColliderPrefab, Terrain.transform);
                    capsule.transform.localPosition = Vector3.Scale(tree.position, Terrain.terrainData.size);
                    capsule.transform.localScale = new Vector3(xzScale, 1, xzScale);
                    TreeColliders.Add(capsule.GetComponentInChildren<CapsuleCollider>());
                }
            }

            LevelManagementService.Get().NotifyLevelLoaded(this);
        }

        public void GenerateTilesAtPosition(Transform position)
        {
             TileContainerGenerator.GenerateTiles(position, 15, 15, transform);
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

        public List<EncounterContainer> GetActiveEncounters()
        {
            return EncounterContainer.GetComponentsInChildren<EncounterContainer>().Where(x => x.IsEncounterPending).ToList();
        }

        public List<CinematicMoment> GetActiveCinematics()
        {
            return CinematicContainer.GetComponentsInChildren<CinematicMoment>().Where(x => x.CinematicReady).ToList();
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
    }
}

