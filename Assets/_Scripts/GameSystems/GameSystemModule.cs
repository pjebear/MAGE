using MAGE.GameServices;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

class GameSystemModule : MonoBehaviour
{
    private readonly string TAG = "GameSystemModule";

    private static GameSystemModule Instance;

    public void InitModule()
    {
        Logger.Log(LogTag.GameSystems, TAG, "::InitModule()");
        Logger.Assert(Instance == null, LogTag.GameSystems, TAG, "::InitModule() - Already initialized!");

        if (Instance == null)
        {
            Instance = this;

            WorldService.Register(new MAGE.GameServices.World.Internal.WorldServiceImpl());
            CharacterService.Register(new MAGE.GameServices.Character.Internal.CharacterServiceImpl());
            StoryService.Register(new MAGE.GameServices.Story.Internal.StoryServiceImpl());
            DBService.Register(new MAGE.DB.Internal.DBServiceImpl());
            MAGE.GameModes.LevelManagementService.Register(gameObject.AddComponent<MAGE.GameModes.LevelManagement.Internal.LevelManagerServiceImpl>());

            DBService.Get().Init();

            MAGE.GameModes.LevelManagementService.Get().Init();
            WorldService.Get().Init();
            StoryService.Get().Init();
            CharacterService.Get().Init();
        }
    }
}

