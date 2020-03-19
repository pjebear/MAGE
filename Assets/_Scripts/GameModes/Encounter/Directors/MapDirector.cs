using System.Collections;
using System.Collections.Generic;
using UnityEngine;

class MapDirector : MonoBehaviour
{
    private static MapDirector Instance;

    public Map MapPrefab;
    public int Width;
    public int Length;

    public Map Map;

    private void Awake()
    {
        Map = Instantiate(MapPrefab);
        Map.Width = Width;
        Map.Length = Length;
        Map.Initialize();

        Instance = this;
    }
}
