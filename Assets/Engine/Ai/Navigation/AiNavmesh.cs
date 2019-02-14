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
    public float[,] occupied;

    private void OnValidate()
    {
        occupied = new float[cellCount.x, cellCount.y];
        instance = this;

        
    }
    public void Awake()
    {
        instance = this;
        OnValidate();
        UpdateNavmesh();
    }

    [ContextMenu("UpdateNavmesh")]
    void UpdateNavmesh()
    {
        occupied = new float[cellCount.x, cellCount.y];
        var objs = FindObjectsOfType(typeof(AiNavigationObject));
        foreach (var it in objs)
        {
            for (int i = 0; i < cellCount.x; ++i) {
                for (int j = 0; j < cellCount.y; ++j)
                {
                    occupied[i, j] += 
                        ((AiNavigationObject)it).eval(GetCellPosition(i,j))
                        + Random.value * 0.025f;
                }
            }
        }
    }
    float EvaluateAllAt(Vector2 pos,Vector2 goal, AnimationCurve goalField) {
        Vector2Int cell= GetCellAt(pos);
        float valueFromMap = -occupied[cell.x, cell.y];
        float valueFromGoal = goalField.Evaluate((goal - pos).magnitude);
        return valueFromGoal + valueFromMap ;
    }
    public Vector2 EvaluateAttractionDir(Vector2 pos,Vector2 goal, AnimationCurve goalField) {
        Vector2 ret = Vector2.zero;
        float potential = Mathf.NegativeInfinity;
        for (int i = 0; i < 32; ++i) {
            Vector2 samplePoint = pos +(Vector2)( Quaternion.Euler(0, 0, i * 360 / 32)*Vector2.up);
            var q = EvaluateAllAt(samplePoint, goal, goalField);
            if (q> potential) {
                potential = q;
                ret = samplePoint;
            }
        }
        return ret - pos;

    }
    private void OnDrawGizmosSelected()
    {
       
        for (int x = 0; x < cellCount.x; ++x)
            for(int y = 0; y < cellCount.y; ++y)
            {
                Gizmos.color = Color.HSVToRGB(0.5f, 1, occupied[x, y]);
                
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
            x * cellRadius,
            y * cellRadius
        );
        return position;
    }
    public Vector2Int GetCellAt(Vector2 position)
    {
        position -= (Vector2)transform.position;  
        return new Vector2Int(Mathf.FloorToInt(position.x/cellRadius), Mathf.FloorToInt(position.y/cellRadius));
    }
}
