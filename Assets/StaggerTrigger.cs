using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StaggerTrigger : MonoBehaviour {

    public float appliedPainValue;
    public int stabilityLevel = 0;
    HealthController hp;

	// Use this for initialization
	void Start () {
        hp = GetComponentInParent<HealthController>();
	}

    private void OnTriggerEnter2D(Collider2D collision)
    {
        var trigger = collision.gameObject.GetComponent<StaggerTrigger>();
        if (!trigger) return;


        if (stabilityLevel <= trigger.stabilityLevel)
        {
            Debug.Log("applied-trigger");
            trigger.hp.DealDamage(0, appliedPainValue, hp.gameObject);
        }
    }
    private void OnCollisionEnter2D(Collision2D collision)
    {
        var trigger = collision.gameObject.GetComponent<StaggerTrigger>();
        if (!trigger) return;

        if (stabilityLevel <= trigger.stabilityLevel)
        {
            Debug.Log("applied-collision");
            trigger.hp.DealDamage(0, appliedPainValue, hp.gameObject);
        }
    }


}
