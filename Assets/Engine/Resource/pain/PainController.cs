using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PainController : ResourceController
{
    public float baseAnimatorSpeed = 1.0f;
    public float dependedAnimatorSpeed = 0.0f;
    public float painResistance = 1.0f;
    Animator animator;
    void Start()
    {
        animator = GetComponent<Animator>();

    }
    new void Update()
    {
        base.Update();
        animator.speed = baseAnimatorSpeed + actual / max * dependedAnimatorSpeed ;
    }

    void OnReceiveDamage(HealthController.DamageData data)
    {
        if (data.pain < 0)
            actual += data.pain*painResistance;
    }
    public void OnDeath(HealthController.DamageData data)
    {
        OnReceiveDamage(data);
    }
}
