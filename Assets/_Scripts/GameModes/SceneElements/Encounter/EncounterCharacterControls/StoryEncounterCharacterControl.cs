﻿using MAGE.GameSystems;
using MAGE.GameSystems.Characters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace MAGE.GameModes.SceneElements.Encounters
{
    class StoryEncounterCharacterControl : CharacterControl
    {
        public CharacterPicker CharacterPicker;
        public int LevelOverride = -1;

        protected override void Cleanup()
        {
            // empty
        }

        public override int GetCharacterId()
        {
            return (int)CharacterPicker.GetCharacterId();
        }

        protected override void Init()
        {
            // empty
        }
    }
}