using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ModulesContainer : MonoBehaviour
{
    static ModulesContainer Container = null;

    private void Awake()
    {
        if (Container != null)
        {
            DestroyImmediate(gameObject);
        }
        else
        {
            Container = this;
            DontDestroyOnLoad(gameObject);
        }
    }
}
