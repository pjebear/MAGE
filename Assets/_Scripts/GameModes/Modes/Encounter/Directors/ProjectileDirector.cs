using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

class ProjectileDirector : MonoBehaviour
{
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

    
}

