﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.EventSystems;

class IUIInteractionInfo
{
    public int ComponentId;
    public UIInteractionType InteractionType;

    public IUIInteractionInfo(int componentId, UIInteractionType interactionType)
    {
        ComponentId = componentId;
        InteractionType = interactionType;
    }
}

abstract class UIInteractibleComponent : UIComponentBase
    , IPointerClickHandler
    , IPointerEnterHandler
    , IPointerExitHandler
{
    public bool IsClickable = true;
    public bool IsHoverable = true;

    protected virtual void HandlePointerClick()
    {
        NotifyInteraction(mId, UIInteractionType.Click);
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (IsClickable)
        {
            HandlePointerClick();
        }
    }

    protected virtual void HandlePointerEnter()
    {
        NotifyInteraction(mId, UIInteractionType.MouseOver);
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (IsHoverable)
        {
            HandlePointerEnter();
        }
    }

    protected virtual void HandlePointerExit()
    {
        NotifyInteraction(mId, UIInteractionType.MouseExit);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (IsHoverable)
        {
            HandlePointerExit();
        }
    }

    protected virtual void NotifyInteraction(int componentId, UIInteractionType interactionType)
    {
        mContainer.HandleInteraction(new IUIInteractionInfo(componentId, interactionType));   
    }
}
