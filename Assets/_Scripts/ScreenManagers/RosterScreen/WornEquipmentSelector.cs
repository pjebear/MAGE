using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEngine.UI;

namespace Screens.Roster
{
    class WornEquipmentSelector : MonoBehaviour
    {
        private Button Button;
        private Image Image;
        void Awake()
        {
            Button = GetComponent<Button>();
            Image = GetComponent<Image>();
        }
         
        public void DisplayEquipment(Action _onClick, Sprite sprite)
        {
            Button.onClick.RemoveAllListeners();
            Button.onClick.AddListener(() => { _onClick(); });
            Image.sprite = sprite;
        }
    }
}
