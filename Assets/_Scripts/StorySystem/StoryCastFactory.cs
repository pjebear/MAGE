using Common.AttributeEnums;
using Common.AttributeTypes;
using Common.CharacterEnums;
using Common.CommonUtil.AttributeUtil;
using Common.EquipmentEnums;
using Common.EquipmentTypes;
using Common.ProfessionEnums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using WorldSystem.Character;

namespace StorySystem.StoryCast
{
    enum StoryCharacterId
    {
        Invalid = -1,
        Rheinhardt = 1000, 
        Asmund,             
        Thanos,
        Ingrid,
        Mattias,
        Magnus
    }

    static class StoryCastFactory
    {
        public static CharacterBase CheckoutStoryCharacter(StoryCharacterId id, int atLevel, UnitGroup controlledBy)
        {
            CharacterBase storyCharacter = null;
            float might = 0, finese = 0, magic = 0, attunement = 0, fortitude = 0;
            AllignmentType allignment = AllignmentType.Unalligned;
            ProfessionType baseProfession = ProfessionType.NONE; // need a system for base professions
            HeldEquipment leftHand = null, rightHand = null;
            ArmorBase armor = null;
            CharacterGender gender = CharacterGender.Male;
            switch (id)
            {
                case (StoryCharacterId.Rheinhardt):
                    might = AttributeConstants.DefaultMight + 10f;
                    finese = AttributeConstants.DefaultFinese + 2f;
                    magic = AttributeConstants.DefaultMagic + 4f;

                    fortitude = AttributeConstants.DefaultFortitude + 10f;
                    attunement = AttributeConstants.DefaultAttunement + 10f;

                    allignment = AllignmentType.Light;
                    baseProfession = ProfessionType.Berserker;
                    leftHand = new WeaponBase(WeaponType.BastardSword);
                    armor = new ArmorBase(ArmorType.Leather);
                    break;

                case (StoryCharacterId.Asmund):
                    might = AttributeConstants.DefaultMight - 2f;
                    finese = AttributeConstants.DefaultFinese - 2f;
                    magic = AttributeConstants.DefaultMagic + 10f;

                    fortitude = AttributeConstants.DefaultFortitude;
                    attunement = AttributeConstants.DefaultAttunement + 15f;

                    allignment = AllignmentType.Earth;
                    baseProfession = ProfessionType.Monk;
                    leftHand = new WeaponBase(WeaponType.Staff);
                    armor = new ArmorBase(ArmorType.Cloth);
                    break;

                case (StoryCharacterId.Ingrid):
                    might = AttributeConstants.DefaultMight - 4f;
                    finese = AttributeConstants.DefaultFinese - 2f;
                    magic = AttributeConstants.DefaultMagic + 5f;

                    fortitude = AttributeConstants.DefaultFortitude;
                    attunement = AttributeConstants.DefaultAttunement + 10f;

                    allignment = AllignmentType.Dark;
                    baseProfession = ProfessionType.Adept;
                    leftHand = new WeaponBase(WeaponType.MageSource);
                    armor = new ArmorBase(ArmorType.Cloth);

                    gender = CharacterGender.Female;
                    break;

                case (StoryCharacterId.Thanos):
                    might = AttributeConstants.DefaultMight + 3f;
                    finese = AttributeConstants.DefaultFinese;
                    magic = AttributeConstants.DefaultMagic;

                    fortitude = AttributeConstants.DefaultFortitude + 5f;
                    attunement = AttributeConstants.DefaultAttunement - 5f;

                    allignment = AllignmentType.Fire;
                    baseProfession = ProfessionType.Footman;
                    leftHand = new WeaponBase(WeaponType.Sword);
                    rightHand = new ShieldBase(ShieldType.Base);
                    armor = new ArmorBase(ArmorType.Chain);
                    break;

                case (StoryCharacterId.Mattias):
                    might = AttributeConstants.DefaultMight + 10f;
                    finese = AttributeConstants.DefaultFinese + 4f;
                    magic = AttributeConstants.DefaultMagic - 5f;

                    fortitude = AttributeConstants.DefaultFortitude + 10f;
                    attunement = AttributeConstants.DefaultAttunement;

                    allignment = AllignmentType.Light;
                    baseProfession = ProfessionType.ShieldWall;
                    leftHand = new ShieldBase(ShieldType.Tower);
                    rightHand = new WeaponBase(WeaponType.Mace);
                    armor = new ArmorBase(ArmorType.Plate);
                    break;

                case (StoryCharacterId.Magnus):
                    might = AttributeConstants.DefaultMight + 15f;
                    finese = AttributeConstants.DefaultFinese + 10f;
                    magic = AttributeConstants.DefaultMagic - 5f;

                    fortitude = AttributeConstants.DefaultFortitude + 20f;
                    attunement = AttributeConstants.DefaultAttunement;

                    allignment = AllignmentType.Sky;
                    baseProfession = ProfessionType.Berserker;
                    leftHand = new WeaponBase(WeaponType.Maul);
                    armor = new ArmorBase(ArmorType.Leather);
                    break;

                default:
                    UnityEngine.Debug.LogError("CheckoutStoryCharacter() No blue print exists for " + id.ToString());
                    break;
            }

            storyCharacter = new CharacterBase(controlledBy, (int)id, id.ToString(), gender, baseProfession, PopulateCharacterAttributes(might, finese, magic, attunement, fortitude, allignment), atLevel);
            if (leftHand != null)
            {
                storyCharacter.EquipHeld(0, leftHand);
            }
            if (rightHand != null)
            {
                storyCharacter.EquipHeld(1, rightHand);
            }
            storyCharacter.EquipWorn(armor);

            

            return storyCharacter;
        }

        private static AttributeContainer PopulateCharacterAttributes(
            float might, float finese, float magic, float attunement, float fortitude, AllignmentType primaryAllignment)
        {
            Dictionary<AttributeType, float[]> attributes = new Dictionary<AttributeType, float[]>();

            float[] stats = new float[(int)CharacterStats.NUM];
            //primary
            stats[(int)PrimaryStat.Might] = might;
            stats[(int)PrimaryStat.Finese] = finese;
            stats[(int)PrimaryStat.Magic] = magic;

            //secondary
            stats[(int)SecondaryStat.Attunement] = attunement;
            stats[(int)SecondaryStat.Fortitude] = fortitude;

            //tertiary
            stats[(int)TertiaryStat.Speed] = AttributeConstants.DefaultSpeed;
            stats[(int)TertiaryStat.Movement] = AttributeConstants.DefaultMovement;
            stats[(int)TertiaryStat.Jump] = AttributeConstants.DefaultJump;
            stats[(int)TertiaryStat.EnduranceRecovery] = AttributeConstants.DefaultEnduranceRecovery;
            attributes.Add(AttributeType.Stat, stats);

            float[] resources = new float[(int)Resource.NUM];
            AttributeUtil.CalculateResourcesFromStats(stats, resources, false);
            attributes.Add(AttributeType.Resource, resources);

            float[] allignments = new float[(int)AllignmentType.NUM];
            AllignmentType[] subAllignments = AttributeUtil.GetSubAllignments(primaryAllignment);
            allignments[(int)primaryAllignment] = 1f;
            allignments[(int)subAllignments[0]] = .5f;
            allignments[(int)subAllignments[0]] = .5f;

            attributes.Add(AttributeType.Allignment, allignments);
            attributes.Add(AttributeType.Status, new float[(int)StatusType.NUM]);

            return new AttributeContainer(attributes);
        }
       
    }
}
