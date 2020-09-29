using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

[System.Serializable]
class Appearance
{
    public static int NO_ASSET = -1;

    public int AppearanceId = -1;
    public PortraitSpriteId PortraitSpriteId = PortraitSpriteId.INVALID;
    public BodyType BodyType = BodyType.Body_0;
    public AppearancePrefabId ArmorId = AppearancePrefabId.prefab_none;
    public AppearancePrefabId LeftHeldId = AppearancePrefabId.prefab_none;
    public AppearancePrefabId RightHeldId = AppearancePrefabId.prefab_none;
}
