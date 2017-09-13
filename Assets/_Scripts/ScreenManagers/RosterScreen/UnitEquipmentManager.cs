using Common.EquipmentEnums;
using Screens.Payloads;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using WorldSystem.Character;
using WorldSystem.Interface;

namespace Screens.Roster
{
    class UnitEquipmentManager : MonoBehaviour
    {
        public RectTransform ScrollRect;
        public RectTransform ContentTransform;
        public Button EquipButton;
        public Button UnequipButton; 
        public WornEquipmentSelector[] WornEquipmentSelectors;

        public InventoryEquipmentSelector[] InventoryEquipmentSelectors;
        private float mSelectorHeight;
       
        private WornEquipmentSlot mSelectedWornSlot;
        private int mSelectedInventoryIndex;
        private CharacterBase rCharacterBase;

        private System.Action _OnBack;

        private void Awake()
        {
            mSelectedWornSlot = WornEquipmentSlot.Invalid;
            mSelectedInventoryIndex = -1;

            EquipButton.interactable = false;
            EquipButton.onClick.AddListener(_OnEquipPress);
            UnequipButton.onClick.AddListener(_OnUnequipPress);
            UnequipButton.interactable = false;

            InventoryEquipmentSelectors = ContentTransform.GetComponentsInChildren<InventoryEquipmentSelector>();
            mSelectorHeight = InventoryEquipmentSelectors[0].GetComponent<RectTransform>().rect.height;

            for (int i = 0; i < InventoryEquipmentSelectors.Length; ++i)
            {
                InventoryEquipmentSelectors[i].gameObject.SetActive(false);
            }
            ContentTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, ScrollRect.rect.height);
        }

        private void Update()
        {
            if (Input.GetMouseButtonDown(1))
            {
                _OnBack();
            }
        }

        public void Initialize(System.Action _onBack)
        {
            _OnBack = _onBack;
        }

        public void DisplayUnit(CharacterBase character)
        {
            rCharacterBase = character;
            WornEquipmentPayload[] wornPayloads = rCharacterBase.GetWornEquipmentPayloads();
            for (int slot = 0; slot < wornPayloads.Length; slot++)
            {
                bool filledSlot = wornPayloads[slot].ImageAssetPath != "empty";
                WornEquipmentSlot equipmentSlot = (WornEquipmentSlot)slot;
                WornEquipmentSelectors[slot].DisplayEquipment(
                    () => { _SelectWornEquipment(equipmentSlot, filledSlot); },
                    Resources.Load<Sprite>(wornPayloads[slot].ImageAssetPath));
            }
            for (int idx = 0; idx < InventoryEquipmentSelectors.Length; ++idx)
            {
                InventoryEquipmentSelectors[idx].gameObject.SetActive(false);
            }
            mSelectedWornSlot = WornEquipmentSlot.Invalid;
            mSelectedInventoryIndex = -1;
            UnequipButton.interactable = false;
            EquipButton.interactable = false;
        }


        private void _SelectWornEquipment(WornEquipmentSlot slot, bool hasEquipment)
        {
            mSelectedWornSlot = slot;
            UnequipButton.interactable = hasEquipment;
  
            WorldSystemFacade wsFacade = WorldSystemFacade.Instance();
            List<InventoryEquipmentPayload> validEquipment = wsFacade.GetValidEquipmentForSlot(rCharacterBase, slot);
            DisplayInventoryEquipment(validEquipment);
        }

        private void _SelectInventoryEquipment(int inventoryIdx)
        {
            mSelectedInventoryIndex = inventoryIdx;
            if (mSelectedWornSlot != WornEquipmentSlot.Invalid)
            {
                EquipButton.interactable = true;
            }
        }

        private void DisplayInventoryEquipment(List<InventoryEquipmentPayload> payloads)
        {
            Debug.Assert(payloads.Count <= InventoryEquipmentSelectors.Length, "Inventory Query returned " + payloads.Count + " items");

            for (int idx = 0; idx < payloads.Count; ++idx)
            {
                InventoryEquipmentPayload payload = payloads[idx];
                int slot = idx;
                InventoryEquipmentSelectors[idx].DisplayEquipment(() => { _SelectInventoryEquipment(slot); }, payload.EquipmentName, payload.Count.ToString(), payload.EquipmentIndex);
                InventoryEquipmentSelectors[idx].gameObject.SetActive(true);
            }
            for (int idx = payloads.Count; idx < InventoryEquipmentSelectors.Length; ++idx)
            {
                InventoryEquipmentSelectors[idx].gameObject.SetActive(false);
            }
        }

        private void _OnEquipPress()
        {
            Debug.Assert(mSelectedInventoryIndex > -1 && mSelectedWornSlot != WornEquipmentSlot.Invalid);
            WorldSystemFacade wsFacade = WorldSystemFacade.Instance();
            wsFacade.EquipCharacterInSlot(rCharacterBase.CharacterID, InventoryEquipmentSelectors[mSelectedInventoryIndex].EquipmentIndex, mSelectedWornSlot);
            DisplayUnit(rCharacterBase);
        }

        private void _OnUnequipPress()
        {
            Debug.Assert(mSelectedWornSlot != WornEquipmentSlot.Invalid);
            WorldSystemFacade wsFacade = WorldSystemFacade.Instance();
            wsFacade.UnequipCharacterSlot(rCharacterBase.CharacterID, mSelectedWornSlot);
            DisplayUnit(rCharacterBase);
        }
    }

}
