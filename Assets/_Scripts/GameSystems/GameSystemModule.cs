using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

class GameSystemModule : MonoBehaviour
{
    private readonly string TAG = "GameSystemModule";

    public static GameSystem Instance;

    public void InitModule()
    {
        Logger.Log(LogTag.GameSystems, TAG, "::InitModule()");
        Logger.Assert(Instance == null, LogTag.GameSystems, TAG, "::InitModule() - Already initialized!");

        Instance = new GameSystem();
    }
}

