﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

class ProjectileDirector : MonoBehaviour
{
    public const float LINEAR_VELOCITY = 10;
    public const float ARC_VELOCITY_START = 9;
    public Vector3 VertOffset = Vector3.up * 1.3f;
    private AssetLoader<GameObject> mProjectileLoader = null;

    public void Init()
    {
        mProjectileLoader = new AssetLoader<GameObject>("Props/Projectiles");
        mProjectileLoader.LoadAssets();
    }

    public void Cleanup()
    {

    }

    public void SpawnProjectile(ProjectileSpawnParams spawnParams)
    {
        ProjectileController projectile = Instantiate(mProjectileLoader.GetAsset(spawnParams.ProjectileId.ToString())).GetComponent<ProjectileController>();
        projectile.transform.position = spawnParams.SpawnPoint;

        projectile.Init(spawnParams.InitialForward, spawnParams.InitialVelocity, spawnParams.PathType == ProjectilePathType.Arc, spawnParams.FlightDuration);
    }

    public ProjectileSpawnParams GenerateSpawnParams(Tile start, Tile end, ProjectilePathType pathType, ProjectileId projectileId)
    {
        ProjectileSpawnParams spawnParams = null;
        if (pathType == ProjectilePathType.Linear)
        {
            spawnParams = GenerateLinearProjectileParams(start, end);
        }
        else
        {
            spawnParams = GenerateArcProjectileParams(start, end);
        }

        spawnParams.ProjectileId = projectileId;

        return spawnParams;
    }

    private ProjectileSpawnParams GenerateLinearProjectileParams(Tile start, Tile end)
    {
        ProjectileSpawnParams linearParams = new ProjectileSpawnParams();

        Vector3 spawnPosition = start.transform.position + VertOffset;
        Vector3 endPosition = end.transform.position + VertOffset;

        linearParams.PathType = ProjectilePathType.Linear;
        linearParams.SpawnPoint = spawnPosition;
        Vector3 trajectory = (endPosition - spawnPosition);
        linearParams.InitialForward = trajectory.normalized;
        linearParams.InitialVelocity = LINEAR_VELOCITY;
        linearParams.EndPoint = endPosition;

        linearParams.CollisionWith = RaycastUtil.GetObjectHitByRay(spawnPosition, trajectory, trajectory.magnitude);
        if (linearParams.CollisionWith != null)
        {
            linearParams.EndPoint = linearParams.CollisionWith.transform.position;
        }

        linearParams.FlightDuration = (linearParams.EndPoint - linearParams.SpawnPoint).magnitude / linearParams.InitialVelocity;

        return linearParams;
    }

    private ProjectileSpawnParams GenerateArcProjectileParams(Tile start, Tile end)
    {
        ProjectileSpawnParams arcParams = new ProjectileSpawnParams();

        Vector3 spawnPosition = start.transform.position + VertOffset;
        Vector3 endPosition = end.transform.position + VertOffset;

        arcParams.PathType = ProjectilePathType.Arc;
        arcParams.SpawnPoint = spawnPosition;

        Vector3 linearTrajectory = (endPosition - spawnPosition);
        Vector3 horzTrajectory = linearTrajectory;
        horzTrajectory.y = 0;
        float horzDistance = horzTrajectory.magnitude;
        horzTrajectory.Normalize();

        GameObject collisionAlongArc = null;
        float shootAngle = 0;
        float speed = ARC_VELOCITY_START;
        int safteyBreak = 100;

        bool foundSolution = false;
        while (!foundSolution && safteyBreak >= 0)
        {
            List<float> angles = GenerateArcPathAngles(spawnPosition, endPosition, speed);
            for (int i = 0; i < angles.Count; ++i)
            {
                shootAngle = angles[i];

                float vertLobSpeed = speed * Mathf.Sin(shootAngle * Mathf.Deg2Rad);
                float horzLobSpeed = speed * Mathf.Cos(shootAngle * Mathf.Deg2Rad);

                Vector3 lobTrajectory = Vector3.up * vertLobSpeed + horzTrajectory * horzLobSpeed;
                float lobDuration = horzDistance / horzLobSpeed;
                collisionAlongArc = ArcMarch(spawnPosition, endPosition, lobTrajectory, lobDuration);

                bool emptyTileAndNoCollisions = (end.OnTile == null && collisionAlongArc == null);
                bool filledTileAndCorrectCollision = (end.OnTile != null && collisionAlongArc != null && collisionAlongArc.GetComponent<EncounterActorController>() == end.OnTile);
                if (emptyTileAndNoCollisions || filledTileAndCorrectCollision)
                {
                    Logger.Log(LogTag.GameModes, "ProjectileDirector", string.Format("Found solution for Lob arc. Angle[{0}] Speed[{1}]", shootAngle, speed));
                    foundSolution = true;
                    break;
                }
            }

            if (!foundSolution)
            {
                speed += 1;
                safteyBreak--;
            }
        }

        if (!foundSolution)
        {
            Logger.Log(LogTag.GameModes, "ProjectileDirector", "Failed to find solution for projectile arc");

            shootAngle = 45;
            speed = ARC_VELOCITY_START;
            collisionAlongArc = null;
        }


        float vertSpeed = speed * Mathf.Sin(shootAngle * Mathf.Deg2Rad);
        float horzSpeed = speed * Mathf.Cos(shootAngle * Mathf.Deg2Rad);

        Vector3 trajectory = Vector3.up * vertSpeed + horzTrajectory * horzSpeed;

        arcParams.InitialForward = trajectory.normalized;
        arcParams.InitialVelocity = speed;
        arcParams.EndPoint = endPosition;

        arcParams.FlightDuration = horzDistance / horzSpeed;
        arcParams.CollisionWith = collisionAlongArc;

        return arcParams;
    }

    List<float> GenerateArcPathAngles(Vector3 startPoint, Vector3 endPoint, float speed)
    {
        List<float> arcPathAngles = new List<float>();
       
        Vector3 displacementToEnd = endPoint - startPoint;
        float verticalDistance = -displacementToEnd.y;
        displacementToEnd.y = 0;
        float horizontalDistance = displacementToEnd.magnitude;
        float gravity = Physics.gravity.y;

        float innerSquaredTerm = gravity * Mathf.Pow(horizontalDistance, 2) + 2 * verticalDistance * Mathf.Pow(speed, 2);
        float squaredComponent = Mathf.Pow(speed, 4) - gravity * innerSquaredTerm;
        if (squaredComponent >= 0)
        {
            float denominator = gravity * horizontalDistance;
            float rootComponent = Mathf.Sqrt(squaredComponent);
            float angle1 = -Mathf.Atan((Mathf.Pow(speed, 2) - rootComponent) / denominator) / Mathf.Deg2Rad;
            float angle2 = -Mathf.Atan((Mathf.Pow(speed, 2) + rootComponent) / denominator) / Mathf.Deg2Rad;
            float shallowAngle = angle1 < angle2 ? angle1 : angle2;
            float lobAngle = angle2 < angle1 ? angle1 : angle2;

            if (shallowAngle > -30)
            {
                arcPathAngles.Add(shallowAngle);
            }
            
            if (lobAngle > 0)
            {
                arcPathAngles.Add(lobAngle);
            }
        }

        return arcPathAngles;
    }

    GameObject ArcMarch(Vector3 startingPos, Vector3 endPos, Vector3 initialVelocity, float duration)
    {
        GameObject objectAlongArc = null;

        float timeStep = duration / 10; // ...

        float cumTime = 0;
        Vector3 horzVelocity = initialVelocity;
        horzVelocity.y = 0;

        float vertVelocity = initialVelocity.y;
        float gravity = Physics.gravity.y;

        Vector3 currentArcPos = startingPos;
        while ((cumTime - duration) < timeStep)
        {
            if (cumTime > duration)
            {
                cumTime = duration;
            }
            float deltaHeight = vertVelocity * cumTime + .5f * gravity * Mathf.Pow(cumTime, 2);

            Vector3 nextArcPosition = startingPos + horzVelocity * cumTime + Vector3.up * deltaHeight;
            objectAlongArc = RaycastUtil.GetObjectHitByRay(currentArcPos, nextArcPosition);
            if (objectAlongArc != null)
            {
                break;
            }
            currentArcPos = nextArcPosition;
            cumTime += timeStep;
        }

        return objectAlongArc;
    }
}
