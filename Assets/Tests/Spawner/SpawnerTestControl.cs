using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;
using MAGE.GameSystems.Characters;
using MAGE.GameSystems;
using MAGE.GameSystems.World.Internal;

namespace MAGE.GameModes.Tests
{
    class SpawnerTestControl : MonoBehaviour
    {
        public Button RefreshSpawnerBtn;
        public SceneElements.ActorSpawner Spawner;

        private int mActorId = -1;

        private List<List<EquippableId>> OutfitRotations = new List<List<EquippableId>>()
        {
            new List<EquippableId>() { EquippableId.LeatherArmor_0, EquippableId.Mace_0, EquippableId.Mace_0, EquippableId.INVALID}
            , new List<EquippableId>() { EquippableId.ChainArmor_0, EquippableId.Bow_0, EquippableId.INVALID, EquippableId.INVALID}
            , new List<EquippableId>() { EquippableId.ChainArmor_0, EquippableId.Staff_0, EquippableId.INVALID, EquippableId.INVALID}
        };
        private int mRotationIdx = 0;

        private void Awake()
        {
            CharacterService.Register(new MAGE.GameSystems.Characters.Internal.CharacterServiceImpl());
            DBService.Register(new MAGE.DB.Internal.DBServiceImpl());
            CharacterService.Get().Init();
            DBService.Get().Init();
            //CharacterDBLoader.LoadDB();

            CharacterCreateParams createParams = new CharacterCreateParams();

            createParams.characterType = CharacterType.Create;
            createParams.characterClass = CharacterClass.MultiSpecialization;
            createParams.id = -1;
            createParams.name = "TEST";
            createParams.currentSpecialization = SpecializationType.Footman;
            createParams.currentEquipment = OutfitRotations[0];

            mActorId = CharacterService.Get().CreateCharacter(createParams);
            //Spawner.CharacterType = CharacterType.Create;
            //Spawner.CreateCharacterId = mActorId;

            RefreshSpawnerBtn.onClick.AddListener(() =>
            {
                MAGE.GameSystems.CharacterService.Get().ChangeSpecialization(mActorId, SpecializationType.Archer);
            });
        }

    }
}



