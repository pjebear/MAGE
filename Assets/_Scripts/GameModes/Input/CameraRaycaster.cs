using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

class CameraRaycaster : MonoBehaviour
{
    private static readonly string LOG_NAME = "CameraRayCaster";

    private const int TILE_LAYER = 8;
    private const int INTERACTABLE_LAYER = 9;

    private int[] mRayCastLayers =
    {
        TILE_LAYER,
        INTERACTABLE_LAYER
    };

    [SerializeField]
    private float mMaxCastRange = 100f;

    private RaycastHit mHit;
    public RaycastHit Hit { get { return mHit; } }

    GameObject mHovered = null;

    private Camera mCaster;
    private List<UnityAction<GameObject>> mHoverChangeCBs;

    private void Awake()
    {
        mHoverChangeCBs = new List<UnityAction<GameObject>>();
        
    }

    // Update is called once per frame
    void LateUpdate()
    {
        GameObject hovered = null;

        if (Camera.main != null && !EventSystem.current.IsPointerOverGameObject())
        {
            Ray ray = Camera.main.ScreenPointToRay(UnityEngine.Input.mousePosition);

            for (int layerIndex = 0; layerIndex < mRayCastLayers.Length; ++layerIndex)
            {
                int layerMask = 1 << mRayCastLayers[layerIndex];
                if (Physics.Raycast(ray, out mHit, mMaxCastRange, layerMask))
                {
                    hovered = mHit.collider.gameObject;
                    break; // return the first hit in the priority search
                }
            }
        }

        if (mHovered != hovered)
        {
            mHovered = hovered;
            foreach (UnityAction<GameObject> hoverCB in mHoverChangeCBs)
            {
                hoverCB(mHovered);
            }
        }
    }

    public void RegisterForHoverChange(UnityAction<GameObject> hoverCB)
    {
        //Logger.Log(LOG_NAME, "RegisterForHoverChange()");
        if (!mHoverChangeCBs.Contains(hoverCB))
        {
            mHoverChangeCBs.Add(hoverCB);
        }
        else
        {
            //Logger.Log(LOG_NAME, "RegisterForHoverChange() CB already present in the list.", Logger.LogLevel.WarningWillRobinson);
        }
    }

    public void UnRegisterForHoverChange(UnityAction<GameObject> hoverCB)
    {
        //Logger.Log(LOG_NAME, "UnRegisterForHoverChange()");
        if (!mHoverChangeCBs.Remove(hoverCB))
        {
            //Logger.Log(LOG_NAME, "UnRegisterForHoverChange() CB wasn't present in the list.", Logger.LogLevel.WarningWillRobinson);
        }
    }
}



