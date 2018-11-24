using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class ParticleEmmiter : MonoBehaviour {


    public int particleId;
    public ParticleSystem particle;
    public float nParticlesPerUpdate;
    float nParticleAccumulator;

    // Use this for initialization
    void Start () {
        if (!particle)
            particle = GameObject.FindGameObjectWithTag("GameController").GetComponent<ParticleTypeController>().particles[particleId];
    }

    void FixedUpdate () {
        if (!enabled )
            return;

        nParticleAccumulator += nParticlesPerUpdate;
        if (nParticleAccumulator >= 1)
        {
            int n = (int)nParticleAccumulator;
            nParticleAccumulator -= n;

            particle.transform.position = transform.position;
            particle.transform.rotation = transform.rotation;
            particle.Emit(n);
        }
    }
}
