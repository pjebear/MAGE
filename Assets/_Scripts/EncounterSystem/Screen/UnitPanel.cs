using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEngine.UI;

namespace EncounterSystem.Screen
{
    using Character;
    using Common.AttributeEnums;
    using Screens.Payloads;

    public class UnitPanel : MonoBehaviour
    {
        public Text Name;
        public Text Profession;
        public Text Level;
        public Text Health;
        public Text Endurance;
        public Text Mana;
        public Text Clock;
        public Image Image;
        public int PanelSide;
        private StatusEffectIconManager mStatusEffectIconManager = null;

        private void Awake()
        {
            Name = transform.Find("Name").GetComponent<Text>();
            Profession = transform.Find("Profession").GetComponent<Text>();
            Level = transform.Find("Level").GetComponent<Text>();
            Health = transform.Find("Health").GetComponent<Text>();
            Endurance = transform.Find("Endurance").GetComponent<Text>();
            Mana = transform.Find("Mana").GetComponent<Text>();
            Clock = transform.Find("Clock").GetComponent<Text>();
            Image = transform.Find("Image").GetComponent<Image>();
            //Debug.Assert(Name != null, "UnitPanel: Name not hooked up!");
            //Debug.Assert(Profession != null, "UnitPanel: Profession not hooked up!");
            //Debug.Assert(Level != null, "UnitPanel: Level not hooked up!");
            //Debug.Assert(Health != null, "UnitPanel: Health not hooked up!");
            //Debug.Assert(Endurance != null, "UnitPanel: Endurance not hooked up!");
            //Debug.Assert(Mana != null, "UnitPanel: Mana not hooked up!");
            //Debug.Assert(Clock != null, "UnitPanel: Clock not hooked up!");
        }

        public void Initialize()
        {
            mStatusEffectIconManager = GetComponent<StatusEffectIconManager>();
            mStatusEffectIconManager.Initialize(GetComponent<RectTransform>().rect.width / mStatusEffectIconManager.NumIconsPerRow);
        }

        public void DisplayUnit(UnitPanelPayload unitPayload)
        {
            gameObject.SetActive(true);
            Name.text = unitPayload.UnitName;
            Profession.text = unitPayload.ProfessionName;
            Level.text = "Lv." + unitPayload.CharacterLevel;

            Health.text = "Hp: " + ((int)unitPayload.CurrentHealth).ToString() + " / " + ((int)unitPayload.MaxHealth).ToString();
            Mana.text = "Mp: " + ((int)unitPayload.CurrentMana).ToString() + " / " + ((int)unitPayload.MaxMana).ToString();
            Endurance.text = "End: " + ((int)unitPayload.CurrentEndurance).ToString() + " / " + ((int)unitPayload.MaxEndurance).ToString();
            Image.sprite = Resources.Load<Sprite>(unitPayload.ImageAssetPath);

            if (unitPayload.CurrentClock != -1)
            {
                Clock.text = "Clk: " + unitPayload.CurrentClock.ToString() + " / " + "100";
            }
            else
            {
                Clock.text = "";
            }
            mStatusEffectIconManager.SetIcons(unitPayload.StatusPayloads);
        }

        public void Hide()
        {
            mStatusEffectIconManager.Hide();
            gameObject.SetActive(false);
        }
    }
}
