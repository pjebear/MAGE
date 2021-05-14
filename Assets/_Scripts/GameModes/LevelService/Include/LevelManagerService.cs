using MAGE.GameSystems;
using MAGE.GameSystems.Appearances;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MAGE.GameModes
{
    interface ILevelManagerService : Services.IService
    {
        CinematicInfo GetCinematicInfo(int cinematicId);
        EncounterInfo GetEncounterInfo(int encounterId);
        GameModes.SceneElements.Level GetLoadedLevel();
        SceneElements.PropInfo GetPropInfo(int propId);
        void LoadLevel(GameSystems.LevelId levelId);
        void UnloadLevel();
        void UpdateEncounterInfo(EncounterInfo encounterInfo);
        void UpdatePropInfo(SceneElements.PropInfo updatedInfo);
    }

    abstract class LevelManagementService : Services.ServiceBase<ILevelManagerService> { }
}
