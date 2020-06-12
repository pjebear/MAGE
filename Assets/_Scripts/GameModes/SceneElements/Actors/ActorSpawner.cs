using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

class ActorSpawner : MonoBehaviour
{
    public int ActorId = -1;

    public Actor Actor = null;
    public GameObject SpawnerPlaceHolder;

    private void Start()
    {

    }

    public void Spawn()
    {
        Appearance appearance = DB.CharacterHelper.FromDB(ActorId).Appearance;
        Actor = GameModesModule.ActorLoader.CreateActor(appearance, transform);
        SpawnerPlaceHolder.gameObject.SetActive(false);
    }
}

