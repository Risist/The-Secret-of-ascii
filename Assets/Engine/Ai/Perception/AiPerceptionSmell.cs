using System.Collections;
using System.Collections.Generic;
using UnityEngine;



public class AiPerceptionSmell : AiPerceptionBase
{
    [Space]
    public float radius;
    public float minWaitTime;
    public float maxWaitTime;
    Timer tLastSearch = new Timer();
    [Range(0f,1f)]
    public float chance = 1.0f;

    void Update()
    {
        if((tLastSearch.IsReady(minWaitTime) && Random.value < chance)
            || tLastSearch.IsReady(maxWaitTime))
        {
            tLastSearch.Restart();
            PerformSearch();
        }
    }

    static Collider2D[] colliders = new Collider2D[50];
    public void PerformSearch()
    {
        int n =  Physics2D.OverlapCircleNonAlloc(transform.position, radius, colliders);

        for(int i = 0; i < n; ++i)
        {
            var unit = colliders[i].GetComponent<AiPerceiveUnit>();
            if (unit && unit != myUnit)
            {
                holder.InsertToMemory(unit, EMemoryEvent.ENoise, unit.transform.position, 1f,
                    memoryTime, matureTime, shadeTime, priority);
            }
        }
    }
}