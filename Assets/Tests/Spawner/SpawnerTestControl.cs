using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;
using MAGE.GameServices.Character;
using MAGE.GameServices;
using MAGE.GameServices.World.Internal;

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
            CharacterService.Register(new MAGE.GameServices.Character.Internal.CharacterServiceImpl());
            DBService.Register(new MAGE.DB.Internal.DBServiceImpl());
            CharacterService.Get().Init();
            DBService.Get().Init();
            //CharacterDBLoader.LoadDB();

            mActorId = CharacterService.Get().CreateCharacter(new CharacterCreateParams("TEST", SpecializationType.Footman, OutfitRotations[0]));
            Spawner.CharacterType = CharacterType.Create;
            Spawner.CreateCharacterId = mActorId;

            RefreshSpawnerBtn.onClick.AddListener(() =>
            {
                MAGE.GameServices.CharacterService.Get().ChangeSpecialization(mActorId, SpecializationType.Archer);
            });
        }

    }
}



