using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEngine.UI;

namespace Screens.Roster
{
    class InventoryEquipmentSelector : MonoBehaviour
    {
        public Text EquipmentName;
        public Text EquipmentCount;
        public KeyValuePair<int, int> EquipmentIndex;

        private Button Button;

        void Awake()
        {
            Button = GetComponent<Button>();
        }
         
        public void DisplayEquipment(Action _onClick, string name, string count, KeyValuePair<int,int> equipmentIndex)
        {
            EquipmentName.text = name;
            EquipmentCount.text = count;
            EquipmentIndex = equipmentIndex;
            Button.onClick.RemoveAllListeners();
            Button.onClick.AddListener(() => { _onClick(); });
        }
    }
}
