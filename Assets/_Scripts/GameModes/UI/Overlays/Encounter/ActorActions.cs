using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace MAGE.UI.Views
{
    class ActorActions : UIContainer
    {
        public enum ComponentId
        {
            ActionList
        }

        public class DataProvider : IDataProvider
        {
            public UIGrid.DataProvider ButtonListDP;
        }

        public UIGrid ActionList;

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
}
