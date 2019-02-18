using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// Generates noise request to all AiPerceptionHolder components around
/// Noise Generation situation:
/// - got damaged
/// - got pushed too hard
class AiNoiseSource : MonoBehaviour
{
    static Timer tPropagate = new Timer(0.4f);

    public GameObject notificationPrefab;
    [Space]

    public float shadeTime = 5f;
    public float memoryTime = 5f;
    public float matureTime = 0f;
    [Space]
    public float priority;
    [Space]

    /// how often callback can be cend out
    public Timer tInsert;
    /// how likely it is to propagate the event onto specific holder?
    [Range(0f, 1f)]
    public float propagateChance;

    #region Push
    [Space]
    public float pushRadiusRatio = 1f;
    public float minimalPushSpeed;
    public float pushPedictionScale = 1f;

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (!tInsert.IsReady() || !collision.rigidbody)
            return;

        if (collision.relativeVelocity.sqrMagnitude >= minimalPushSpeed * minimalPushSpeed && tInsert.IsReady() && tPropagate.IsReadyRestart())
        {
            Propagate(collision.collider.transform.position, -collision.relativeVelocity*pushPedictionScale, collision.relativeVelocity.magnitude*pushRadiusRatio);
            tInsert.Restart();
        }
    }
    #endregion Push

    #region Damage
    [Space]
    public float damageRadiusRatio = 0.1f;
    /// minimal damage in damageAccumulator needed to Propagate noise
    public float minimalDamage;
    /// how much damage accumulator will be decreased in one second
    public float damageAccumulatorDecrease;
    /// records current damage dealed to the object in last couple of frames
    float damageAccumulator;

    private void Update()
    {
        damageAccumulator += damageAccumulatorDecrease;
        damageAccumulator = damageAccumulator <= 0 ? damageAccumulator : 0;
    }
    public void OnReceiveDamage(HealthController.DamageData data)
    {
        if (data.damage >= 0)
            return;
        damageAccumulator += data.damage + data.pain;
        
        if(damageAccumulator < minimalDamage && tInsert.IsReady() && tPropagate.IsReadyRestart())
        {
            tInsert.Restart();
            Propagate(data.position, Vector2.zero, -data.damage*damageRadiusRatio);
        }
    }
    public void OnDeath(HealthController.DamageData data)
    {
        damageAccumulator += (data.damage + data.pain)*4;

        if (damageAccumulator < minimalDamage && tPropagate.IsReadyRestart())
        {
            Propagate(data.position, Vector2.zero, -data.damage * damageRadiusRatio);
        }
    }
    #endregion Damage


    static CircleCollider2D[] colliders = new CircleCollider2D[100];
    void Propagate(Vector2 position, Vector2 direction, float callbackRadius)
    {
        int n = Physics2D.OverlapCircleNonAlloc(transform.position, callbackRadius, colliders);
        for(int i = 0;i < n; ++i)
        {
            var holder = colliders[i].GetComponentInChildren<AiPerceptionHolder>();
            if(holder && Random.value <= propagateChance)
                holder.InsertToMemory(EMemoryEvent.ENoise, position, direction,
                    memoryTime, matureTime, shadeTime, priority
                );
        }
        if (notificationPrefab)
            Instantiate(notificationPrefab, transform.position, Quaternion.identity);
    }

    
}

