using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace MAGE.Input
{
    class InputManager
       : MonoBehaviour
 
    {
        public static InputManager Instance;

        MouseInputController MouseInputController;
        KeyboardInputController KeyboardInputController;
        CameraRaycaster CameraRayCaster;

        List<KeyValuePair<IInputHandler, bool>> mInputHandlers;

        void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
                mInputHandlers = new List<KeyValuePair<IInputHandler, bool>>();

                MouseInputController = GetComponent<MouseInputController>();
                KeyboardInputController = GetComponent<KeyboardInputController>();
                CameraRayCaster = GetComponent<CameraRaycaster>();
            }
        }

        private void Start()
        {
            KeyboardInputController.RegisterForKeyboardInput(KeyboardInputCB);
            MouseInputController.RegisterForMouseInput(MouseInputCB);
            MouseInputController.RegisterForMouseScroll(MouseScrollCB);
            CameraRayCaster.RegisterForHoverChange(MouseHoverCB);
        }

        void MouseInputCB(MouseKey key, InputState state)
        {
            for (int i = mInputHandlers.Count - 1; i >= 0; --i)
            {
                mInputHandlers[i].Key.OnKeyPressed(InputSource.Mouse, (int)key, state);

                if (mInputHandlers[i].Value)
                    break;
            }
        }

        void MouseScrollCB(float delta)
        {
            for (int i = mInputHandlers.Count - 1; i >= 0; --i)
            {
                mInputHandlers[i].Key.OnMouseScrolled(delta);

                if (mInputHandlers[i].Value)
                    break;
            }
        }

        void KeyboardInputCB(KeyCode key, InputState state)
        {
            for (int i = mInputHandlers.Count - 1; i >= 0; --i)
            {
                mInputHandlers[i].Key.OnKeyPressed(InputSource.Keyboard, (int)key, state);

                if (mInputHandlers[i].Value)
                    break;
            }
        }

        void MouseHoverCB(GameObject hoverObj)
        {
            for (int i = mInputHandlers.Count - 1; i >= 0; --i)
            {
                mInputHandlers[i].Key.OnMouseHoverChange(hoverObj);

                if (mInputHandlers[i].Value)
                    break;
            }
        }

        // public
        public void RegisterHandler(IInputHandler handler, bool consumeInput)
        {
            mInputHandlers.Add(new KeyValuePair<IInputHandler, bool>(handler, consumeInput));
        }

        public void ReleaseHandler(IInputHandler handler)
        {
            mInputHandlers.RemoveAll((x) => { return x.Key == handler; });
        }
   }
}





