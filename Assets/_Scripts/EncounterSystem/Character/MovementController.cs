using System.Collections;
using UnityEngine;

namespace EncounterSystem
{
    using Common.ActionTypes;
    using Common.AttributeEnums;
    using Map;
    using MapTypes;
    namespace Character
    {
        class MovementController : MonoBehaviour
        {
            public bool HasMoved = false;
            protected Animator animator;
            protected Rigidbody rb;

            //jumping variables
            bool isJumping = false;
            bool isFalling = false;
            bool mIsGrounded = true;

            public float CharacterHeightMultiplier = 1f;

            //movement variables
            private float walkSpeed = 1.85f;
            private float runSpeed = 6f;
            float horzSpeed = 0f;
            float mVertSpeed = 0f;
            float rotationSpeed = 40f;

            private Vector3 mTargetPosition;
            bool arrivedAtLocation = true;

            public MapTile CurrentTile;

            void Start()
            {
                animator = GetComponent<Animator>();
                rb = GetComponent<Rigidbody>();
            }

            #region MovementAnimation
            private void Update()
            {
                if (!arrivedAtLocation)
                {
                    CheckForGrounded();
                    if (Vector3.Dot(transform.forward,(mTargetPosition - transform.position)) <= 0)
                    {
                        //animator.SetBool("Moving", false);
                        arrivedAtLocation = true;
                        //transform.position = mTargetPosition;
                        horzSpeed = 0;
                        mVertSpeed = 0;
                    }
                    else
                    {
                        transform.position += (transform.forward * horzSpeed + Vector3.up * mVertSpeed) * Time.deltaTime;
                    }
                }
            }


            private void LateUpdate()
            {
                //Get local velocity of charcter
                float velocityXel = transform.InverseTransformDirection(rb.velocity).x;
                float velocityZel = transform.InverseTransformDirection(rb.velocity).z;
                //Update animator with movement values
                animator.SetFloat("Velocity X", velocityXel / walkSpeed);
                animator.SetFloat("Velocity Z", velocityZel / walkSpeed);
                //if character is alive and can move, set our animator
            }

            void CheckForGrounded()
            {
                float threshold = .2f;
                RaycastHit hit;

                if (Physics.Raycast((transform.position), -Vector3.up, out hit, 100f))
                {
                    if (hit.distance <= threshold && mVertSpeed <= 0) // only check if the character is falling
                    {
                        if (!mIsGrounded)
                        {
                            animator.SetInteger("Jumping", 0);
                            animator.SetTrigger("JumpTrigger");
                        }
                        mIsGrounded = true;
                        
                        mVertSpeed = 0f;
                        transform.position = hit.point;
                        if (!isJumping)
                        {
                            animator.SetInteger("Jumping", 0);
                        }
                    }
                    else
                    {
                        mIsGrounded = false;
                        mVertSpeed += Physics.gravity.y * Time.deltaTime;
                    }
                }
            }

            public void MoveTo(Vector3 nextPosition)
            {
                arrivedAtLocation = false;
                mTargetPosition = nextPosition;
                float elevationChange = mTargetPosition.y - transform.position.y;
                Vector3 forward = nextPosition - transform.position;
                forward.y = 0f;
                transform.rotation = Quaternion.LookRotation(forward, Vector3.up);

                if (elevationChange > 0.1f) //TODO: what is a realistic value when units start getting added
                {
                    animator.SetBool("Moving", false);
                    StartCoroutine(Jump());
                }
                else
                {
                    animator.SetBool("Moving", true);
                    horzSpeed = walkSpeed;
                    if (elevationChange < 0f)
                    {
                        StartCoroutine(_JumpDown());
                    }
                }
            }

            public void KnockBack(Vector3 direction)
            {
                MapManager instance = MapManager.Instance;
                direction.Normalize();
                CharacterManager character = GetComponent<CharacterManager>();
                MapTile currentTile = character.GetCurrentTile();
                MapTile knockOnTile = instance.GetTileAt(currentTile.transform.localPosition + direction);
                if (knockOnTile != null)
                {
                    // if no one else is on the tile
                    if (knockOnTile.GetCharacterOnTile() == null)
                    {
                        // is tile at or below the current elevation?
                        float elevationDifference = currentTile.transform.localPosition.y - knockOnTile.transform.localPosition.y;
                        if (elevationDifference <= 0)
                        {
                            currentTile.RemoveCharacterFromTile();

                            //switch tiles

                            character.UpdateCurrentTile(knockOnTile, false);
                            StartCoroutine(_KnockBack(character, direction, knockOnTile.transform.localPosition, elevationDifference));
                        }
                    }
                    
                }
            }

            private IEnumerator _KnockBack(CharacterManager beingKnocked, Vector3 direction, Vector3 targetLocation, float elevationDifference)
            {
                float knockSpeed = 5f;

                Vector3 displacement = (beingKnocked.transform.position - targetLocation);
                displacement.y = 0f;
                float distance = displacement.magnitude;
                do
                {
                    beingKnocked.transform.localPosition += direction * Time.deltaTime * knockSpeed;
                    displacement = (beingKnocked.transform.localPosition - targetLocation);
                    displacement.y = 0f;
                    distance = displacement.magnitude;
                    yield return new WaitForFixedUpdate();
                }
                while (distance > float.Epsilon);

                CheckForGrounded();
                float vertSpeed = 0f;
                while (!mIsGrounded)
                {
                    vertSpeed += Time.deltaTime * Physics.gravity.y;
                    beingKnocked.transform.position += Vector3.up * Time.deltaTime * vertSpeed;
                    CheckForGrounded();
                    yield return new WaitForFixedUpdate();
                }
                beingKnocked.GetCurrentTile().PlaceCharacterAtTileCenter();
                if (elevationDifference > .5f)
                {
                    float fallDamage = elevationDifference * 50f;
                    beingKnocked.ModifyCurrentResource(new ResourceChange(Resource.Health, fallDamage));
                }
            }

            public bool Arrived()
            {
                return arrivedAtLocation;
            }

            public void LookAt(Vector3 localPosition)
            {
                Vector3 direction = localPosition - gameObject.transform.localPosition;
                localPosition.y = 0f;
                gameObject.transform.localRotation = Quaternion.LookRotation(direction, Vector3.up);
            }

            public void CleanUp()
            {
                horzSpeed = 0f;
                mVertSpeed = 0f;
                //transform.position = mTargetPosition;
                animator.SetBool("Moving", false);
                if (animator.GetInteger("Jumping") != 0)
                {
                    Debug.Log("Ended walk cycle with non 0 jump trigger");
                    animator.SetInteger("Jumping", 0);
                    animator.SetTrigger("JumpTrigger");
                }
            }

            IEnumerator Jump()
            {
                isJumping = true;
                animator.SetInteger("Jumping", 1);
                animator.SetTrigger("JumpTrigger");
                isJumping = false;

                float characterHeight = GetComponent<CapsuleCollider>().height * transform.lossyScale.y;
                float elevationGain = (mTargetPosition.y - transform.position.y) + characterHeight * CharacterHeightMultiplier;
                mVertSpeed = Mathf.Sqrt(2f * -Physics.gravity.y * elevationGain);
                animator.SetInteger("Jumping", 3);
                while (transform.position.y < mTargetPosition.y)
                {
                    Debug.Log("ElevationDifference: " + (transform.position.y - mTargetPosition.y));
                    yield return new WaitForFixedUpdate();
                }


                Vector3 horzDistance = mTargetPosition - transform.position;
                horzDistance.y = 0;
                horzSpeed = Mathf.Abs(horzDistance.magnitude / (2f * mVertSpeed / -Physics.gravity.y));
            }

            IEnumerator _JumpDown()
            {
                yield return new WaitUntil(() => !mIsGrounded);
                animator.SetBool("Moving", false);
                animator.SetInteger("Jumping", 2);
                animator.SetTrigger("JumpTrigger");
                horzSpeed = 0;
                yield return new WaitUntil(() => mIsGrounded);
                animator.SetInteger("Jumping", 0);
                animator.SetTrigger("JumpTrigger");
                animator.SetBool("Moving", true);
                horzSpeed = walkSpeed;
            }
            #endregion
        }
    }
}


