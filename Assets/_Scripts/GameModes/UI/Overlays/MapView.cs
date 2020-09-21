using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace MAGE.UI.Views
{
    class MapView : UIContainer
    {
        public enum ComponentId
        {
            ForestLevelBtn,
            DemoLevelBtn,

            NUM
        }

        public class DataProvider : IDataProvider
        {
            public bool IsForestCurrentLocation;
        }

        public IconTextBase ForestLevelBtn;
        public IconTextBase DemoLevelBtn;
        public UIImage CurrentLocationMarker;

        public override void Publish(IDataProvider dataProvider)
        {
            DataProvider dp = dataProvider as DataProvider;
            if (dp.IsForestCurrentLocation)
            {
                CurrentLocationMarker.transform.position = ForestLevelBtn.transform.position;
            }
            else
            {
                CurrentLocationMarker.transform.position = DemoLevelBtn.transform.position;
            }
        }

        protected override void InitChildren()
        {
            ForestLevelBtn.Init((int)ComponentId.ForestLevelBtn, this);
            DemoLevelBtn.Init((int)ComponentId.DemoLevelBtn, this);
        }
    }

}

