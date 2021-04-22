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
        public Resources Resources = new Resources();

        public void InitResourcesFromAttributes()
        {
            Resources = new Resources(
              AttributeUtil.ResourceFromAttribtues(ResourceType.Health, GetComponent<StatsControl>().Attributes),
              AttributeUtil.ResourceFromAttribtues(ResourceType.Mana, GetComponent<StatsControl>().Attributes),
              AttributeUtil.ResourceFromAttribtues(ResourceType.Endurance, GetComponent<StatsControl>().Attributes),
              AttributeUtil.ResourceFromAttribtues(ResourceType.Clock, GetComponent<StatsControl>().Attributes),
              AttributeUtil.ResourceFromAttribtues(ResourceType.Actions, GetComponent<StatsControl>().Attributes),
              AttributeUtil.ResourceFromAttribtues(ResourceType.MovementRange, GetComponent<StatsControl>().Attributes));
        }

        public int GetNumAvailableActions()
        {
            return Resources[ResourceType.Actions].Current;
        }

        public void OnActionPerformed(StateChange actionCost)
        {
            Debug.Assert(GetNumAvailableActions() > 0);

            Resources[ResourceType.Actions].Modify(-1);

            ApplyStateChange(actionCost);
        }

        public int GetAvailableMovementRange()
        {
            return Resources[ResourceType.MovementRange].Current;
        }

        public void OnMovementPerformed(int movementRange)
        {
            Debug.Assert(GetAvailableMovementRange() >= movementRange);

            Resources[ResourceType.MovementRange].Modify(-movementRange);
        }

        public void OnStatsUpdated()
        {
            Attributes attributes = GetComponent<StatsControl>().Attributes;
            for (int i = 0; i < (int)ResourceType.NUM; ++i)
            {
                ResourceType resource = (ResourceType)i;
                Resources[resource].SetMax(AttributeUtil.ResourceFromAttribtues(resource, attributes));
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

        public bool HasResourcesForAction(StateChange actionCost)
        {
            bool hasResources = true;

            hasResources &= (Resources[ResourceType.Health].Current + actionCost.healthChange) > 0;
            hasResources &= (Resources[ResourceType.Mana].Current + actionCost.resourceChange) >= 0;

            foreach (StatusEffect statusEffect in actionCost.statusEffects)
            {
                int countRequirement = statusEffect.StackCount;
                int hasCount = GetComponent<StatusEffectControl>().GetStackCountForStatus(statusEffect.EffectType, GetComponent<CombatCharacter>().Character.Id);

                hasResources &= countRequirement >= hasCount;
            }

            return hasResources;
        }

        public void OnDeath()
        {
            GetComponent<AudioSource>().PlayOneShot(AudioManager.Instance.GetSFXClip(SFXId.MaleDeath));
            GetComponent<ActorAnimator>().Trigger("die");
            GetComponent<CapsuleCollider>().enabled = false;
        }
    }
}
