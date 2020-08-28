using MAGE.GameModes.SceneElements;
using MAGE.GameSystems;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MAGE.GameModes.SceneElements
{
    class Level : MonoBehaviour
    {
        public LevelId LevelId;

        public Terrain Terrain;
        public TileContainer Tiles;
        public Transform SpawnPoint;
        public Transform ScenarioContainer;
        public Transform NPCContainer;
        public Dictionary<ScenarioId, Scenario> Scenarios = new Dictionary<ScenarioId, Scenario>();

        public TileContainerGenerator TileContainerGenerator;
        public TileContainer TemporaryTiles = null;
        public GameObject TreeColliderPrefab;
        //public Dictionary<NPCId, GameO> Scenarios = new Dictionary<ScenarioId, Scenario>();

        private void Awake()
        {
            if (!Tiles.gameObject.activeSelf)
            {
                Tiles.gameObject.SetActive(true);
            }

            
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
                        }
                    }
                }
            }

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
            }

            LevelManagementService.Get().NotifyLevelLoaded(this);
        }

        public void GenerateTilesAtPosition(Transform position)
        {
            if (TemporaryTiles != null)
            {
                Destroy(TemporaryTiles.gameObject);

                
            }

            TemporaryTiles = TileContainerGenerator.GenerateTiles(position, 15, 15, transform);
        }

        // Update is called once per frame
        void Update()
        {

        }
    }
}

