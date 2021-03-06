﻿using UnityEngine;
using System.Collections;

/*
 * Two kinds of events:
 *      death event (method "OnReceiveDamage" with HealthController.DamageData argument) - called once the character is considered as dead
 *      damage event (method "OnDeath" with HealthController.DamageData argument) - called everytime character takes damage, unless those damage are fatal
 *      
 * 
 * 
 */

public class HealthController : ResourceController
{
	// perform removing of owner at OnDeath event?
	public bool removeAfterDeath = true;
	// object to remove after death
	// in case you should remove at death something else than owner of the script
	// if not set up the value is assigned to an owner of the script 
	public GameObject objectToRemove;
    public bool IsAlive()
    {
        return actual > 0;
    }

	protected void Start()
	{
		if (removeAfterDeath && !objectToRemove)
			objectToRemove = gameObject;
        if(regeneration == 0)
            enabled = false;
	}

	protected new void Update()
	{
        //base.Update();
        //Gain(regeneration * Time.deltaTime);

        actual = actual + regeneration * Time.deltaTime;
        if (actual > max)
            actual = max;
        else if (!IsAlive())
            BroadcastMessage("OnDeath", new DamageData(transform.position, regeneration, gameObject));
    }

	/// struct for broadcasting messages
	public struct DamageData
    {
        public DamageData(Vector2 _position, float _d, GameObject _o)
        {
            damage = _d;
            causer = _o;
            pain = _d;
            position = _position;
        }
        public DamageData(Vector2 _position, float _d, float _p = 0, GameObject _o = null)
        {
            damage = _d;
            causer = _o;
            pain = _p;
            position = _position;
        }

        public float damage;
        public float pain;

        public Vector2 position;

        public GameObject causer;
    }

    /*public void DealDamage(float damage, float pain, GameObject causer = null)
    {
        var data = new DamageData(Vector2.zero, damage, pain, causer);
        DealDamage(data);
    }
    public void DealDamage(float damage, GameObject causer = null)
    {
        DealDamage(damage, damage, causer);
    }*/
    public void DealDamage(DamageData data)
    {
        actual = Mathf.Clamp(actual + data.damage, -max, max);

        if (IsAlive())
            BroadcastMessage("OnReceiveDamage", data);
        else
            BroadcastMessage("OnDeath", data);
    }

    public void OnReceiveDamage(DamageData data){}
	public void OnDeath(DamageData data)
	{
		if(objectToRemove && removeAfterDeath)
			Destroy(objectToRemove);
	}

}