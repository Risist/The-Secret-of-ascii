using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DamageEventParticle : MonoBehaviour {

    public int particleId;
	public ParticleSystem particle;
	public Timer emitCd;
	public int minParticles;
	public float damageScale = 1.0f;
	public float minimumDamage = 0;

	public int minParticlesDeath;
	public float damageScaleDeath = 1.0f;
    CharacterStateController character;

	private void Start()
	{
        if (!particle)
            particle = ParticleTypeController.instance.particles[particleId];
    
        character = GetComponentInParent<CharacterStateController>();
	}

	void OnReceiveDamage(HealthController.DamageData data)
	{
        float damage = character ? character.GetDamageAccumulator() : data.damage;

		if ( damage < minimumDamage && emitCd.isReadyRestart() )
		{
			int n = minParticles + (int)(-damage * damageScale);
			particle.transform.position = transform.position;
			particle.transform.rotation = transform.rotation;
			particle.Emit(n);
		}
	}

	void OnDeath(HealthController.DamageData data)
	{
        float damage = character ? character.GetDamageAccumulator() : data.damage;

        if (particle)
		{
			int n = minParticlesDeath + (int)(-damage * damageScaleDeath);
			particle.transform.position = transform.position;
			particle.transform.rotation = transform.rotation;
			particle.Emit(n);
		}
	}
}
