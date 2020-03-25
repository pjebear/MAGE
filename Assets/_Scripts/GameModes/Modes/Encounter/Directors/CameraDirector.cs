using UnityEngine;
using System.Collections.Generic;

class CameraDirector : MonoBehaviour
{

    private Transform m_currentTarget;

    [SerializeField]
    private Vector3 mPositionOffset;

    [SerializeField]
    private Vector3 mTargetOffset;

    private int m_currentIndex;

    private void LateUpdate()
    {
        if(m_currentTarget == null) { return; }

        float targetHeight = m_currentTarget.position.y + mTargetOffset.y;
        float currentRotationAngle = m_currentTarget.rotation.eulerAngles.y;

        Quaternion currentRotation = Quaternion.Euler(0, currentRotationAngle, 0);

        Vector3 position = m_currentTarget.position;
        position += currentRotation * Vector3.forward * mTargetOffset.z;
        position.y = targetHeight;

        transform.position = position + mPositionOffset;
        transform.LookAt(m_currentTarget.position + new Vector3(0, mTargetOffset.y, 0));
    }

    public void FocusTarget(Transform target)
    {
        m_currentTarget = target;
    }

    public void ClearFocus()
    {
        m_currentTarget = null;
    }
}
