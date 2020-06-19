using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Events;

class MovementDirector : MonoBehaviour
{
    public delegate void MovementCompleteCB();

    private MovementCompleteCB mGoalReachedCB = null;
    private ActorController mBeingMoved;
    private List<Transform> mRoute = new List<Transform>();
    private int mCurrentGoalIdx = 0;

    public void Init()
    {
        
    }

    public void Cleanup()
    {

    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void RotateActor(ActorController toRotate, Transform goal, MovementCompleteCB callback)
    {
        mBeingMoved = toRotate;
        mGoalReachedCB = callback;
        StartCoroutine(RotateToNextGoal(goal));
    }

    public void DirectMovement(ActorController toMove, List<Transform> route, MovementCompleteCB callback)
    {
        mBeingMoved = toMove;
        mRoute = route;
        mCurrentGoalIdx = 0;
        mGoalReachedCB = callback;
        StartCoroutine(RotateToNextGoal(mRoute[mCurrentGoalIdx].transform));
    }

    IEnumerator RotateToNextGoal(Transform goal)
    {
        bool finishedRotating = false;

        Vector3 actorForwardFlattened = mBeingMoved.transform.forward;
        actorForwardFlattened.y = 0;
        Vector3 goalFlattened = goal.position - mBeingMoved.transform.position;
        goalFlattened.y = 0;

        float initialAngleToGoalDeg = Vector3.SignedAngle(actorForwardFlattened, goalFlattened, Vector3.up);

        float rotSpeed = mBeingMoved.RotSpeed * initialAngleToGoalDeg < 0 ? -1 : 1;
        mBeingMoved.mRigidBody.maxAngularVelocity = 10;
        mBeingMoved.mRigidBody.angularVelocity = Vector3.up * 10;

        while (!finishedRotating)
        {
            actorForwardFlattened = mBeingMoved.transform.forward;
            actorForwardFlattened.y = 0;
            goalFlattened = goal.transform.position - mBeingMoved.transform.position;
            goalFlattened.y = 0;

            float angleToGoalDeg = Vector3.SignedAngle(actorForwardFlattened, goalFlattened, Vector3.up);
            bool pastGoalAngle = initialAngleToGoalDeg < 0 ? angleToGoalDeg >= 0 : angleToGoalDeg <= 0;
            //if (Mathf.Abs(angleToGoalDeg) > 1 )
            //{
            //    float rotDegrees = Time.deltaTime * mBeingMoved.ActorController.RotSpeed;
            //    if (rotDegrees > Mathf.Abs(angleToGoalDeg)) rotDegrees = Mathf.Abs(angleToGoalDeg);
            //    if (initialAngleToGoalDeg < 0) rotDegrees *= -1;
            //    mBeingMoved.transform.Rotate(Vector3.up, rotDegrees); // LOL Fix this
            //    yield return new WaitForFixedUpdate();
            //}
            //else
            if (Mathf.Abs(angleToGoalDeg) <= 1 || pastGoalAngle)
            {
                finishedRotating = true;
                Vector3 finalForward = goal.transform.position - mBeingMoved.transform.position;
                finalForward.y = mBeingMoved.transform.forward.y;
                mBeingMoved.transform.forward = finalForward;
                mBeingMoved.mRigidBody.angularVelocity = Vector3.zero;
            }
            else {
                float rotDegrees = Time.deltaTime * mBeingMoved.RotSpeed;
                if (rotDegrees > Mathf.Abs(angleToGoalDeg)) rotDegrees = Mathf.Abs(angleToGoalDeg);
                if (initialAngleToGoalDeg < 0) rotDegrees *= -1;
                mBeingMoved.transform.Rotate(Vector3.up, rotDegrees); // LOL Fix this
            }
            yield return new WaitForFixedUpdate();
        }

        RotationComplete();
    }

    void RotationComplete()
    {
        if (mCurrentGoalIdx < mRoute.Count)
        {
            StartCoroutine(ProgressToNextGoal(mRoute[mCurrentGoalIdx]));
        }
        else
        {
            NotifyComplete();
        }
    }

    IEnumerator ProgressToNextGoal(Transform goal)
    {
        float timeoutTimer = 0;

        bool arrived = false;

        Vector3 initialForward = goal.position - mBeingMoved.transform.position;

        while (!arrived)
        {
            timeoutTimer += Time.deltaTime;

            if (timeoutTimer >= 5)
            {
                mBeingMoved.transform.position = goal.position;
            }

            Vector3 goalDirection = (goal.position - mBeingMoved.transform.position).normalized;
            if (goalDirection.magnitude >= .1f && Vector3.Angle(goalDirection, initialForward) < 1)
            {
                mBeingMoved.MoveCharacter(Vector3.forward);
                yield return new WaitForFixedUpdate();
            }
            else
            {
                arrived = true;
                mBeingMoved.transform.position = goal.position;
                mBeingMoved.MoveCharacter(Vector3.zero);
            }
        }
        GoalReached();
    }

    void GoalReached()
    {
        mCurrentGoalIdx++;
        mBeingMoved.mRigidBody.angularVelocity = Vector3.zero;
        if (mCurrentGoalIdx < mRoute.Count)
        {
            StartCoroutine(RotateToNextGoal(mRoute[mCurrentGoalIdx].transform));
        }
        else
        {
            NotifyComplete();
        }
    }

    void NotifyComplete()
    {
        mBeingMoved.MoveCharacter(Vector3.zero);
        mRoute.Clear();
        mCurrentGoalIdx = 0;

        if (mGoalReachedCB != null)
        {
            MovementCompleteCB cb = mGoalReachedCB;
            mGoalReachedCB = null;
            cb();
        }
    }
}

