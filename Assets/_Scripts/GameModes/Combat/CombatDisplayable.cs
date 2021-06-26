using MAGE.GameSystems.Actions;
using MAGE.GameSystems.Characters;
using MAGE.GameSystems.Stats;
using MAGE.UI.Views;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace MAGE.GameModes.Combat
{
    class CombatDisplayable : MonoBehaviour
    {
        public CharacterInspector.DataProvider GetDisplayDP()
        {
            CharacterInspector.DataProvider dp = new CharacterInspector.DataProvider();

            if (GetComponent<ControllableEntity>() != null)
            {
                ControllableEntity combatCharacter = GetComponent<ControllableEntity>();
                Character toPublish = combatCharacter.Character;

                dp.IsAlly = combatCharacter.TeamSide == TeamSide.AllyHuman;
                dp.PortraitAsset = toPublish.Appearance.PortraitSpriteId.ToString();
                dp.Name = toPublish.Name;
                dp.Level = toPublish.Level;
                dp.Exp = toPublish.Experience;
                dp.Specialization = toPublish.CurrentSpecializationType.ToString();

                dp.CurrentHP = combatCharacter.GetComponent<ResourcesControl>().Resources[ResourceType.Health].Current;
                dp.MaxHP = combatCharacter.GetComponent<ResourcesControl>().Resources[ResourceType.Health].Max;
                dp.CurrentMP = combatCharacter.GetComponent<ResourcesControl>().Resources[ResourceType.Mana].Current;
                dp.MaxMP = combatCharacter.GetComponent<ResourcesControl>().Resources[ResourceType.Mana].Max;
                dp.CurrentClock = combatCharacter.GetComponent<ResourcesControl>().Resources[ResourceType.Clock].Current;

                dp.Might = (int)combatCharacter.GetComponent<StatsControl>().Attributes[PrimaryStat.Might];
                dp.Finesse = (int)combatCharacter.GetComponent<StatsControl>().Attributes[PrimaryStat.Finese];
                dp.Magic = (int)combatCharacter.GetComponent<StatsControl>().Attributes[PrimaryStat.Magic];

                dp.Fortitude = (int)combatCharacter.GetComponent<StatsControl>().Attributes[SecondaryStat.Fortitude];
                dp.Attunement = (int)combatCharacter.GetComponent<StatsControl>().Attributes[SecondaryStat.Attunement];

                float block, dodge, parry;
                InteractionUtil.GetAvoidanceAttributesForEntity(GetComponent<CombatEntity>(), out dodge, out block, out parry);

                dp.Block = (int)block;
                dp.Dodge = (int)dodge;
                dp.Parry = (int)parry;

                List<IDataProvider> statusEffects = new List<IDataProvider>();
                foreach (StatusEffect effect in combatCharacter.GetComponent<StatusEffectControl>().mStatusEffectLookup.Values)
                {
                    StatusIcon.DataProvider statusDp = new StatusIcon.DataProvider();
                    statusDp.Count = effect.StackCount;
                    statusDp.IsBeneficial = effect.Beneficial;
                    statusDp.AssetName = effect.SpriteId.ToString();
                    statusEffects.Add(statusDp);
                }
                dp.StatusEffects = new UIList.DataProvider(statusEffects);
            }
            else
            {
                dp.IsAlly = GetComponent<CombatEntity>().TeamSide == TeamSide.AllyHuman;
                //dp.PortraitAsset = combatCharacter.SummonType.ToString();
                //dp.Name = combatCharacter.SummonType.ToString();

                if (GetComponent<ResourcesControl>())
                {
                    dp.CurrentHP = GetComponent<ResourcesControl>().Resources[ResourceType.Health].Current;
                    dp.MaxHP = GetComponent<ResourcesControl>().Resources[ResourceType.Health].Max;
                    dp.CurrentMP = GetComponent<ResourcesControl>().Resources[ResourceType.Mana].Current;
                    dp.MaxMP = GetComponent<ResourcesControl>().Resources[ResourceType.Mana].Max;
                    dp.CurrentClock = GetComponent<ResourcesControl>().Resources[ResourceType.Clock].Current;
                }
                
                if (GetComponent<StatsControl>())
                {
                    dp.Might = (int)GetComponent<StatsControl>().Attributes[PrimaryStat.Might];
                    dp.Finesse = (int)GetComponent<StatsControl>().Attributes[PrimaryStat.Finese];
                    dp.Magic = (int)GetComponent<StatsControl>().Attributes[PrimaryStat.Magic];

                    dp.Fortitude = (int)GetComponent<StatsControl>().Attributes[SecondaryStat.Fortitude];
                    dp.Attunement = (int)GetComponent<StatsControl>().Attributes[SecondaryStat.Attunement];

                    float block, dodge, parry;
                    InteractionUtil.GetAvoidanceAttributesForEntity(GetComponent<CombatEntity>(), out dodge, out block, out parry);

                    dp.Block = (int)block;
                    dp.Dodge = (int)dodge;
                    dp.Parry = (int)parry;
                }

                if (GetComponent<StatusEffectControl>())
                {
                    List<IDataProvider> statusEffects = new List<IDataProvider>();
                    foreach (StatusEffect effect in GetComponent<StatusEffectControl>().mStatusEffectLookup.Values)
                    {
                        StatusIcon.DataProvider statusDp = new StatusIcon.DataProvider();
                        statusDp.Count = effect.StackCount;
                        statusDp.IsBeneficial = effect.Beneficial;
                        statusDp.AssetName = effect.SpriteId.ToString();
                        statusEffects.Add(statusDp);
                    }
                    dp.StatusEffects = new UIList.DataProvider(statusEffects);
                }
            }

            return dp;
        }
    }
}
