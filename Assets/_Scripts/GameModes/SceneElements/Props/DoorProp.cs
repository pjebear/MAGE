using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace MAGE.GameModes.SceneElements
{
    class DoorProp : PropBase
    {
        [System.Serializable]
        public struct HingeInfo
        {
            public Transform Hinge;
            public Vector3 RotationAxis;
        }

        enum DoorState
        {
            Closed,
            Open,

            NUM
        }

        public DoorPropId DoorPropId = DoorPropId.None;

        public List<HingeInfo> Hinges = new List<HingeInfo>();

        public float OpenSpeedSec = 1;
        public float OpenAngle = 90;

        private Coroutine mOpenRoutine = null;
        private DoorState mState = DoorState.Closed;

        public override void Start()
        {
            base.Start();
        }

        public override int GetPropId()
        {
            return (int)DoorPropId;
        }

        protected override void Refresh()
        {
            base.Refresh();

            DoorState updatedState = (DoorState)PropInfo.State;
            if (mState != updatedState)
            {
                Logger.Log(LogTag.Level, DoorPropId.ToString(), 
                    string.Format("::Refresh() - State updated from {0} to {1}.", mState.ToString(), updatedState.ToString()));

                mState = updatedState;
                SetDoorsFromState();
            }
        }

        private void SetDoorsFromState()
        {
            Debug.Assert(mOpenRoutine == null);
            if (mOpenRoutine != null)
            {
                StopCoroutine(mOpenRoutine);
            }

            float angle = mState == DoorState.Open ? OpenAngle : 0;
            foreach (HingeInfo hingeInfo in Hinges)
            {
                hingeInfo.Hinge.transform.SetPositionAndRotation(hingeInfo.Hinge.position, Quaternion.Euler(hingeInfo.RotationAxis * angle));
            }
        }

        public override void OnInteractionEnd()
        {
            // empty
        }

        public override void OnInteractionStart()
        {
            PropInfo info = LevelManagementService.Get().GetPropInfo(GetPropId());
            Debug.Assert(info.State == PropInfo.State);

            mState = mState == DoorState.Open ? DoorState.Closed : DoorState.Open;
            info.State = (int)mState;
            LevelManagementService.Get().UpdatePropInfo(info);

            if (mOpenRoutine != null)
            {
                StopCoroutine(mOpenRoutine);
            }

            mOpenRoutine = StartCoroutine(_Rotate(mState));
        }

        private IEnumerator _Rotate(DoorState state)
        {
            float goalAngleDeg = state == DoorState.Open ? OpenAngle : 0;

            float duration = 0;
            while (duration < OpenSpeedSec)
            {
                duration += Time.deltaTime;
                float rotationProgress = duration / OpenSpeedSec;

                foreach (HingeInfo hingeInfo in Hinges)
                {
                    
                    hingeInfo.Hinge.transform.SetPositionAndRotation(hingeInfo.Hinge.position, Quaternion.Euler(hingeInfo.RotationAxis * rotationProgress * goalAngleDeg));
                }

                yield return new WaitForFixedUpdate();
            }

            if (mOpenRoutine != null)
            {
                StopCoroutine(mOpenRoutine);
                mOpenRoutine = null;
            }

            SetDoorsFromState();
        }
    }
}
