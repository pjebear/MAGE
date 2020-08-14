using MAGE.GameModes.SceneElements;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace MAGE.Tests
{
    class PropTestController : MonoBehaviour
        , IInputHandler
    {
        PropBase mHoveredProp = null;
        PropBase mInteractingWith = null;

        void Start()
        {
            Input.InputManager.Instance.RegisterHandler(this, false);
        }

        public void OnKeyPressed(InputSource source, int key, InputState state)
        {
            if (source == InputSource.Mouse)
            {
                if (key == (int)MouseKey.Left && state == InputState.Down)
                {
                    if (mInteractingWith != null)
                    {
                        mInteractingWith.OnInteractionEnd();
                        mInteractingWith = null;
                    }

                    if (mHoveredProp != null)
                    {
                        mInteractingWith = mHoveredProp;
                        mInteractingWith.OnInteractionStart();
                    }
                }
            }
        }

        public void OnMouseHoverChange(GameObject mouseHover)
        {
            mHoveredProp = null;

            if (mouseHover != null)
            {
                mHoveredProp = mouseHover.GetComponentInParent<PropBase>();
            }
        }

        public void OnMouseScrolled(float scrollDelta)
        {
            //throw new NotImplementedException();
        }
    }
}
