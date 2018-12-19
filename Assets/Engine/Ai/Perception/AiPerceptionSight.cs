using System.Collections;
using System.Collections.Generic;
using UnityEngine;



public class AiPerceptionSight : AiPerceptionBase
{
    public float coneRadius = 170.0f;
    public float searchDistance = 5.0f;
    public float addictionalRotation = 0.0f;

    void Update()
    {
        PerformSearch();
    }
    public void PerformSearch()
    {
        float angleOffset = coneRadius *Random.value;
        
        var rays = Physics2D.RaycastAll(transform.position, Quaternion.Euler(0, 0, -coneRadius * 0.5f + angleOffset + addictionalRotation) * transform.up, searchDistance);
        Debug.DrawRay(transform.position, Quaternion.Euler(0, 0, -coneRadius * 0.5f + angleOffset + addictionalRotation) * transform.up * searchDistance, Color.green, 0.25f);

        var rayList = new List<RaycastHit2D>(rays);
        rayList.Sort(delegate (RaycastHit2D item1, RaycastHit2D item2) { return item1.distance.CompareTo(item2.distance); });

        foreach (var it in rayList)
        {
            var unit = it.collider.GetComponent<AiPerceiveUnit>();
            if (unit && unit != myUnit)
            {
                holder.InsertToMemory(unit, it.distance);

                if (unit.blocksVision)
                    break;
            }
        }
    }
}