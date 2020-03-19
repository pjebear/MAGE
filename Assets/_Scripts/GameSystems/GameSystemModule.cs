using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

class GameSystemModule : MonoBehaviour
{
    public static GameSystem Instance;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = new GameSystem();
        }
    }
}

