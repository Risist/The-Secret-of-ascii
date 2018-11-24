using UnityEngine;
using System.Collections;

public class DamageOnTrigger : MonoBehaviour {

	public float damageEnter;
	public float damageStay;
	public float damageExit;
    [Space]
    public float painEnter;
    public float painStay;
    public float painExit;
    [Space]
    public bool customPain = false;
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
			healthController.DealDamage(damageEnter, customPain? painEnter : damageEnter, instigator);
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
			healthController.DealDamage(damageStay, customPain ? painStay : damageStay, instigator);
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
			healthController.DealDamage(damageExit, customPain ? painExit : damageExit, instigator);
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
			healthController.DealDamage(damageEnter, customPain ? painEnter : damageEnter, gameObject);
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
			healthController.DealDamage(damageStay, customPain ? painStay : damageStay, gameObject);
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
			healthController.DealDamage(damageExit, customPain ? painExit : damageExit, gameObject);
			if (removeOnExit)
            {
                if ((other.collider.isTrigger && removeOnTrigger) ||
                    (!other.collider.isTrigger && removeOnCollision))
                    Destroy(objToRemove);
            }
        }
	}
}
