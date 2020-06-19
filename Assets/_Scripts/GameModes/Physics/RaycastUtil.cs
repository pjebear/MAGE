using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

public enum RayCastLayer
{
    Tile = 8,
    Interactible = 9,

    NUM
}

static class RaycastUtil
{
    public static GameObject GetObjectHitByRay(Vector3 rayStart, Vector3 rayEnd, List<RayCastLayer> sortLayers)
    {
        Vector3 direction = rayEnd - rayStart;
        float distance = direction.magnitude;

        Ray ray = new Ray(rayStart, direction);
        return GetObjectHitByRay(ray, distance, sortLayers);
    }

    public static GameObject GetObjectHitByRay(Vector3 origin, Vector3 direction, float maxRange, List<RayCastLayer> sortLayers)
    {
        Ray ray = new Ray(origin, direction);
        return GetObjectHitByRay(ray, maxRange, sortLayers);
    }

    public static GameObject GetObjectHitByRay(Ray ray, float maxRange, List<RayCastLayer> sortLayers)
    {
        GameObject hit = null;

        for (int layerIndex = 0; layerIndex < sortLayers.Count; ++layerIndex)
        {
            int layerMask = 1 << (int)sortLayers[layerIndex];
            RaycastHit raycastHit;
            if (Physics.Raycast(ray, out raycastHit, maxRange, layerMask))
            {
                hit = raycastHit.collider.gameObject;
                break; // return the first hit in the priority search
            }
        }

        return hit;
    }

    public static GameObject GetObjectHitByRay(Vector3 rayStart, Vector3 rayEnd)
    {
        Vector3 direction = rayEnd - rayStart;
        float distance = direction.magnitude;

        Ray ray = new Ray(rayStart, direction);
        return GetObjectHitByRay(ray, distance);
    }

    public static GameObject GetObjectHitByRay(Vector3 origin, Vector3 direction, float maxRange)
    {
        Ray ray = new Ray(origin, direction);
        return GetObjectHitByRay(ray, maxRange);
    }

    public static GameObject GetObjectHitByRay(Ray ray, float maxRange)
    {
        GameObject hit = null;

        RaycastHit raycastHit;
        if (Physics.Raycast(ray, out raycastHit, maxRange))
        {
            hit = raycastHit.collider.gameObject;
        }

        return hit;
    }

    public static GameObject GetObjectAtPosition(Vector3 position)
    {
        GameObject objectAtPosition = null;

        Collider[] objects = Physics.OverlapCapsule(position, position, .5f);
        if (objects.Length > 0)
        {
            objectAtPosition = objects[0].gameObject;
        }

        return objectAtPosition;
    }
}

