using System.Collections;
using System.Collections.Generic;
using UnityEngine;

class ModulesContainer : MonoBehaviour
{
    public static ModulesContainer Container = null;

    public GameModesModule GameModesModule;
    public GameSystemModule GameSystemModule;
    public UIManager UIManager;

    public bool DebugEncounter = false;

    private void Awake()
    {
        if (Container != null)
        {
            DestroyImmediate(gameObject);
        }
        else
        {
            Container = this;
            DontDestroyOnLoad(gameObject);

            GameSystemModule.InitModule();
            GameModesModule.InitModule();
            UIManager.Initialize();

            if (DebugEncounter)
            {
                GameSystemModule.Instance.PrepareNewGame();
                EncounterCreateParams encounterCreateParams = new EncounterCreateParams();
                encounterCreateParams.BottomLeft = new TileIdx(12, 4);
                encounterCreateParams.TopRight = new TileIdx(24, 8);
                GameSystemModule.Instance.PrepareEncounter(encounterCreateParams);
                GameModesModule.Encounter();
            }
            else
            {
                GameModesModule.MainMenu();
            }
        }
    }
}
