using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParticleInstanceController : MonoBehaviour {

    public int id;
    public int particlesPerFrame;
	
	// Update is called once per frame
	void Update () {

        var particle = ParticleTypeController.instance.particles[id];

        particle.transform.position = transform.position;
        particle.transform.rotation = Quaternion.Euler(0,0,transform.rotation.eulerAngles.z);
        particle.Emit(particlesPerFrame);
    }
}
