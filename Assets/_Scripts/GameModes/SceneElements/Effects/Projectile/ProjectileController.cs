using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

class ProjectileController : MonoBehaviour
{
    public Collider Collider;
    public Rigidbody Rigidbody;

    float mExpectedDuration = 0;
    float mFlightTime = 0;

    public void Init(Vector3 forward, float velocity, bool useGravity, float expectedDuration)
    {
        transform.forward = forward;
        Rigidbody.velocity = transform.forward * velocity;
        Rigidbody.useGravity = useGravity;
        mExpectedDuration = expectedDuration;
    }

    private void OnTriggerEnter(Collider other)
    {
        float durationVarience = mExpectedDuration - mFlightTime;
        Logger.Log(LogTag.GameModes, "Projectile", 
            string.Format("Projectile collision event occured. FlightTime[{0}] ExpectedFlightTime[{1}] CollidedWith[{2}]", mFlightTime, mExpectedDuration, other.name));
    }

    private void Update()
    {
        transform.forward = Rigidbody.velocity.normalized;

        mFlightTime += Time.deltaTime;
        if (mFlightTime >= mExpectedDuration)
        {
            Logger.Log(LogTag.GameModes, "Projectile", "Duration Elapsed. Destroying");
            Destroy(gameObject);
        }
    }
}

