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
        private bool mIsLoaded = false;

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
                if (mIsLoaded)
                {
                    BroadcastMessage("OnCharacterChanged", null, SendMessageOptions.DontRequireReceiver);
                }
            }
        }

        private void Start()
        {
            mIsLoaded = true;
            if (mCharacterId != -1)
            {
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
