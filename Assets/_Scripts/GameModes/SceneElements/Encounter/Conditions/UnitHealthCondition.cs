using MAGE.GameModes.Combat;
using MAGE.GameModes.Encounter;
using MAGE.GameSystems.Stats;
using MAGE.Utility;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MAGE.GameModes.SceneElements.Encounters
{
    class UnitHealthCondition : EncounterCondition
    {
        public CharacterPickerControl Unit;
        public float mHealthPercent;
        public Operator Operator;

        public override bool IsConditionMet(EncounterModel model)
        {
            Debug.Assert(Unit != null);
            if (Unit == null)
                return false;

            Debug.Assert(model.Players.ContainsKey(Unit.CharacterId));

            float healthPercent = 0;

            if (model.Players.ContainsKey(Unit.CharacterId))
            {
                healthPercent = model.Players[Unit.CharacterId].GetComponent<ResourcesControl>().Resources[ResourceType.Health].Ratio;
            }

            return Condition.Compare(healthPercent, mHealthPercent, Operator);
        }
    }
}
