using MAGE.GameServices.Character;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MAGE.GameModes.SceneElements
{
    static class PropConstants
    {
        public const int CONTAINER_ID_OFFSET    = Common.GameConstants.PROP_ID_OFFSET + Common.GameConstants.CATEGORY_ID_OFFSET * (int)PropType.Environment;
        public const int ENVIRONMENT_ID_OFFSET  = Common.GameConstants.PROP_ID_OFFSET + Common.GameConstants.CATEGORY_ID_OFFSET * (int)PropType.Container;
        public const int NPC_ID_OFFSET          = Common.GameConstants.PROP_ID_OFFSET + Common.GameConstants.CATEGORY_ID_OFFSET * (int)PropType.NPC;
    }

    struct PropTag
    {
        public int Id;
        public PropType PropType;

        public PropTag(int id) : this(id, PropUtil.PropTypeFromId(id)) { }
        public PropTag(ContainerPropId id) : this((int)id, PropType.Container) { }
        public PropTag(EnvironmentPropId id) : this((int)id, PropType.Environment) { }
        public PropTag(NPCPropId id) : this((int)id, PropType.NPC) { }
        public PropTag(int id, PropType propType)
        {
            Id = id;
            PropType = propType;
        }
    }

    class PropInfo
    {
        public PropTag Tag;
        public string Name = "MISSING";
        public bool IsActive = true;
        public bool IsInteractible = true;
        public int Currency = 0;
        public int AppearanceId = -1;
        public List<GameServices.Item> Inventory = new List<GameServices.Item>();
        public List<GameServices.Conversation> Conversations = new List<GameServices.Conversation>();
    }
}



