using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Screens.Roster
{
    public class UnitSelector : MonoBehaviour
    {

        public static float SelectorWidth = 90f;
        public static float SelectorHeight = 110f;

        private Image mUnitImage;
        private Action _OnHover;
        private Action _OnHoverLeave;
        private Action _OnClick;

        private void Awake()
        {
            mUnitImage = GetComponent<Image>();
            Debug.Assert(mUnitImage != null);
        }

        public void OnMouseEnter()
        {
            _OnHover();
        }

        public void OnMouseExit()
        {
            _OnHoverLeave();
        }

        public void Initialize(Sprite imageAsset, Action onHover, Action onHoverLeave, Action onClick)
        {
            gameObject.SetActive(true);
            mUnitImage.sprite = imageAsset;
            _OnHover = onHover;
            _OnHoverLeave = onHoverLeave;
            _OnClick = onClick;
            GetComponent<Button>().onClick.AddListener(() => { _OnClick(); });
        }


    }
}

