using System.Collections;
using System.Collections.Generic;
using UnityEngine;


/*
 * Todo: version which makes an ai only aware about an event around
 */
public class AiPerceptionPain : AiPerceptionBase
{
    public void OnReceiveDamage(HealthController.DamageData data)
    {
        if (!data.causer)
            return;
        var unit = data.causer.GetComponentInParent<AiPerceiveUnit>();
        if (!unit)
            return;

        holder.InsertToMemory(unit, (transform.position - unit.transform.position).sqrMagnitude);
    }
}