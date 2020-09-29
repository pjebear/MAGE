using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

namespace MAGE.UI.Views
{
    class EncounterCharacterInfoViewRight : UIContainer
    {
        private string TAG = "EncounterCharacterInfoViewRight";

        public CharacterInspector CharacterInspector;

        public override void Publish(IDataProvider dataProvider)
        {
            CharacterInspector.Publish(dataProvider);
        }

        protected override void InitChildren()
        {
            // empty
        }
    }
}


