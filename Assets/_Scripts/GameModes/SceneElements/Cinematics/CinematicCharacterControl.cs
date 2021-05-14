using MAGE.GameSystems;
using MAGE.GameSystems.Appearances;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace MAGE.GameModes.SceneElements
{
    [RequireComponent(typeof(CharacterPickerControl))]
    class CinematicCharacterControl : MonoBehaviour
    {
        public void OnCharacterChanged()
        {
            if (GetComponent<ActorOutfitter>() != null && GetComponent<CharacterPickerControl>() != null)
            {
                GetComponent<ActorOutfitter>().UpdateAppearance(GetComponent<CharacterPickerControl>().Appearance);
            }
        }
    }
}
