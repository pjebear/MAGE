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
        private void Start()
        {
            Appearance appearance = null;
            CharacterPickerControl characterPickerControl = GetComponent<CharacterPickerControl>();
            int characterId = characterPickerControl.GetCharacterId();
            
            appearance = LevelManagementService.Get().GetAppearance(characterId);

            if (GetComponent<ActorOutfitter>() != null)
            {
                GetComponent<ActorOutfitter>().UpdateAppearance(appearance);
            }
        }
    }
}
