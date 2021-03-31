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
        private void Start()
        {
            Appearance appearance = GetComponent<CharacterPickerControl>().CharacterPicker.GetAppearance();

            if (GetComponent<ActorOutfitter>() != null)
            {
                GetComponent<ActorOutfitter>().UpdateAppearance(appearance);
            }

            gameObject.name = GetComponent<CharacterPickerControl>().CharacterPicker.GetActorName();
        }
    }
}
