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
using MAGE.GameModes.SceneElements;
using MAGE.GameSystems.Appearances;
using MAGE.DB.Internal;
using MAGE.GameSystems.Characters.Internal;
using CharacterInfo = MAGE.GameSystems.Characters.CharacterInfo;

namespace MAGE.GameModes.Tests
{
    class SpawnerTestControl : MonoBehaviour
    {
        public Button RefreshSpawnerBtn;
        public SceneElements.ActorSpawner Spawner;

        public Appearance[] Appearances = new Appearance[4];

        public List<Actor> Actors = new List<Actor>();
        public List<ActorSpawner> Spawners = new List<ActorSpawner>();
        public List<StoryCharacterId> StoryCharacterIds = new List<StoryCharacterId>()
        {
            StoryCharacterId.Rheinhardt,
            StoryCharacterId.Asmund,
            StoryCharacterId.Balgrid,
            StoryCharacterId.Maric
        };

        private void Awake()
        {
            {
                Appearance appearance = new Appearance();
                appearance.BaseOutfitType = ApparelAssetId.Cloth_0;
                appearance.SkinToneType = SkinToneType.Pale;
                appearance.HairType = HairType.MaleLong;
                appearance.FacialHairType = FacialHairType.LongBeard;
                appearance.HairColor = HairColor.Grey;
                appearance.OverrideRightHeldAssetId = ApparelAssetId.Staff_0;

                Appearances[0] = appearance;
            }
            {
                Appearance appearance = new Appearance();
                appearance.BaseOutfitType = ApparelAssetId.Leather_0;
                appearance.SkinToneType = SkinToneType.Tan;
                appearance.HairType = HairType.MaleShort;
                appearance.FacialHairType = FacialHairType.None;
                appearance.HairColor = HairColor.Dark;
                appearance.OverrideLeftHeldAssetId = ApparelAssetId.Axe_0;
                appearance.OverrideRightHeldAssetId = ApparelAssetId.Axe_0;

                Appearances[1] = appearance;
            }
            {
                Appearance appearance = new Appearance();
                appearance.BaseOutfitType = ApparelAssetId.Mail_0;
                appearance.SkinToneType = SkinToneType.Base;
                appearance.HairType = HairType.MaleShort;
                appearance.FacialHairType = FacialHairType.None;
                appearance.HairColor = HairColor.Blonde;
                appearance.OverrideLeftHeldAssetId = ApparelAssetId.Shield_0;
                appearance.OverrideRightHeldAssetId = ApparelAssetId.Sword_0;

                Appearances[2] = appearance;
            }

            {
                Appearance appearance = new Appearance();
                appearance.BaseOutfitType = ApparelAssetId.Plate_0;
                appearance.SkinToneType = SkinToneType.Base;
                appearance.HairType = HairType.MaleShort;
                appearance.FacialHairType = FacialHairType.ShortBeard;
                appearance.HairColor = HairColor.Brunette;
                appearance.OverrideLeftHeldAssetId = ApparelAssetId.TowerShield_0;
                appearance.OverrideRightHeldAssetId = ApparelAssetId.Mace_0;

                Appearances[3] = appearance;
            }

            DBServiceImpl mockDBService = new DBServiceImpl();
            DBService.Register(mockDBService);
            CharacterServiceImpl characterService = new CharacterServiceImpl();
            CharacterService.Register(characterService);
            mockDBService.Init();

            Actors = GameObject.Find("Characters").GetComponentsInChildren<Actor>().ToList();
            Spawners = GameObject.Find("Spawners").GetComponentsInChildren<ActorSpawner>().ToList();

            for (int i = 0; i < 4; ++i)
            {
                mockDBService.WriteAppearance(i, AppearanceUtil.ToDB(Appearances[i]));

                CharacterCreateParams createParams = new CharacterCreateParams();
                createParams.id = i;
                createParams.appearanceOverrides = Appearances[i];

                characterService.CreateCharacter(createParams);
                Actors[i].gameObject.GetComponent<ActorOutfitter>().UpdateAppearance(Appearances[i]);
            }
        }

        private void Start()
        {
            for (int i = 0; i < 4; ++i)
            {
                Actors[i].gameObject.GetComponent<ActorOutfitter>().UpdateAppearance(Appearances[i]);
            }
        }

        int offset = 0;
        float combatChangeTimer = 0;
        bool inCombat = false;
        private void Update()
        {
            combatChangeTimer += Time.deltaTime;
            if (combatChangeTimer > 3f)
            {
                combatChangeTimer = 0;
                inCombat = !inCombat;

                for (int i = 0; i < 4; ++i)
                {
                    Actors[i].GetComponent<ActorOutfitter>().UpdateHeldApparelState(inCombat ? HumanoidActorConstants.HeldApparelState.Held : HumanoidActorConstants.HeldApparelState.Holstered);
                    Actors[i].GetComponent<ActorAnimator>().SetInCombat(inCombat);

                    //Spawners[i].SetInCombat(inCombat);
                    int index = offset + i;
                    index = index % 4;
                    Spawners[i].GetComponent<CharacterPickerControl>().CharacterId = (int)StoryCharacterIds[index];
                    Spawners[i].Refresh();
                    Actors[i].GetComponentInChildren<ActorAnimator>().SetInCombat(inCombat);
                }

                offset++;
            }
        }

    }
}



