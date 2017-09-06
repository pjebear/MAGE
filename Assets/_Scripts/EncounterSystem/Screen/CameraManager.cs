using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace EncounterSystem.Screen
{
    class CameraManager : MonoBehaviour
    {
        private Camera mCamera;
        public Vector3 DefaultPosition = new Vector3(0, 5, -7);
        public float MaxHorizontalOffset = 5;
        public float MaxVerticalOffset = 5;
        public float MaxPerspectiveZoomIn = 50;
        public float MaxPerspectiveZoomOut = 60;
        public float MaxOrthoZoomIn = 2;
        public float MaxOrthoZoomOut = 3;

        public float ZoomSpeed = 15;
        public float HorizontalPanSpeed = 10;
        public float VerticalPanSpeed = 5;

        private GameObject mPanTarget;
        private float mPanSpeed = 3f;


        private void Update()
        {
            if (mPanTarget != null)
            {
                Vector3 screenSpaceLocation = Camera.main.WorldToScreenPoint(mPanTarget.transform.position);
                Vector3 panVector = (screenSpaceLocation - new Vector3(UnityEngine.Screen.width / 2, UnityEngine.Screen.height / 2, 0));
                panVector.z = 0f;
                if (panVector.sqrMagnitude < 2000)
                {
                    mPanTarget = null;
                }
                else
                {
                    mCamera.transform.position += panVector.normalized * Time.deltaTime * mPanSpeed;
                }
            }
        }

        private void Awake()
        {
            mCamera = gameObject.GetComponent<Camera>();
            gameObject.transform.position = DefaultPosition;
            Debug.Assert(mCamera != null, "No Camera attached to CameraManager!");
        }

        public void PanHorizontal(bool left)
        {
            if (mPanTarget == null)
            {
                mCamera.transform.position += (left ? Vector3.left : Vector3.right) * HorizontalPanSpeed * Time.deltaTime;
                Vector3 offset = transform.position - DefaultPosition;
                if (offset.x < -MaxHorizontalOffset)
                {
                    offset.x = -MaxHorizontalOffset;
                }
                if (offset.x > MaxHorizontalOffset)
                {
                    offset.x = MaxHorizontalOffset;
                }
                transform.position = DefaultPosition + offset;
            }
        }

        public void PanVertical(bool down)
        {
            if (mPanTarget == null)
            {
                mCamera.transform.position += (down ? Vector3.down : Vector3.up) * VerticalPanSpeed * Time.deltaTime;
                Vector3 offset = transform.position - DefaultPosition;
                if (offset.y < -MaxVerticalOffset)
                {
                    offset.y = -MaxVerticalOffset;
                }
                if (offset.y > MaxVerticalOffset)
                {
                    offset.y = MaxVerticalOffset;
                }
                transform.position = DefaultPosition + offset;
            }
        }

        public void PanToTarget(GameObject panTarget)
        {
            mPanTarget = panTarget;
        }

        public void Zoom(bool zoomOut)
        {
            bool orthographic = mCamera.orthographic;
            if (orthographic)
            {
                mCamera.orthographicSize += (zoomOut ? 1 : -1) * ZoomSpeed * Time.deltaTime;
                if (mCamera.orthographicSize < MaxOrthoZoomIn)
                {
                    mCamera.orthographicSize = MaxOrthoZoomIn;
                }
                if (mCamera.orthographicSize > MaxOrthoZoomOut)
                {
                    mCamera.orthographicSize = MaxOrthoZoomOut;
                }
            }
            else
            {
                mCamera.fieldOfView += (zoomOut ? 1 : -1) * ZoomSpeed * Time.deltaTime;
                if (mCamera.fieldOfView < MaxPerspectiveZoomIn)
                {
                    mCamera.fieldOfView = MaxPerspectiveZoomIn;
                }
                if (mCamera.fieldOfView > MaxPerspectiveZoomOut)
                {
                    mCamera.fieldOfView = MaxPerspectiveZoomOut;
                }
            }
        }
    }
}
