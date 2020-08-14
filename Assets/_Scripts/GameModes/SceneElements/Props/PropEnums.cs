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

    enum NPCPropId
    {
        None = -1,

        FieldVendor = PropConstants.NPC_ID_OFFSET,

        NUM
    }
}


