using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Events;



class MouseInputController : MonoBehaviour
{
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
        for (int i = 0; i < (int)MouseKey.NUM; ++i)
        {
            if (UnityEngine.Input.GetMouseButtonDown(i))
            {
                mButtonCallback((MouseKey)i, InputState.Down);
            }
            else if (UnityEngine.Input.GetMouseButtonUp(i))
            {
                mButtonCallback((MouseKey)i, InputState.Up);
            }
            else if (UnityEngine.Input.GetMouseButton(i))
            {
                mButtonCallback((MouseKey)i, InputState.Held);
            }
        }

        if (Input.mouseScrollDelta.y != 0)
        {
            mScrollCallback(Input.mouseScrollDelta.y);
        }
    }

    // public
    public void RegisterForMouseInput(UnityAction<MouseKey, InputState> mouseInputCB)
    {
        mButtonCallback = mouseInputCB;
    }

    public void RegisterForMouseScroll(UnityAction<float> mouseInputCB)
    {
        mScrollCallback = mouseInputCB;
    }
}



