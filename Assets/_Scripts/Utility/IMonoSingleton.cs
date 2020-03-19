using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

abstract class IMonoSingleton<SingletonType> : MonoBehaviour where SingletonType : MonoBehaviour
{
    public static SingletonType Instance;

    protected abstract void Init();
    private void Awake()
    {
        Instance = GetComponent<SingletonType>();
        // TODO: Do 'IsAlive' check
        Init();
    }
}

