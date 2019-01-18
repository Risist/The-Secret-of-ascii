using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeathEventPhysicsDestructionDamage : MonoBehaviour {

    public bool active = true;
	// explosion data
	public float forceBase;
	public float forceScale;
	public float forceMax = 4500;
	public float explosionRadius;
	// remove & fadeout data
	public float removeDelay;
	// physics data
    [Range(0f,1f)]
	public float linearDamping;
    [Space]
    public float damageParticle;
    public float damageRadius;

    #region Dmg Accumulator
    [Range(0f,1f)]
    public float dmgDamping;
    public void AlterDamageAccumulator(float s) { dmgAccumulator = s; }
    float dmgAccumulator;
    public void OnReceiveDamage(HealthController.DamageData data) {
        if(data.damage < 0)
            dmgAccumulator += data.damage;
    }
    void FixedUpdate()
    {
        dmgAccumulator *= dmgDamping;
    }

    #endregion Dmg Accumulator

    Vector2 GetExplosionForce(Transform body, float explosionForce, Vector3 explosionPosition, float explosionRadius)
	{
		var dir = (body.transform.position - explosionPosition);
		float wearoff = 1 - (dir.magnitude / explosionRadius);
		return dir.normalized * explosionForce * wearoff;
	}

    public void OnDeath(HealthController.DamageData data)
	{
        if (!active)
            return;

        OnReceiveDamage(data);

		Vector2 explosionPosition = data.causer.transform.position;

		var sprites = GetComponentsInChildren<SpriteRenderer>();
		foreach(var it in sprites)
		{
			it.transform.parent = null;
            var particle = it.GetComponent<DestructionParticle>();

            if (!particle)
                particle = it.gameObject.AddComponent<DestructionParticle>();

            particle.delayTime = new Timer(removeDelay);

            particle.damping = linearDamping;
            particle.force = GetExplosionForce(it.transform, Mathf.Clamp(forceBase - forceScale * dmgAccumulator, -forceMax, forceMax), explosionPosition, explosionRadius);
            particle.Start();


            var rb = it.gameObject.AddComponent<Rigidbody2D>();
            rb.bodyType = RigidbodyType2D.Kinematic;

            var collider = it.gameObject.AddComponent<CircleCollider2D>();
            collider.isTrigger = true;
            collider.radius = damageRadius;

            var damage = it.gameObject.GetComponent<DamageOnTrigger>();
            if (!damage)
                damage = it.gameObject.AddComponent<DamageOnTrigger>();
            damage.damageEnter = damageParticle;
		}
		Destroy(this);
	}
    public void OnDeath(Vector2 explosionPosition)
    {
        if (!active)
            return;
        
        var sprites = GetComponentsInChildren<SpriteRenderer>();
        foreach (var it in sprites)
        {
            it.transform.parent = null;
            var particle = it.GetComponent<DestructionParticle>();

            if (!particle)
                particle = it.gameObject.AddComponent<DestructionParticle>();

            particle.delayTime = new Timer(removeDelay);

            particle.damping = linearDamping;
            particle.force = GetExplosionForce(it.transform, Mathf.Clamp(forceBase - forceScale * dmgAccumulator, -forceMax, forceMax), explosionPosition, explosionRadius);
            particle.Start();
        }
        Destroy(this);
    }
}
