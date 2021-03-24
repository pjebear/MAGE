using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.AI;

namespace MAGE.GameModes.SceneElements.Navigation
{
    class TopDownActorController : MonoBehaviour
    {
        public float CameraRotSpeed = 15;
        public float CameraScrollSpeed = 10;
        public GameObject TopDownCamera;

        private Actor rActor;
        
        private void Awake()
        {
            rActor = GetComponent<Actor>();
        }

        private void Start()
        {
                    
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
                        OnLeftClick(mouseInfo.ScreenPosCurr);
                    }
                    else if (mouseInfo.KeyStates[(int)MouseKey.Left] == InputState.Held)
                    {
                        Vector2 dragDelta = mouseInfo.ScreenPosCurr - mouseInfo.ScreenPosPrev;
                        OnLeftClickDrag(dragDelta);
                    }

                    if (mouseInfo.KeyStates[(int)MouseKey.Right] == InputState.Down)
                    {
                        OnRightClick(mouseInfo.ScreenPosCurr);
                    }
                    else if (mouseInfo.KeyStates[(int)MouseKey.Right] == InputState.Held)
                    {
                        Vector2 dragDelta = mouseInfo.ScreenPosCurr - mouseInfo.ScreenPosPrev;
                        OnRightClickDrag(dragDelta);
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


            rActor.GetComponent<ActorAnimator>().SetCurrentSpeed(rActor.GetComponent<ActorMotor>().GetCurrentSpeed());
        }

        void OnLeftClick(Vector2 screenPos)
        {
           // empty
        }

        void OnLeftClickDrag(Vector2 direction)
        {
            // empty
        }

        void OnRightClick(Vector2 screenPos)
        {
            Ray ray = Camera.main.ScreenPointToRay(screenPos);
            RaycastHit hit;

            int layerMask = 1 << (int)RayCastLayer.Terrain;
            if (Physics.Raycast(ray, out hit, 100, layerMask))
            {
                MoveToPoint(hit.point);
            }
        }

        void OnRightClickDrag(Vector2 direction)
        {
            // empty
        }

        void OnMouseScroll(float delta)
        {
            // empty
        }

        public void MoveToPoint(Vector3 point)
        {
            rActor.GetComponent<ActorMotor>().MoveToPoint(point);
        }

        public void Enable(bool enabled)
        {
            this.enabled = enabled;

            rActor.GetComponent<ActorMotor>().Enable(enabled);

            if (!enabled)
            {
                rActor.GetComponent<ActorAnimator>().SetCurrentSpeed(0);
            }
        }
    }
}
