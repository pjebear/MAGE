using System.Collections;
using System.Collections.Generic;
using UnityEngine;

class ModulesContainer : MonoBehaviour
{
    static ModulesContainer Container = null;

    public GameModesModule GameModesModule;
    public GameSystemModule GameSystemModule;
    public UIManager UIManager;

    public bool DebugRun = false;

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

            if (DebugRun)
            {
                GameSystemModule.Instance.PrepareNewGame();
            }
        }
    }
}
