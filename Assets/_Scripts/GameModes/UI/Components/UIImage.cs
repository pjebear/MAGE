using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

class UIImage : UIInteractibleComponent
{
    public Image Image;

    class DataProvider : IDataProvider
    {
        public Optional<ImageAssetId> AssetId;

        public DataProvider(Optional<ImageAssetId> assetId)
        {
            AssetId = assetId;
        }

        public override string ToString()
        {
            return "UIImageDP";
        }
    }

    public override void Publish(IDataProvider dataProvider)
    {
        // TODO:
    }
}
