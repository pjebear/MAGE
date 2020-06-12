using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

class ActorActions : UIContainer
{
    public enum ComponentId
    {
        ActionList
    }

    public class DataProvider : IDataProvider
    {
        public UIList.DataProvider ButtonListDP;
    }

    public UIList ActionList;

    public override void Publish(IDataProvider dataProvider)
    {
        DataProvider dp = (DataProvider)dataProvider;

        ActionList.Publish(dp.ButtonListDP);
    }

    protected override void InitChildren()
    {
        ActionList.Init((int)ComponentId.ActionList, this);
    }
}