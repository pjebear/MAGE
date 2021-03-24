using MAGE.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace MAGE.GameModes.Cameras
{
    class TopDownCameraController : MonoBehaviour
    {
        public Transform Target;

        public float Pitch = 2f;

        public Vector3 Offset;

        public float ZoomSpeed = 4f;
        public float MinZoom = 5f;
        public float MaxZoom = 15f;
        private float mCurrentZoom = 10f;

        public float RotateSpeed = 4f;
        private float mCurrentRotateDeg = 0f;

        private void Start()
        {
            mCurrentRotateDeg = Target.rotation.eulerAngles.y + 180;
        }

        private void Update()
        {
            MouseInfo info = Input.InputManager.Instance.GetMouseInfo();

            // zoom
            float zoomInput = info.ScrollDelta;
            mCurrentZoom -=  zoomInput * ZoomSpeed * Time.deltaTime;
            mCurrentZoom = Mathf.Clamp(mCurrentZoom, MinZoom, MaxZoom);

            // rotation 
            if (info.KeyStates[(int)MouseKey.Left] == InputState.Held)
            {
                float deltaMouseX = info.ScreenPosCurr.x - info.ScreenPosPrev.x;
                mCurrentRotateDeg += deltaMouseX * RotateSpeed * Time.deltaTime;
                while (mCurrentRotateDeg < 0) mCurrentRotateDeg += 360;
                while (mCurrentRotateDeg >= 360) mCurrentRotateDeg -= 360;
            }
        }

        private void LateUpdate()
        {
            if (Target != null)
            {
                Vector3 offset = Offset * mCurrentZoom;

                transform.position = Target.transform.position;
                transform.rotation = Quaternion.Euler(Vector3.up * mCurrentRotateDeg);
                transform.position = transform.position - transform.forward * offset.z;
                transform.position = transform.position + Vector3.up * offset.y;

                transform.LookAt(Target.transform.position + Vector3.up * Pitch);
            }
        }
    }
}
