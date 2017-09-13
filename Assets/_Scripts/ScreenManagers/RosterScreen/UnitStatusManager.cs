using Common.AttributeEnums;
using Common.UnitTypes;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using WorldSystem.Character;

namespace Screens.Roster
{
    class UnitStatusManager : MonoBehaviour
    {
        public Text Jump;
        public Text Move;
        public Text Speed;
        public Text Physical;
        public Text Magic;
        public Text MagicCrit;
        public Text PhysicalCrit;
        public Text CastSpeed;
        public Text EnduranceRecovery;
        public Text FrontalBlock;
        public Text PeripheralBlock;
        public Text FrontalParry;
        public Text PeripheralParry;
        public Text Dodge;
        public Text PhysicalResistance;
        public Text MagicalResistance;
        public Text PhysicalCritResistance;
        public Text MagicalCritResistance;
        Action OnBack;

        public void Initialize(Action _onBack)
        {
            OnBack = _onBack;
        }

        // Use this for initialization
        void Start()
        {

        }

        // Update is called once per frame
        void Update()
        {
            if (Input.GetMouseButtonUp(1))
            {
                OnBack();
            }
        }

        public void DisplayUnit(CharacterBase character)
        {
            Jump.text = "Jump: " + character.Attributes[AttributeType.Stat][(int)TertiaryStat.Jump];
            Move.text = "Move: " + character.Attributes[AttributeType.Stat][(int)TertiaryStat.Movement];
            Speed.text = "Speed: " + character.Attributes[AttributeType.Stat][(int)TertiaryStat.Speed];
            Physical.text = "Physical %: " + character.Attributes[AttributeType.Stat][(int)TertiaryStat.PhysicalMultiplier];
            Magic.text = "Magical %: " + character.Attributes[AttributeType.Stat][(int)TertiaryStat.MagicalMultiplier];
            PhysicalCrit.text = "Phys Crit %: " + character.Attributes[AttributeType.Stat][(int)TertiaryStat.PhysicalCritChance];
            MagicCrit.text = "Mag Crit %: " + character.Attributes[AttributeType.Stat][(int)TertiaryStat.MagicalCritChance];
            CastSpeed.text = "Cast Speed: " + character.Attributes[AttributeType.Stat][(int)TertiaryStat.CastSpeed];
            EnduranceRecovery.text = "End Recovery: " + character.Attributes[AttributeType.Stat][(int)TertiaryStat.EnduranceRecovery];
            FrontalBlock.text = "Frontal Block %: " + character.Attributes[AttributeType.Stat][(int)TertiaryStat.FrontalBlock];
            PeripheralBlock.text = "Peripheral Block %: " + character.Attributes[AttributeType.Stat][(int)TertiaryStat.PeriferalBlock];
            FrontalParry.text = "Frontal Parry %: " + character.Attributes[AttributeType.Stat][(int)TertiaryStat.FrontalParry];
            PeripheralParry.text = "Peripheral Parry %: " + character.Attributes[AttributeType.Stat][(int)TertiaryStat.PeriferalParry];
            Dodge.text = "Dodge %: " + character.Attributes[AttributeType.Stat][(int)TertiaryStat.Dodge];
            MagicalCritResistance.text = "Mag Crit Resist %: " + -character.Attributes[AttributeType.Stat][(int)TertiaryStat.MagicalCritSusceptibility];
            PhysicalCritResistance.text = "Phys Crit Resist %: " + -character.Attributes[AttributeType.Stat][(int)TertiaryStat.PhysicalCritSusceptibility];
            PhysicalResistance.text = "Phys Resist %: " + character.Attributes[AttributeType.Stat][(int)TertiaryStat.PhysicalResistance];
            MagicalResistance.text = "Mag Resist %: " + character.Attributes[AttributeType.Stat][(int)TertiaryStat.MagicalResistance];

        }
    }
}

