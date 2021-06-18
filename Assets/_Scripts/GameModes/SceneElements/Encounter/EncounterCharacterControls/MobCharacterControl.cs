using MAGE.GameSystems;
using MAGE.GameSystems.Characters;
using MAGE.GameSystems.Mobs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace MAGE.GameModes.SceneElements.Encounters
{
    class MobCharacterControl : CreateCharacterControlBase
    {
        public MobId MobId = MobId.INVALID;
        public int OverrideLevel = -1;
        public int LevelDifficultyModifier = 0;

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
                Debug.Assert(MobId != MobId.INVALID);
                if (MobId == MobId.INVALID) MobId = MobId.DEMO_Bandit;

                int levelId = 1;
                if (OverrideLevel != -1)
                {
                    levelId = OverrideLevel;
                }
                else
                {
                    levelId = WorldService.Get().GetAverageLevelOfParty();
                }

                levelId += LevelDifficultyModifier;

                mCreateCharacterId = characterService.CreateMob(MobId, levelId);
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
