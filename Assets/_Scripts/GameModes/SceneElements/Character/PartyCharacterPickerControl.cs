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
    [RequireComponent(typeof(CharacterPickerControl))]
    public class PartyCharacterPickerControl : MonoBehaviour
    {
        [SerializeField]
        public int PartyCharacterIdx = -1;

        private void Start()
        {
            if (PartyCharacterIdx != -1)
            {
                GetComponent<CharacterPickerControl>().CharacterId = CharacterUtil.CreateIdToDBId(PartyCharacterIdx);
            }
            else
            {
                Debug.LogWarningFormat("[{0}] PartyCharacterId not set!", gameObject.name);
            }

            
        }
    }
}
