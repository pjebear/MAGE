using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

class Actor : MonoBehaviour
{
    public Transform LeftHand;
    public Transform RightHand;
    public Transform Body;
    public Animator Animator;
    public AudioSource AudioSource;

    private void Awake()
    {
        Animator = GetComponentInChildren<Animator>();
    }
}



