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
    Terrain = 10,
    Trees = 14,

    NUM
}

static class RaycastUtil
{
    public static Vector3 GetRayCastHit(Vector3 origin, Vector3 direction, float maxRange, List<RayCastLayer> sortLayers)
    {
        Vector3 hit = Vector3.zero;

        Ray ray = new Ray(origin, direction);

        for (int layerIndex = 0; layerIndex < sortLayers.Count; ++layerIndex)
        {
            int layerMask = 1 << (int)sortLayers[layerIndex];
            RaycastHit raycastHit;
            if (Physics.Raycast(ray, out raycastHit, maxRange, layerMask))
            {
                hit = raycastHit.point;
                break; // return the first hit in the priority search
            }
        }

        return hit;
    }

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

    public static List<GameObject> GetObjectsBetweenPoints(Vector3 p1, Vector3 p2, float range)
    {
        return Physics.OverlapCapsule(p1, p2, range).Select(x => x.gameObject).ToList();
    }

    public static List<RaycastHit> GetObjectsHitByRay(Ray ray, float maxRange)
    {
        return Physics.RaycastAll(ray, maxRange).ToList();
    }

    public static List<RaycastHit> GetObjectsHitByCursor()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

        return GetObjectsHitByRay(ray, 100);
    }
}

