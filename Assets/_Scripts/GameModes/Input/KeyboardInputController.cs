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
            InputState keyState = GetKeyState(key);
            if (keyState != InputState.None)
            {
                mKeyboardCallback(key, keyState);
            }
        }
    }

    // public
    public void RegisterForKeyboardInput(UnityAction<KeyCode, InputState> keyInputCB)
    {
        mKeyboardCallback = keyInputCB;
    }

    public InputState GetKeyState(KeyCode key)
    {
        InputState inputState = InputState.None;

        if (UnityEngine.Input.GetKey(key))
        {
            inputState = InputState.Held;
        }
        else if (UnityEngine.Input.GetKeyDown(key))
        {
            inputState = InputState.Down;
        }
        else if (UnityEngine.Input.GetKeyUp(key))
        {
            inputState = InputState.Up;
        }

        return inputState;
    }
}



