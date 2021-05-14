using MAGE.GameSystems;
using MAGE.GameSystems.Appearances;
using MAGE.GameSystems.Characters;
using MAGE.Utility.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace MAGE.GameModes.SceneElements
{
    class CharacterPickerControl : MonoBehaviour
    {
        private int mCharacterId = -1;
        public int CharacterId
        {
            get
            {
                return mCharacterId;
            }
            set
            {
                mCharacterId = value;
                BroadcastMessage("OnCharacterChanged", null, SendMessageOptions.DontRequireReceiver);
            }
        }
        public Appearance Appearance
        {
            get
            {
                return AppearanceUtil.GetAppearance(mCharacterId);
            }
        }
    }
}
