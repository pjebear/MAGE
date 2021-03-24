using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace MAGE.UI.Views
{
    class CursorControl
    {
        public enum CursorType
        {
            Default,

            Combat_Near,
            Combat_Far,

            Interact_Near,
            Interact_Far,

            NUM
        }

        private AssetLoader<Texture2D> mCursorSpriteLoader;

        public CursorControl()
        {
            mCursorSpriteLoader = new AssetLoader<Texture2D>("UI/Cursors");
            mCursorSpriteLoader.LoadAssets();
        }

        public void SetCursorState(CursorType cursorType)
        {
            Texture2D texture = mCursorSpriteLoader.GetAsset(cursorType.ToString());
            Cursor.SetCursor(texture, Vector2.zero, CursorMode.Auto);
        }
    }


}
