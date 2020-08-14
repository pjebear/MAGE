using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace MAGE.GameModes.SceneElements
{
    class ContainerProp : PropBase
    {
        enum ContainerState
        {
            Open,
            Closed,

            NUM
        }

        public ContainerPropId ContainerId = ContainerPropId.None;

        public Transform ContainerHinge;
        public Vector3 HingeAngle;
        public float OpenSpeedSec = 1;
        public float OpenAngle = 90;

        private Coroutine mContainerOpenRoutine = null;

        public override void Start()
        {
            base.Start();
        }

        public override int GetPropId()
        {
            return (int)ContainerId;
        }

        public override void OnInteractionEnd()
        {
            if (mContainerOpenRoutine != null)
            {
                StopCoroutine(mContainerOpenRoutine);
            }

            StartCoroutine(_Rotate(ContainerState.Closed));
        }

        public override void OnInteractionStart()
        {
            if (mContainerOpenRoutine != null)
            {
                StopCoroutine(mContainerOpenRoutine);
            }

            StartCoroutine(_Rotate(ContainerState.Open));
        }

        private IEnumerator _Rotate(ContainerState state)
        {
            float goalAngleDeg = state == ContainerState.Open ? OpenAngle : 0;

            float duration = 0;
            while (duration < OpenSpeedSec)
            {
                duration += Time.deltaTime;
                float rotationProgress = duration / OpenSpeedSec;

                Vector3 hingeLocation = ContainerHinge.position;
                ContainerHinge.transform.SetPositionAndRotation(ContainerHinge.position, Quaternion.Euler(HingeAngle * rotationProgress * goalAngleDeg));

                yield return new WaitForFixedUpdate();
            }

            ContainerHinge.transform.SetPositionAndRotation(ContainerHinge.position, Quaternion.Euler(HingeAngle * goalAngleDeg));
        }
    }
}
