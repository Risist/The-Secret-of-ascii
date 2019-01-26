using System.Collections;
using System.Collections.Generic;
using UnityEngine;


class AiNavmesh : MonoBehaviour
{
    public static AiNavmesh instance;
    public float cellRadius;
    public Vector2Int cellCount;

    /// ractangular table of vaues indicating whether or not given area is blocked
    [System.NonSerialized]
    public int[,] occuped;

    private void OnValidate()
    {
        occuped = new int[cellCount.x, cellCount.y];
        instance = this;

        
    }
    public void Awake()
    {
        instance = this;
        OnValidate();
    }

    [ContextMenu("UpdateNavmesh")]
    void UpdateNavmesh()
    {
        occuped = new int[cellCount.x, cellCount.y];
        var objs = FindObjectsOfType(typeof(AiNavigationObject));
        foreach (var it in objs)
        {
            ((AiNavigationObject)it).ToggleNavmesh(1);
        }
    }


    private void OnDrawGizmosSelected()
    {
        var node = GetClosestNode( Camera.main.ScreenToWorldPoint(Input.mousePosition));
        //Debug.Log("id = " + node);

        for (int x = 0; x < cellCount.x; ++x)
            for(int y = 0; y < cellCount.y; ++y)
            {
                Gizmos.color = occuped[x, y]  != 0 ? Color.red : Color.green;

                if (node.x == x && node.y == y)
                    Gizmos.color = Color.blue;
                Gizmos.DrawSphere(GetCellPosition(x,y), (cellRadius + cellRadius) * 0.1f);
            }
    }


    public Vector2 GetCellPosition(Vector2Int id)
    {
        return GetCellPosition(id.x, id.y);
    }
    public Vector2 GetCellPosition(int x, int y)
    {
        Vector2 position = transform.position;
        position += new Vector2( 
            (x - cellCount.x * 0.5f) * cellRadius,
            (y - cellCount.y * 0.5f) * cellRadius
        );
        return position;
    }


    public List<Vector2> FindPath(Vector2 from, Vector2 to)
    {
        List<Vector2> path = new List<Vector2>();

        /// find closest node related to given vector


        return path;
    }


    public Vector2Int GetMiddleNode()
    {
        return new Vector2Int(cellCount.x / 2, cellCount.y / 2);
    }
    /// Wip
    public Vector2Int GetClosestNode(Vector2 v)
    {
        Vector2 middle = (Vector2)transform.position;
        return new Vector2Int(
            (int)( (v.x - middle.x) / cellRadius) + (cellCount.x) / 2,
            (int)( (v.y - middle.y) / cellRadius) + (cellCount.y) / 2
            )
            ;
    }

    public void GetPath(ref List<Vector2> path, Vector2 from, Vector2 to)
    {
        /// TODO
        path.Add(to);
    }
    public void GetPath(ref Vector2[] path, Vector2 from, Vector2 to)
    {
        /// TODO
        path[0] = to;
    }
}
