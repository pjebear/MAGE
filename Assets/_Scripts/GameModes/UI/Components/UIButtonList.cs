using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;


class UIButtonList : UIContainer
{
    public List<UIButton> ButtonList;

    public class DataProvider : IDataProvider
    {
        public List<UIButton.DataProvider> ButtonsToDisplay;

        public DataProvider(List<UIButton.DataProvider> buttonsToDisplay)
        {
            ButtonsToDisplay = buttonsToDisplay;
        }

        public override string ToString()
        {
            string toString = "";
            int i = 0;
            foreach(UIButton.DataProvider buttonDp in ButtonsToDisplay)
            {
                toString += string.Format("{0}:{1}|", i.ToString(), buttonDp.ToString());
            }

            return toString;
        }
    }

    public class ButtonListInteractionInfo : IUIInteractionInfo
    {
        public int ButtonIdx;

        public ButtonListInteractionInfo(int componentId, IUIInteractionInfo buttonInteractionInfo) 
            : base(componentId, buttonInteractionInfo.InteractionType)
        {
            ButtonIdx = buttonInteractionInfo.ComponentId;
        }
    }

    public override void Publish(IDataProvider dataProvider)
    {
        DataProvider dp = (DataProvider)dataProvider;

        for (int i = 0; i < ButtonList.Count; ++i)
        {
            if (i < dp.ButtonsToDisplay.Count)
            {
                ButtonList[i].gameObject.SetActive(true);

                ButtonList[i].Publish(dp.ButtonsToDisplay[i]);
            }
            else
            {
                ButtonList[i].gameObject.SetActive(false);
            }
        }
    }

    protected override void InitComponents()
    {
        for (int i = 0; i < ButtonList.Count; ++i)
        {
            ButtonList[i].Init(i, this);
        }
    }

    protected override void InitSelf()
    {
        // empty
        if (GetComponentInParent<UIContainer>() == null)
        {
            Debug.Assert(mId != INVALID_ID);
        }
    }

    protected override IUIInteractionInfo ModifyInteractionInfo(IUIInteractionInfo interactionInfo)
    {
        return new ButtonListInteractionInfo(mId, interactionInfo);
    }
}

