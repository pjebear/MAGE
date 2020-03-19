using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Events;



class MouseInputController : MonoBehaviour
{
    private UnityAction<MouseKey, InputState> mMouseCallback;

    private void Awake()
    {
        mMouseCallback = null;
    }

    void Update()
    {
        if (mMouseCallback != null)
            GetMouseInput();
    }

    void GetMouseInput()
    {
        for (int i = 0; i < (int)MouseKey.NUM; ++i)
        {
            if (UnityEngine.Input.GetMouseButtonDown(i))
            {
                mMouseCallback((MouseKey)i, InputState.Down);
            }
            else if (UnityEngine.Input.GetMouseButtonUp(i))
            {
                mMouseCallback((MouseKey)i, InputState.Up);
            }
            else if (UnityEngine.Input.GetMouseButton(i))
            {
                mMouseCallback((MouseKey)i, InputState.Held);
            }
        }
    }



    // public
    public void RegisterForMouseInput(UnityAction<MouseKey, InputState> mouseInputCB)
    {
        mMouseCallback = mouseInputCB;
    }
}



