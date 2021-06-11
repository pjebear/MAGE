using MAGE.GameModes.SceneElements;
using MAGE.GameSystems.Stats;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using Resources = MAGE.GameSystems.Stats.Resources;

namespace MAGE.GameModes.Combat
{
    class ResourcesControl : MonoBehaviour
    {
        public int Health = -1;
        public Resources Resources = new Resources();

        public void Awake()
        {
            Resources = new Resources(
              Health,
              0,
              0,
              0,
              0,
              0);
        }

        public void InitResourcesFromAttributes(Attributes attributes)
        {
            Resources = new Resources(
              AttributeUtil.ResourceFromAttribtues(ResourceType.Health, attributes),
              AttributeUtil.ResourceFromAttribtues(ResourceType.Mana, attributes),
              AttributeUtil.ResourceFromAttribtues(ResourceType.Endurance, attributes),
              AttributeUtil.ResourceFromAttribtues(ResourceType.Clock, attributes),
              AttributeUtil.ResourceFromAttribtues(ResourceType.Actions, attributes),
              AttributeUtil.ResourceFromAttribtues(ResourceType.MovementRange, attributes));
        }

        public void OnStatsUpdated()
        {
            Attributes attributes = GetComponent<StatsControl>().Attributes;
            for (int i = 0; i < (int)ResourceType.NUM; ++i)
            {
                ResourceType resource = (ResourceType)i;

                Resources[resource].SetMax(
                    AttributeUtil.ResourceFromAttribtues(resource, attributes)
                    , AttributeUtil.GetScaleTypeForResource(resource));
            }
        }

        public void ApplyStateChange(StateChange stateChange)
        {
            if (stateChange.healthChange != 0)
            {
                int healthChange = stateChange.healthChange;
                if (healthChange > 0)
                {
                    healthChange = Mathf.Min(healthChange, Resources[ResourceType.Health].Max - Resources[ResourceType.Health].Current);
                }
                
                Billboard.Params param = new Billboard.Params();
                param.anchor = transform;
                param.offset = new Vector3(1f, 2f, 0);
                param.text = healthChange.ToString();
                GetComponent<BillboardEmitter>().Emitt(param, 2f);
            }

            Resources[ResourceType.Health].Modify(stateChange.healthChange);
            Resources[ResourceType.Mana].Modify(stateChange.resourceChange);
            Resources[ResourceType.Endurance].Modify(stateChange.resourceChange);

            if (Resources[ResourceType.Health].Current <= 0)
            {
                BroadcastMessage("OnDeath");
            }
        }

        public bool IsAlive()
        {
            return Resources[GameSystems.Stats.ResourceType.Health].Current > 0;
        }
    }
}
