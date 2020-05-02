using System.Collections;
using System.Collections.Generic;
using UnityEngine;

class Level : MonoBehaviour
{
    public LevelId LevelId;
    
    public Terrain Terrain;
    public TileContainer Tiles;
    public Transform SpawnPoint;

    private void Awake()
    {
        if (!Tiles.gameObject.activeSelf)
        {
            Tiles.gameObject.SetActive(true);
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        if (LevelManager.Instance != null)
        {
            LevelManager.Instance.NotifyLevelLoaded(this);
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
