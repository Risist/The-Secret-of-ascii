using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParticleTypeController : MonoBehaviour
{
    public static ParticleTypeController instance;
    private void Awake()
    {
        instance = this;
    }
    public ParticleSystem[] particles;
}
