using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

class ActorActions : UIContainer
{
    public enum ComponentId
    {
        ButtonList
    }

    public class DataProvider : IDataProvider
    {
        public UIButtonList.DataProvider ButtonListDP;

        public DataProvider(UIButtonList.DataProvider buttonListDP)
        {
            ButtonListDP = buttonListDP;
        }

        public override string ToString()
        {
            return ButtonListDP.ToString();
        }
    }

    public UIButtonList ButtonList;

    public override void Publish(IDataProvider dataProvider)
    {
        DataProvider dp = (DataProvider)dataProvider;

        ButtonList.Publish(dp.ButtonListDP);
    }

    protected override void InitComponents()
    {
        ButtonList.Init((int)ComponentId.ButtonList, this);
    }

    protected override void InitSelf()
    {
        mId = (int)UIContainerId.ActorActionsView;
        mContainerName = "ActorActions";
    }

    protected override IUIInteractionInfo ModifyInteractionInfo(IUIInteractionInfo interactionInfo)
    {
        return interactionInfo;
    }
}
