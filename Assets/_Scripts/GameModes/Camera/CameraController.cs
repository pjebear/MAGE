using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace MAGE.GameModes.Cameras
{
    public enum CameraType
    {
        ThirdPerson,
        TopDown,
        Outfitter,
        NUM
    }

    class CameraController : MonoBehaviour
    {
        public Transform Focus;
        public CameraType CameraType;
        public Cinemachine.CinemachineFreeLook rTopDownCamera;
        public float TopDownCamera_RotSpeed = 15;
        public float TopDownCamera_ScrollSpeed = 10;

        public Cinemachine.CinemachineFreeLook rThirdPersonCamera;

        public Cinemachine.CinemachineFreeLook rOutfitterCamera;

        public void SetTarget(Transform target, CameraType cameraType)
        {
            Focus = target;
            UpdateCameraType(cameraType);
        }

        private void UpdateCameraType(CameraType cameraType)
        {
            CameraType = cameraType;

            switch (cameraType)
            {
                case CameraType.TopDown:
                {
                    rThirdPersonCamera.gameObject.SetActive(false);
                    rOutfitterCamera.gameObject.SetActive(false);

                    rTopDownCamera.gameObject.SetActive(true);
                    rTopDownCamera.Follow = Focus;
                    rTopDownCamera.LookAt = Focus;
                }
                break;

                case CameraType.ThirdPerson:
                {
                    rTopDownCamera.gameObject.SetActive(false);
                    rOutfitterCamera.gameObject.SetActive(false);

                    rThirdPersonCamera.gameObject.SetActive(true);
                    rThirdPersonCamera.Follow = Focus;
                    rThirdPersonCamera.LookAt = Focus;
                }
                break;

                case CameraType.Outfitter:
                {
                    rTopDownCamera.gameObject.SetActive(false);
                    rThirdPersonCamera.gameObject.SetActive(false);

                    rOutfitterCamera.gameObject.SetActive(true);
                    rOutfitterCamera.Follow = Focus;
                    rOutfitterCamera.LookAt = Focus;
                }
                break;
            }
        }

        private void Update()
        {
            Input.InputManager input = Input.InputManager.Instance;
            if (input != null)
            {
                Input.MouseInfo mouseInfo = input.GetMouseInfo();
                if (mouseInfo.IsOverWindow)
                {
                    if (mouseInfo.KeyStates[(int)MouseKey.Left] == InputState.Down)
                    {
                        //OnLeftClick(mouseInfo.ScreenPosCurr);
                    }
                    else if (mouseInfo.KeyStates[(int)MouseKey.Left] == InputState.Held)
                    {
                        Vector2 dragDelta = mouseInfo.ScreenPosCurr - mouseInfo.ScreenPosPrev;
                        OnLeftClickDrag(dragDelta);
                    }

                    if (mouseInfo.KeyStates[(int)MouseKey.Right] == InputState.Down)
                    {
                        //OnRightClick(mouseInfo.ScreenPosCurr);
                    }
                    else if (mouseInfo.KeyStates[(int)MouseKey.Right] == InputState.Held)
                    {
                        Vector2 dragDelta = mouseInfo.ScreenPosCurr - mouseInfo.ScreenPosPrev;
                       // OnRightClickDrag(dragDelta);
                    }

                    if (mouseInfo.ScrollDelta != 0)
                    {
                        OnMouseScroll(mouseInfo.ScrollDelta);
                    }

                    //if (mouseInfo.KeyStates[(int)MouseKey.Middle] == InputState.Down)
                    //{
                    //    OnMiddleClick(mouseInfo.ScreenPosCurr);
                    //}
                    //else if (mouseInfo.KeyStates[(int)MouseKey.Middle] == InputState.Held)
                    //{
                    //    Vector2 dragDelta = mouseInfo.ScreenPosCurr - mouseInfo.ScreenPosPrev;
                    //    OnMiddleClickDrag(dragDelta);
                    //}
                }
            }
        }

        void OnLeftClickDrag(Vector2 direction)
        {
            if (CameraType == CameraType.TopDown)
            {
                float xDelta = direction.x * TopDownCamera_RotSpeed * Time.deltaTime;
                rTopDownCamera.m_XAxis.Value += xDelta;
            }
        }

        void OnMouseScroll(float delta)
        {
            if (CameraType == CameraType.TopDown)
            {
                float currValue = rTopDownCamera.m_YAxis.Value;
                currValue -= delta * TopDownCamera_ScrollSpeed * Time.deltaTime;
                Mathf.Clamp(currValue, 0, 1);

                rTopDownCamera.m_YAxis.Value = currValue;
            }
        }
    }
}
