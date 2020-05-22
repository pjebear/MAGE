using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Invector;
using UnityEngine;

class ThirdPersonCamera 
    : MonoBehaviour
    , IInputHandler
{

    public Transform Target = null;
    [SerializeField] private float mFollowDistance = 5;
    [SerializeField] private float mHeight = 1;

    void OnDestroy()
    {
        transform.SetParent(null);
    }

    void FixedUpdate()
    {
        transform.position = Target.position - Target.forward * mFollowDistance + Vector3.up * mHeight;
        transform.LookAt(Target);
    }

    public void SetTarget(Transform target)
    {
        Target = target;
        //transform.SetParent(target);
        //transform.localPosition = target.forward * -1 * mFollowDistance + Vector3.up * mHeight;
    }

    #region IInputHandler
    // IInputHandler
    public void OnKeyPressed(InputSource source, int key, InputState state)
    {
        if (source == InputSource.Keyboard)
        {
            int inputValue = state == InputState.Up ? 0 : 1;

            switch ((KeyCode)key)
            {
                case (KeyCode.A):
                {
                    
                }
                break;
                case (KeyCode.D):
                {
                    
                }
                break;
            }
        }
    }

    public void OnMouseHoverChange(GameObject mouseHover)
    {
        // empty
    }

    public void OnMouseScrolled(float scrollDelta)
    {
        // empty 
    }
    #endregion // IInputHandler
}

