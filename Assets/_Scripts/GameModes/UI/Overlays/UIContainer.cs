using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;



abstract class UIContainer : UIComponentBase
{
    protected string mContainerName = "Someone forgot to write a name!";
    private void Awake()
    {
        InitSelf();
        InitComponents();
    }

    protected abstract void InitSelf();
    protected abstract void InitComponents();
    public string Name() { return mContainerName; }

    protected abstract IUIInteractionInfo ModifyInteractionInfo(IUIInteractionInfo interactionInfo);

    public void HandleInteraction(IUIInteractionInfo interactionInfo)
    {
        IUIInteractionInfo modifiedInfo = ModifyInteractionInfo(interactionInfo);

        if (mContainer != null)
        {
            mContainer.HandleInteraction(modifiedInfo);
        }
        else
        {
            UIManager.Instance.ComponentInteracted(mId, modifiedInfo);
        }
    }
}

