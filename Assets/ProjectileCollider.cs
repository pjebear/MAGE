using EncounterSystem.Character;

using UnityEngine;

class ProjectileCollider : MonoBehaviour {

    public bool FinishedCollision;
    public CharacterManager CollidedWith = null;
    public CharacterManager Owner = null;
    public Vector3 TargetPosition;
    private Vector3 Velocity;
    private float timer;
    private float MaxTime = 5f;
    private void Awake()
    {
        FinishedCollision = false;
    }
    public void Initialize(Vector3 velocity, Vector3 targetPosition, CharacterManager owner)
    {
        Velocity = velocity;
        GetComponent<Rigidbody>().velocity = transform.TransformDirection(velocity);
        TargetPosition = targetPosition;
        Owner = owner;
        timer = 0f;
    }
	
	// Update is called once per frame
	void Update ()
    {
        timer += Time.deltaTime;
        if (timer > MaxTime)
        {
            FinishedCollision = true;
        }
        Vector3 velocity = GetComponent<Rigidbody>().velocity;
        //transform.localPosition += (Vector3.forward * Velocity.z + Vector3.up * Velocity.y) * Time.deltaTime;
        transform.rotation = Quaternion.LookRotation(velocity);
        if (TargetPosition.y - transform.position.y > 0 && velocity.y < 0)
        {
            FinishedCollision = true;
        }
	}

    private void OnTriggerEnter(Collider other)
    {
        //Debug.Log("Trigger Entered " + other.gameObject.name);
        if (other.CompareTag("Player") || other.CompareTag("MapTile"))
        {
            CharacterManager otherUnit = other.gameObject.GetComponent<CharacterManager>();
            if (otherUnit != Owner && otherUnit.IsAlive) // Not colliding with a dead unit
            {
                FinishedCollision = true;
                CollidedWith = otherUnit;
            }
        }
    }
}
