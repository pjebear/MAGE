using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Events;


class KeyboardInputController : MonoBehaviour
{
    private UnityAction<KeyCode, InputState> mKeyboardCallback;

    private void Awake()
    {
        mKeyboardCallback = null;
    }

    void Update()
    {
        if (mKeyboardCallback != null)
            GetKeyboardInput();
    }

    void GetKeyboardInput()
    {
        foreach (KeyCode key in Enum.GetValues(typeof(KeyCode)))
        {
            if (UnityEngine.Input.GetKey(key))
            {
                mKeyboardCallback(key, InputState.Held);
            }

            if (UnityEngine.Input.GetKeyDown(key))
            {
                mKeyboardCallback(key, InputState.Down);
            }

            if (UnityEngine.Input.GetKeyUp(key))
            {
                mKeyboardCallback(key, InputState.Up);
            }
        }
    }

    // public
    public void RegisterForKeyboardInput(UnityAction<KeyCode, InputState> keyInputCB)
    {
        mKeyboardCallback = keyInputCB;
    }
}



