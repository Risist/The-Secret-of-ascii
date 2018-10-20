using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DamageEventShake : MonoBehaviour {

	public float shakePositionBase = 0.0f;
	public float shakePositionDamage = 1.0f;

	public float shakeRotationBase = 0.0f;
	public float shakeRotationDamage = 1.0f;

	public float shakeScaleBase = 0.0f;
	public float shakeScaleDamage = 1.0f;

    [Space]
	///
	public float shakePositionBaseDeath = 0.0f;
	public float shakePositionDamageDeath = 0.0f;

	public float shakeRotationBaseDeath = 0.0f;
	public float shakeRotationDamageDeath = 0.0f;

	public float shakeScaleBaseDeath = 0.0f;
	public float shakeScaleDamageDeath = 0.0f;

	MultiCameraController controller;

    [Space]
    [Range(0,1)]
    public float damageDamping = 0.95f;
	float damageAcumulator = 0;


	// Use this for initialization
	void Start () {
		controller = Camera.main.GetComponent<MultiCameraController>();
	}

    void OnReceiveDamage(HealthController.DamageData data)
    {
        if (data.damage < 0)
            damageAcumulator += data.damage;
    }

    public void shake()
	{
		if (damageAcumulator < 0)
		{
			controller.shakePosition(	Random.insideUnitCircle.normalized	* (shakePositionDamage + shakePositionDamage	* damageAcumulator));
			controller.shakeRotation(	Random.Range(-1, 1)					* (shakeRotationDamage + shakeRotationDamage	* damageAcumulator));
			controller.shakeScale(		Random.Range(-1, 1)					* (shakeScaleDamage	+ shakeScaleDamage		* damageAcumulator));
		}
	}

	public void OnDeath(HealthController.DamageData data)
	{
		if (data.damage < 0)
            damageAcumulator += data.damage;

        {
            controller.shakePosition(Random.insideUnitCircle.normalized * (shakePositionDamageDeath + shakePositionDamageDeath * damageAcumulator));
			controller.shakeRotation(Random.Range(-1, 1) * (shakeRotationDamage + shakeRotationDamageDeath * damageAcumulator));
			controller.shakeScale(Random.Range(-1, 1) * (shakeScaleDamageDeath + shakeScaleDamageDeath * damageAcumulator));
		}
	}

	private void OnDestroy()
	{
		controller.shakePosition(Random.insideUnitCircle.normalized * (shakePositionDamageDeath + shakePositionDamageDeath * damageAcumulator));
		controller.shakeRotation(Random.Range(-1, 1) * (shakeRotationDamage + shakeRotationDamageDeath * damageAcumulator));
		controller.shakeScale(Random.Range(-1, 1) * (shakeScaleDamageDeath + shakeScaleDamageDeath * damageAcumulator));
	}

    private void FixedUpdate()
    {
        damageAcumulator *= damageDamping;
    }
}
