using UnityEngine;
using System.Collections;

public class DamageOnTrigger : MonoBehaviour {

	public float damageEnter;
	public float damageStay;
	public float damageExit;
    [Space]
    public float bonusPainEnter;
    public float bonusPainStay;
    public float bonusPainExit;
    [Space]
    public bool removeOnEnter = false;
	public bool removeOnExit = false;
    public bool removeOnCollision = true;
    public bool removeOnTrigger = true;
    public GameObject objToRemove;
	AiFraction myFraction;
	public GameObject instigator;

	private void Start()
	{
		if (!objToRemove)
			objToRemove = gameObject;
		if (!instigator)
			instigator = gameObject;
        myFraction = instigator.GetComponent<AiFraction>();
	}

	void OnTriggerEnter2D(Collider2D other)
	{
		if (myFraction)
		{
			var otherFraction = other.gameObject.GetComponent<AiFraction>();
			if (otherFraction && myFraction.GetAttitude(otherFraction.fractionName) == AiFraction.Attitude.friendly)
				return;
		}

		HealthController healthController = other.gameObject.GetComponent<HealthController>();
		if ( healthController != null )
        {
            HealthController.DamageData damageData = new HealthController.DamageData();
            damageData.causer = instigator;
            damageData.damage = damageEnter;
            damageData.pain = bonusPainEnter + damageEnter;
            damageData.position = transform.position;

            healthController.DealDamage(damageData);

            if (removeOnEnter)
            {
                if( (other.isTrigger && removeOnTrigger) ||
                    (!other.isTrigger && removeOnCollision))
                    Destroy(objToRemove);
            }
		}
	}

	void OnTriggerStay2D(Collider2D other)
	{
		if(myFraction)
		{
			var otherFraction = other.gameObject.GetComponent<AiFraction>();
			if (otherFraction && myFraction.GetAttitude(otherFraction.fractionName) == AiFraction.Attitude.friendly)
				return;
		}

		HealthController healthController = other.gameObject.GetComponent<HealthController>();
		if (healthController != null)
		{
            HealthController.DamageData damageData = new HealthController.DamageData();
            damageData.causer = instigator;
            damageData.damage = damageStay;
            damageData.pain = bonusPainStay + damageStay;
            damageData.position = transform.position;

            healthController.DealDamage(damageData);
		}
	}

	void OnTriggerExit2D(Collider2D other)
	{
		if (myFraction)
		{
			var otherFraction = other.gameObject.GetComponent<AiFraction>();
			if (otherFraction && myFraction.GetAttitude(otherFraction.fractionName) == AiFraction.Attitude.friendly)
				return;
		}

		HealthController healthController = other.gameObject.GetComponent<HealthController>();
		if ( healthController != null)
		{
            HealthController.DamageData damageData = new HealthController.DamageData();
            damageData.causer = instigator;
            damageData.damage = damageStay;
            damageData.pain = bonusPainExit + damageExit;
            damageData.position = transform.position;

            healthController.DealDamage(damageData);

            if (removeOnExit)
            {
                if ((other.isTrigger && removeOnTrigger) ||
                    (!other.isTrigger && removeOnCollision))
                    Destroy(objToRemove);
            }
        }
	}

	void OnCollisionEnter2D(Collision2D other)
	{
		/*if (myFraction)
		{
			var otherFraction = other.gameObject.GetComponent<AiFraction>();
			if (otherFraction && myFraction.GetAttitude(otherFraction.fractionName) == AiFraction.Attitude.friendly)
				return;
		}*/

		HealthController healthController = other.gameObject.GetComponent<HealthController>();
		if ( healthController != null )
		{
            HealthController.DamageData damageData = new HealthController.DamageData();
            damageData.causer = instigator;
            damageData.damage = damageEnter;
            damageData.pain = bonusPainEnter + damageEnter;
            damageData.position = transform.position;

            healthController.DealDamage(damageData);
            if (removeOnEnter)
            {
                if ((other.collider.isTrigger && removeOnTrigger) ||
                    (!other.collider.isTrigger && removeOnCollision))
                    Destroy(objToRemove);
            }
        }
	}

	void OnCollisionStay2D(Collision2D other)
	{
		/*if (myFraction)
		{
			var otherFraction = other.gameObject.GetComponent<AiFraction>();
			if (otherFraction && myFraction.GetAttitude(otherFraction.fractionName) == AiFraction.Attitude.friendly)
				return;
		}*/

		HealthController healthController = other.gameObject.GetComponent<HealthController>();
		if ( healthController != null )
		{
            HealthController.DamageData damageData = new HealthController.DamageData();
            damageData.causer = instigator;
            damageData.damage = damageStay;
            damageData.pain = bonusPainStay + damageStay;
            damageData.position = transform.position;

            healthController.DealDamage(damageData);
        }
	}

	void OnCollisionExit2D(Collision2D other)
	{
		/*if (myFraction)
		{
			var otherFraction = other.gameObject.GetComponent<AiFraction>();
			if (otherFraction && myFraction.GetAttitude(otherFraction.fractionName) == AiFraction.Attitude.friendly)
				return;
		}*/

		HealthController healthController = other.gameObject.GetComponent<HealthController>();
		if ( healthController != null )
		{
            HealthController.DamageData damageData = new HealthController.DamageData();
            damageData.causer = instigator;
            damageData.damage = damageExit;
            damageData.pain = bonusPainExit + damageExit;
            damageData.position = transform.position;

            healthController.DealDamage(damageData);

            if (removeOnExit)
            {
                if ((other.collider.isTrigger && removeOnTrigger) ||
                    (!other.collider.isTrigger && removeOnCollision))
                    Destroy(objToRemove);
            }
        }
	}
}
