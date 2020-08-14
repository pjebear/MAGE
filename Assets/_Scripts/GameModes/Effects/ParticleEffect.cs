using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

class ParticleEffect : MonoBehaviour
{
    public float EffectDuration = 2f;
    public float EffectInteractionPoint = 1.0f;

    private void Awake()
    {
        Destroy(gameObject, EffectDuration);
    }
  
}

