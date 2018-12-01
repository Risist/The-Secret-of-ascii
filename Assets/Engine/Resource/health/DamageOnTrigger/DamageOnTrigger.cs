using UnityEngine;
using System.Collections;

public class DamageOnTrigger : MonoBehaviour {

	public float damageEnter;
	public float damageStay;
	public float damageExit;
    [Space]
    public float bunusPainEnter;
    public float bonusPainStay;
    public float bunusPainExit;
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
			healthController.DealDamage(damageEnter, bunusPainEnter + damageEnter, instigator);
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
			healthController.DealDamage(damageStay, bonusPainStay + damageStay, instigator);
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
			healthController.DealDamage(damageExit, bunusPainExit + damageExit, instigator);
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
			healthController.DealDamage(damageEnter, bunusPainEnter + damageEnter, gameObject);
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
			healthController.DealDamage(damageStay, bonusPainStay + damageStay, gameObject);
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
			healthController.DealDamage(damageExit, bunusPainExit + damageExit, gameObject);
			if (removeOnExit)
            {
                if ((other.collider.isTrigger && removeOnTrigger) ||
                    (!other.collider.isTrigger && removeOnCollision))
                    Destroy(objToRemove);
            }
        }
	}
}
