using System.Collections;
using System.Collections.Generic;
using UnityEngine;

class AiNavigationObject : MonoBehaviour    
{
    private void Start()
    {
        Debug.Assert(AiNavmesh.instance, "No AiNavmesh on scene");
        ToggleNavmesh(1);
    }
    private void OnEnable()
    {
        ToggleNavmesh(1);
    }
    private void OnDisable()
    {
        ToggleNavmesh(-1);
    }
    private void OnDestroy()
    {
        ToggleNavmesh(-1);
    }

    public void ToggleNavmesh(int value)
    {
        if (!AiNavmesh.instance)
            return;

        var colliders = GetComponentsInChildren<Collider2D>();
        foreach (var it in colliders)
        {
            for (int x = 0; x < AiNavmesh.instance.cellCount.x; ++x)
                for (int y = 0; y < AiNavmesh.instance.cellCount.y; ++y)
                {
                    if (it.OverlapPoint(AiNavmesh.instance.GetCellPosition(x, y)))
                        AiNavmesh.instance.occuped[x, y] += value;
                }
        }
    }
}
