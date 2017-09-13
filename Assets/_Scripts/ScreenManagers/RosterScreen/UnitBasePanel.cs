using Screens.Payloads;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Screens.Roster
{    
    class UnitBasePanel : MonoBehaviour
    {
        public Text Name;
        public Text ProfessionName;
        public Image ProfileImage;
        public Image AllignmentImage;
        public Text Allignment1;
        public Text Allignment2;
        public Text Allignment3;
        public Text Level;
        public Text Experience;
        public Text ProfessionLevel;
        public Text ProfessionExperience;
        public Text Health;
        public Text Mana;
        public Text Endurance;
        public Text Might;
        public Text Magic;
        public Text Finese;
        public Text Fortitude;
        public Text Attunement;

        // Use this for initialization
        void Start()
        {

        }

        // Update is called once per frame
        void Update()
        {

        }

        public void DisplayUnit(UnitPanelPayload unitPayload)
        {
            Name.text = unitPayload.UnitName;
            ProfessionName.text = unitPayload.ProfessionName;
            ProfileImage.sprite = Resources.Load<Sprite>(unitPayload.ImageAssetPath);
            AllignmentImage.sprite = Resources.Load<Sprite>(unitPayload.AllignmentAssetPath);
            Allignment1.text = unitPayload.MainAllignment;
            Allignment2.text = unitPayload.SecondaryAllignment;
            Allignment3.text = unitPayload.TertiaryAllignment;
            Level.text = "Lv " + unitPayload.CharacterLevel.ToString() ;
            ProfessionLevel.text = "Plv " + unitPayload.ProfessionLevel;
            Experience.text = "Exp " + unitPayload.Experience + "/" + unitPayload.ExperienceMax;
            ProfessionExperience.text = "Pxp " + unitPayload.ProfessionExperience + "/" + unitPayload.ProfessionExperienceMax;
            Health.text = "Hp: " + unitPayload.MaxHealth;
            Mana.text = "Mp: " + unitPayload.MaxMana;
            Endurance.text = "End: " + unitPayload.MaxEndurance;
            Might.text = "Might: " + unitPayload.Might.ToString();
            Finese.text = "Finese: " + unitPayload.Finese.ToString();
            Magic.text = "Magic: " + unitPayload.Magic.ToString();
            Fortitude.text = "Foritude: " + unitPayload.Fortitude.ToString();
            Attunement.text = "Attunement: " + unitPayload.Attunement.ToString();
        }
    }

}
