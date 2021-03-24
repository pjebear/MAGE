using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEditor;
using UnityEngine;
using UnityEngine.Events;


namespace MAGE.Input
{
    class MouseInfo
    {
        public InputState[] KeyStates = new InputState[(int)MouseKey.NUM];
        public float ScrollDelta = 0;
        public Vector2 ScreenPosCurr = Vector2.zero;
        public Vector2 ScreenPosPrev = Vector2.zero;
        public bool IsOverWindow = true;
    }

    class MouseInputController : MonoBehaviour
    {
        private MouseInfo mMouseInfo = new MouseInfo();
        private UnityAction<MouseKey, InputState> mButtonCallback;
        private UnityAction<float> mScrollCallback;

        private void Awake()
        {
            mButtonCallback = null;
            mScrollCallback = null;
        }

        void Update()
        {
            if (mButtonCallback != null)
                GetMouseInput();
        }

        void GetMouseInput()
        {
            mMouseInfo.IsOverWindow = IsMouseOverWindow();

            if (mMouseInfo.IsOverWindow)
            {
                mMouseInfo.ScreenPosPrev = mMouseInfo.ScreenPosCurr;
                mMouseInfo.ScreenPosCurr = GetMouseScreenPos();

                for (int i = 0; i < (int)MouseKey.NUM; ++i)
                {
                    mMouseInfo.KeyStates[i] = GetInputStateForKey((MouseKey)i);

                    if (mMouseInfo.KeyStates[i] != InputState.None)
                    {
                        mButtonCallback((MouseKey)i, mMouseInfo.KeyStates[i]);
                    }
                }

                mMouseInfo.ScrollDelta = GetScrollDelta();

                if (mMouseInfo.ScrollDelta != 0)
                {
                    mScrollCallback(mMouseInfo.ScrollDelta);
                }
            }
            else
            {
                mMouseInfo.ScreenPosPrev = Vector2.zero;
                mMouseInfo.ScreenPosCurr = Vector2.zero;

                for (int i = 0; i < (int)MouseKey.NUM; ++i)
                {
                    mMouseInfo.KeyStates[i] = InputState.None;
                }

                mMouseInfo.ScrollDelta = 0;
            }
        }

        // public
        public InputState GetInputStateForKey(MouseKey key)
        {
            InputState inputState = InputState.None;

            if (UnityEngine.Input.GetMouseButtonDown((int)key))
            {
                inputState = InputState.Down;
            }
            else if (UnityEngine.Input.GetMouseButtonUp((int)key))
            {
                inputState = InputState.Up;
            }
            else if (UnityEngine.Input.GetMouseButton((int)key))
            {
                inputState = InputState.Held;
            }

            return inputState;
        }

        public float GetScrollDelta()
        {
            return UnityEngine.Input.mouseScrollDelta.y;
        }

        public Vector2 GetMouseScreenPos()
        {
            return UnityEngine.Input.mousePosition;
        }

        public bool IsMouseOverWindow()
        {
            Vector2 currMousePos = GetMouseScreenPos();

            bool overWindow = false;
#if UNITY_EDITOR
            overWindow = (currMousePos.x > 0 && currMousePos.y > 0 && currMousePos.x < Handles.GetMainGameViewSize().x && currMousePos.y < Handles.GetMainGameViewSize().y);
#else
            overWindow = (currMousePos.x > 0 && currMousePos.y > 0 && currMousePos.x < Screen.width && currMousePos.y < Screen.height);
#endif
            return overWindow;
        }

        public MouseInfo GetMouseInfo()
        {
            return mMouseInfo;
        }

        public void RegisterForMouseInput(UnityAction<MouseKey, InputState> mouseInputCB)
        {
            mButtonCallback = mouseInputCB;
        }

        public void RegisterForMouseScroll(UnityAction<float> mouseInputCB)
        {
            mScrollCallback = mouseInputCB;
        }
    }
}
