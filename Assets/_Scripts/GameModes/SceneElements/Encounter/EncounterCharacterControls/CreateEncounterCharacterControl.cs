using MAGE.GameSystems;
using MAGE.GameSystems.Characters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace MAGE.GameModes.SceneElements.Encounter
{
    class CreateEncounterCharacterControl : EncounterCharacterControl
    {
        public string NameOverride = "";
        public SpecializationType SpecializationType;

        public EquippableId LeftHeld = EquippableId.INVALID;
        public EquippableId RightHeld = EquippableId.INVALID;
        public EquippableId Worn = EquippableId.INVALID;
        public EquippableId Accessory = EquippableId.INVALID;

        public PortraitSpriteId PortraitOverride = PortraitSpriteId.INVALID;

        private int mCreateCharacterId = -1;

        public override int GetCharacterId()
        {
            return mCreateCharacterId;
        }

        protected override void Init()
        {
            ICharacterService characterService = CharacterService.Get();
            Debug.Assert(characterService != null);
            if (characterService != null)
            {
                CharacterCreateParams characterCreateParams = new CharacterCreateParams();

                // Info
                characterCreateParams.characterType = CharacterType.Temporary;
                characterCreateParams.characterClass = CharacterClass.MonoSpecialization;
                characterCreateParams.currentSpecialization = SpecializationType;
                characterCreateParams.name = NameOverride != "" ? NameOverride : SpecializationType.ToString();

                // Equipment
                characterCreateParams.currentEquipment[(int)Equipment.Slot.LeftHand] = LeftHeld;
                characterCreateParams.currentEquipment[(int)Equipment.Slot.RightHand] = RightHeld;
                characterCreateParams.currentEquipment[(int)Equipment.Slot.Armor] = Worn;
                characterCreateParams.currentEquipment[(int)Equipment.Slot.Accessory] = Accessory;

                // Appearance
                characterCreateParams.appearanceOverrides = new Appearance() { PortraitSpriteId = PortraitOverride };

                mCreateCharacterId = characterService.CreateCharacter(characterCreateParams);
            }
        }

        protected override void Cleanup()
        {
            ICharacterService characterService = CharacterService.Get();
            Debug.Assert(characterService != null);
            if (characterService != null && mCreateCharacterId != -1)
            {
                characterService.DeleteCharacter(mCreateCharacterId);
            }
        }
    }
}
