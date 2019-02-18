using System.Collections;
using System.Collections.Generic;
using UnityEngine;


/*
 * Todo: version which makes an ai only aware about an event around
 */
public class AiPerceptionPain : AiPerceptionBase
{
    public enum EPropagateMode
    {
        ENone,
        EUnitOnly,
        EAll
    }
    [Space]
    public GameObject propatationPrefab;
    public EPropagateMode propagateMode = EPropagateMode.ENone;
    public float propagateRadius;
    public float matureTimeMaxOffset;
    public float knowledgeTimeMaxOffset;
    [Range(0f,1f)]
    public float propagateChance;
    public Timer tPropagate;
    [Range(0f, 1f)]
    public float propagateChanceSpawn;

    public void OnReceiveDamage(HealthController.DamageData data)
    {
        /*var unit = data.causer.GetComponentInParent<AiPerceiveUnit>();
        if (unit)
        {
            //add "unless the unit is friendly" - this would mean that it was by mistake
            // or unless the unit is from same fraction - fun effect that if player hits the friendly agent he can hit him back
            holder.InsertToMemory(unit, EMemoryEvent.EEnemy, data.position, memoryTime, matureTime, shadeTime);
            if (propagateMode == EPropagateMode.EAll)
                Propagate(data, unit);
        }
        else*/
        {   
            holder.InsertToMemory(EMemoryEvent.EEnemy_Pain, data.position, Vector2.zero, memoryTime, matureTime, shadeTime);
            if (propagateMode != EPropagateMode.ENone && Random.value <= propagateChanceSpawn && tPropagate.IsReadyRestart()) 
                Propagate(data);
        }
    }

    void Propagate(HealthController.DamageData data, AiPerceiveUnit unit)
    {
        var colliders = Physics2D.OverlapCircleAll(transform.position, propagateRadius);
        foreach (var it in colliders)
            if(Random.value <= propagateChance)
        {
            var holder = it.GetComponentInChildren<AiPerceptionHolder>();
            if (holder)
            {
                holder.InsertToMemory(unit, EMemoryEvent.ENoise, data.position, Vector2.zero, 
                    memoryTime, matureTime + matureTimeMaxOffset * Random.value, shadeTime);
            }
        }
        if (propatationPrefab)
            Instantiate(propatationPrefab, transform.position, Quaternion.identity);
    }
    void Propagate(HealthController.DamageData data)
    {
        var colliders = Physics2D.OverlapCircleAll(transform.position, propagateRadius);
        foreach (var it in colliders)
            if(Random.value <= propagateChance)
        {
            var holder = it.GetComponentInChildren<AiPerceptionHolder>();
            if (holder)
            {
                holder.InsertToMemory(EMemoryEvent.ENoise, data.position, Vector2.zero,
                    memoryTime + knowledgeTimeMaxOffset * Random.value, matureTime + matureTimeMaxOffset * Random.value, shadeTime);
            }
        }
        if(propatationPrefab)
            Instantiate(propatationPrefab, transform.position, Quaternion.identity);
    }
}