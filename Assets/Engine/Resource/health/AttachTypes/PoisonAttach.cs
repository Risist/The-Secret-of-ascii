using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PoisonAttach : AttachBase
{
	public Timer applyCd;
	public float damage;
	HealthController parentHealth;

	protected new void Start()
	{
		base.Start();
		parentHealth = transform.parent.GetComponent<HealthController>();
	}

	protected new void Update()
	{
		if(applyCd.IsReadyRestart())
		{
            HealthController.DamageData damageData = new HealthController.DamageData();
            damageData.causer = gameObject;
            damageData.damage = damage;
            damageData.pain = damage;
            damageData.position = transform.position;

            parentHealth.DealDamage(damageData);
		}
		base.Update();
	}
}
