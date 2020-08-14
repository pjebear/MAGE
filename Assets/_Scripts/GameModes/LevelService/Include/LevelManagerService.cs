using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MAGE.GameModes
{
    interface ILevelManagerService : Services.IService
    {
        GameModes.SceneElements.Level GetLoadedLevel();
        SceneElements.PropInfo GetPropInfo(int propId);
        Appearance GetAppearance(int appearanceId);
        void LoadLevel(GameServices.LevelId levelId);
        void NotifyLevelLoaded(GameModes.SceneElements.Level level);
        void UnloadLevel();
        void UpdatePropInfo(SceneElements.PropInfo updatedInfo);
    }

    abstract class LevelManagementService : Services.ServiceBase<ILevelManagerService> { }
}
