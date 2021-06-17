using MAGE.UI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.EventSystems;

namespace MAGE.UI.Views
{
    class IDataProvider
    {
        public static IDataProvider Empty { get { return new IDataProvider(); } }
        protected IDataProvider() { }
    }

    abstract class UIComponentBase
        : MonoBehaviour
        , IPointerClickHandler
        , IPointerEnterHandler
        , IPointerExitHandler
    {
        public bool IsClickable = false;
        public bool IsHoverable = false;

        public static int INVALID_ID = -1;
        public int mId = INVALID_ID;

        protected UIContainer mContainer = null;

        // ----------------------------------------------------------------------------------------------- 
        public virtual void Init(int id, UIContainer container)
        {
            mId = id;
            mContainer = container;
        }

        // ----------------------------------------------------------------------------------------------- 
        public abstract void Publish(IDataProvider dataProvider);

        // ----------------------------------------------------------------------------------------------- 
        protected virtual void HandlePointerClick()
        {
            UIManager.Instance.PlaySFX(SFXId.Confirm);   

            NotifyInteraction(mId, UIInteractionType.Click);
        }

        // ----------------------------------------------------------------------------------------------- 
        public void OnPointerClick(PointerEventData eventData)
        {
            if (IsClickable)
            {
                HandlePointerClick();
            }
        }

        // ----------------------------------------------------------------------------------------------- 
        protected virtual void HandlePointerEnter()
        {
            UIManager.Instance.PlaySFX(SFXId.MenuHover);

            NotifyInteraction(mId, UIInteractionType.MouseOver);
        }

        // ----------------------------------------------------------------------------------------------- 
        public void OnPointerEnter(PointerEventData eventData)
        {
            if (IsHoverable)
            {
                HandlePointerEnter();
            }
        }

        // ----------------------------------------------------------------------------------------------- 
        protected virtual void HandlePointerExit()
        {
            NotifyInteraction(mId, UIInteractionType.MouseExit);
        }

        // ----------------------------------------------------------------------------------------------- 
        public void OnPointerExit(PointerEventData eventData)
        {
            if (IsHoverable)
            {
                HandlePointerExit();
            }
        }

        // ----------------------------------------------------------------------------------------------- 
        protected virtual void NotifyInteraction(int componentId, UIInteractionType interactionType)
        {
            if (mContainer != null)
            {
                mContainer.HandleInteraction(new UIInteractionInfo(componentId, interactionType));
            }
        }
    }


}