using UnityEngine;
using System.Collections;
using System.Linq;

using Common.ActionTypes;
using System.Collections.Generic;
using Common.EquipmentTypes;
using Common.ActionEnums;

namespace EncounterSystem.Action
{
    using Character;
    using MapEnums;
    using MapTypes;

    class ActionRangedWeaponBase : ActionBase
    {
        public float StraightShotSpeed = 10f;
        public float ArcSpeed = 10f;
        private float mCharacterHeightOffset = 0.4f; // The point on the character the projectile will seek to hit

        public override IEnumerator ExececuteAction(CharacterManager caster, List<MapTile> targets, System.Action<ActionInteractionResult> _RecordActionResults)
        {
            Debug.Assert(targets.Count <= 1, "Recieved more than one target for Ranged action");
            FinishedExecution = false;

            List<WeaponBase> weapons = caster.GetHeldWeapons();
            int counter = 1;
            foreach (var weapon in weapons)
            {
                if (weapon != null && weapon.ActionIndex == ActionIndex.RANGED_WEAPON)
                {
                    ActionResourceChangeInformation resourceChangeInformation = caster.GetModifiedActionStrength(ActionInfo.BaseResourceChangeInfo, weapon.DamageModifiers);
                    caster.GetComponent<Animator>().SetTrigger(string.Format("Attack{0}Trigger", 3 * counter++));

                    yield return new WaitForSeconds(0.5f);

                    if (targets.Count > 0 && targets[0] != null)
                    {
                        ProjectileCollider arrow = (Instantiate(Resources.Load("Weapons/arrow_1")) as GameObject).GetComponent<ProjectileCollider>();
                        Vector3 trajectory = Vector3.zero;
                        // Arc attack
                        if (weapon.AttackAreaInfo.ActionAreaType == TileAreaType.Ring)
                        {
                            trajectory = FindArc(caster.transform.position + Vector3.up * mCharacterHeightOffset, targets[0].transform.GetChild(0).position + Vector3.up * mCharacterHeightOffset);
                            arrow.GetComponent<Rigidbody>().useGravity = true;
                        }
                        else // linear shot
                        {
                            trajectory = FindStraightShot(caster.transform.position + Vector3.up * mCharacterHeightOffset, targets[0].transform.GetChild(0).position + Vector3.up * mCharacterHeightOffset);
                            arrow.GetComponent<Rigidbody>().useGravity = false;
                        }
                         
                        //Debug.Log("trajectory calculated: " + trajectory);
                        // Spawn Arrow
                        //...
                        
                       
                        arrow.transform.parent = caster.transform;
                        arrow.transform.localPosition = Vector3.up * mCharacterHeightOffset;
                        arrow.transform.forward = caster.transform.forward;
                        arrow.Initialize(trajectory, targets[0].GetTileCenter(), caster);

                        //Debug.Log("Spawned projectile");
                        


                        yield return new WaitUntil(() => { return arrow.FinishedCollision; });

                        if (arrow.CollidedWith != null)
                        {
                            ActionInteractionResult actionResults = new ActionInteractionResult(arrow.CollidedWith);
                            ActionOrientation targetOrientation = ActionUtil.GetActionOrienation(caster.transform.position, actionResults.Target.transform.position, actionResults.Target.transform.forward);

                            actionResults.Target.AttemptActionResourceChange(resourceChangeInformation, ref actionResults, targetOrientation);
                            _RecordActionResults(actionResults);
                        }
                        // wait for trigger to hit something
                        //...
                        // check if the trigger hit a unit
                        //...
                        // deal damage

                        Destroy(arrow.gameObject);
                    }

                    yield return new WaitForSeconds(1);
                    break;
                }
            }

            FinishedExecution = true;
        }

        Vector3 FindStraightShot(Vector3 start, Vector3 target)
        {
            Vector3 trajectory = Vector3.zero;
            Vector3 direction = target - start;
            direction.y = 0f;
            float xDist = direction.magnitude;
            float yDist = target.y - start.y;

            float timeToTarget = xDist / StraightShotSpeed;
            float vertSpeed = yDist / timeToTarget;
            trajectory.z = StraightShotSpeed;
            trajectory.y = vertSpeed;

            return trajectory;
        }

        //helper functions for Bow Arc
        Vector3 FindArc(Vector3 start, Vector3 target)
        {
            Vector3 arc = Vector3.zero;
            float angle1 = 0f;
            float angle2 = 0f;

            float baseProjectileSpeed = ArcSpeed;
            bool foundLaunchAngle = false;
            do
            {
                foundLaunchAngle = FindLaunchAngle(start, target, out angle1, out angle2);
                if (!foundLaunchAngle)
                {
                    ArcSpeed++;
                }
                else if (angle1 < 0 || angle2 < 0)
                {
                    foundLaunchAngle = false;
                    ArcSpeed--;
                }
            }
            while (!foundLaunchAngle);
            
            if (baseProjectileSpeed != ArcSpeed)
            {
                //Debug.LogFormat("Projectile speed needed calibrating to hit shoot from {0} to {1}. Current Projectile Speed: {2}, Needed: {3}", start, target, baseProjectileSpeed, ArcSpeed);
            }
            if (FindLaunchAngle(start, target, out angle1, out angle2))
            {
                arc.x = 0f;
                //float tallestPoint = FindHighestPoint(new Vector2(transform.parent.position.x, transform.parent.position.z), new Vector2(target.x, target.z));
                bool arcChosen = false;
                bool isLob = false;
                Vector3 rayStart = start;
                RaycastHit hitInfo;
                do
                {
                    if (Physics.Linecast(rayStart, target, out hitInfo, 1 << LayerMask.NameToLayer("Map")))
                    {
                        string tag = hitInfo.collider.tag;
                        if (tag == "MapTile")
                        {
                            arcChosen = isLob = true;
                        }
                        else if (tag == "Player")
                        {
                            CharacterManager player = hitInfo.collider.GetComponent<CharacterManager>();
                            if (player.IsAlive)
                            {
                                arcChosen = true;
                                Vector3 displacementToTarget = hitInfo.point - target;
                                isLob = displacementToTarget.magnitude > 0.5f;
                            }
                            else
                            {
                                rayStart = hitInfo.point;
                            }
                        }
                    }
                    else // nothing found along linecast
                    {
                        arcChosen = true;
                        isLob = false;
                    }
                } while (!arcChosen && Vector3.Dot((target - start), (target - rayStart)) > 0);

               
                if (isLob)
                {
                    //Debug.Log("Using lobbing arc");
                    arc.y = Mathf.Sin(angle1);
                }
                else
                {
                    //Debug.Log("Using direct arc");
                    arc.y = Mathf.Sin(angle2);
                }
                arc.z = Mathf.Sqrt(1 - arc.y * arc.y);
                arc *= ArcSpeed;
            }
            return arc;
        }

        //two anlges are found with method found from:
        //https://en.wikipedia.org/wiki/Trajectory_of_a_projectile#Angle_required_to_hit_coordinate_.28x.2Cy.29
        bool FindLaunchAngle(Vector3 start, Vector3 end, out float angle1, out float angle2)
        {
            Vector3 direction = end - start;
            direction.y = 0f;
            float xDist = direction.magnitude;
            float yDist = end.y - start.y;
            float gravity = Physics.gravity.y * -1;

            float temp = ArcSpeed * ArcSpeed;
            float sqrt = temp * temp - gravity * (gravity * xDist * xDist + 2 * yDist * temp);

            if (sqrt >= 0)
            {
                angle1 = Mathf.Atan((temp + Mathf.Sqrt(sqrt)) / (gravity * xDist));
                angle2 = Mathf.Atan((temp - Mathf.Sqrt(sqrt)) / (gravity * xDist));
                //Debug.Log ("Options: " + (angle1 * Mathf.Rad2Deg) + ", " + (angle2) * Mathf.Rad2Deg);
                return true;
            }
            else
            {
                //Debug.Log("no angle");
                angle1 = 0f;
                angle2 = 0f;
            }
            return false;
        }


        //Currently not used
        //ray cast downwards at intervals along a straight line between two points to find highest elevation between the two.
        //currently used to determine if there is anything in the way of the arrow shot so the steeper angle will be used to lob the arrow
        //in the future when map has a full collider mesh, may be replaced with a simple line cast between the two locations
        float FindHighestPoint(Vector2 start, Vector2 end)
        {
            Vector2 direction = end - start;
            float distance = direction.magnitude;
            direction.Normalize();
            RaycastHit rayhit;
            float highestPoint = float.NegativeInfinity;

            for (float i = 0.5f; i < distance; i += 0.5f)
            {
                Vector2 origin = start + direction * i;

                if (Physics.Raycast(new Vector3(origin.x, 20f, origin.y), Vector3.down, out rayhit, 100f, 1 << LayerMask.NameToLayer("Map")))
                {
                    if (rayhit.point.y > highestPoint)
                    {

                        highestPoint = rayhit.point.y;
                    }
                }
            }
            return highestPoint;

        }
    }
}

