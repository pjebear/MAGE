using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MAGE.GameModes.SceneElements
{
    enum PropType
    {
        Environment,
        Container,
        NPC,
        Door,

        NUM
    }

    enum EnvironmentPropId
    {
        None = -1,

        NUM
    }

    enum ContainerPropId
    {
        None = -1,

        FieldVendorContainer = PropConstants.CONTAINER_ID_OFFSET,
    

        NUM
    }

    enum DoorPropId
    {
        None = -1,

        DemoLevel_TownGateFront = PropConstants.DOOR_ID_OFFSET,


        NUM
    }

    enum NPCPropId
    {
        None = -1,

        FieldVendor = PropConstants.NPC_ID_OFFSET,

        // Demo Level
        DemoLevel_GateGuard0,
        DemoLevel_GateGuard1,
        DemoLevel_Vendor,
        DemoLevel_Magistrate,
        DemoLevel_GuildLeader,
        DemoLevel_Captain,


        NUM
    }
}


