﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

class ParticleEffect : MonoBehaviour
{
    public float EffectDuration = 2f;

    private float mLifeTime = 0f;

    private void Update()
    {
        mLifeTime += Time.deltaTime;
        if (mLifeTime > EffectDuration)
        {
            Destroy(gameObject);
        }
    }
}

