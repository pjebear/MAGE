using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

enum ProjectilePathType
{
    Linear,
    Arc,

    NUM
}

enum ProjectileId
{
    Arrow,
    FireBall,

    NUM
}

class ProjectileSpawnParams
{
    public ProjectileId ProjectileId;
    public ProjectilePathType PathType = ProjectilePathType.Linear;
    public Vector3 SpawnPoint = Vector3.zero;
    public Vector3 EndPoint = Vector3.zero;
    public Vector3 InitialForward = Vector3.forward;
    public float InitialVelocity = 1;
    public float FlightDuration = 0;

    public GameObject CollisionWith = null;
}



