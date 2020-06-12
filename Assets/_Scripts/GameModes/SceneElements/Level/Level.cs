using System.Collections;
using System.Collections.Generic;
using UnityEngine;

class Level : MonoBehaviour
{
    public LevelId LevelId;
    
    public Terrain Terrain;
    public TileContainer Tiles;
    public Transform SpawnPoint;
    public Transform ScenarioContainer;
    public Transform NPCContainer;
    public Dictionary<ScenarioId, Scenario> Scenarios = new Dictionary<ScenarioId, Scenario>();
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

        if (GameModesModule.LevelManager != null)
        {
            GameModesModule.LevelManager.NotifyLevelLoaded(this);
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
