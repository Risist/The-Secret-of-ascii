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

	new MultiCameraController camera;
    CharacterController controller;
    

	// Use this for initialization
	void Start () {
		camera = Camera.main.GetComponent<MultiCameraController>();
        controller = GetComponentInParent<CharacterController>();
	}

    public void shake()
	{
		if (controller.GetDamageAccumulator() < 0)
		{
			camera.shakePosition(	Random.insideUnitCircle.normalized	* (shakePositionDamage + shakePositionDamage	* controller.GetDamageAccumulator()));
			camera.shakeRotation(	Random.Range(-1, 1)					* (shakeRotationDamage + shakeRotationDamage	* controller.GetDamageAccumulator()));
			camera.shakeScale(		Random.Range(-1, 1)					* (shakeScaleDamage	+ shakeScaleDamage		* controller.GetDamageAccumulator()));
		}
	}

	public void OnDeath(HealthController.DamageData data)
	{
        camera.shakePosition(Random.insideUnitCircle.normalized * (shakePositionDamageDeath + shakePositionDamageDeath * controller.GetDamageAccumulator()));
        camera.shakeRotation(Random.Range(-1, 1) * (shakeRotationDamage + shakeRotationDamageDeath * controller.GetDamageAccumulator()));
        camera.shakeScale(Random.Range(-1, 1) * (shakeScaleDamageDeath + shakeScaleDamageDeath * controller.GetDamageAccumulator()));
    }

    private void OnDestroy()
	{
		camera.shakePosition(Random.insideUnitCircle.normalized * (shakePositionDamageDeath + shakePositionDamageDeath * controller.GetDamageAccumulator()));
		camera.shakeRotation(Random.Range(-1, 1) * (shakeRotationDamage + shakeRotationDamageDeath * controller.GetDamageAccumulator()));
		camera.shakeScale(Random.Range(-1, 1) * (shakeScaleDamageDeath + shakeScaleDamageDeath * controller.GetDamageAccumulator()));
	}
}
