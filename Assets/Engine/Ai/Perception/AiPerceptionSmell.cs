﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;



public class AiPerceptionSmell : AiPerceptionBase
{
    public float radius;
    [Range(0f,1f)]
    public float chance = 1.0f;

    void Update()
    {
        if(timerPerformSearch.isReadyRestart())
        {
            PerformClear();
            PerformSearch();
        }
    }

    static Collider2D[] colliders = new Collider2D[50];
    public void PerformSearch()
    {
        int n =  Physics2D.OverlapCircleNonAlloc(transform.position, radius, colliders);

        bool any = false;
        for(int i = 0; i < n; ++i)
        {
            var unit = colliders[i].GetComponent<AiPerceiveUnit>();
            if (unit && Random.value < chance && InsertToMemory(unit, (transform.position - unit.transform.position).sqrMagnitude))
                any = true;
        }
        if (any)
            SortMemory();
    }
}