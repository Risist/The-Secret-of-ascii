using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PatrolManager : MonoBehaviour {

    public static PatrolManager instance;
    private void Awake() { instance = this; }

    /// TODO : smooth
    /// 
    [System.Serializable]
    public class PatrolPath
    {
        public Vector2[] points;

        public Vector2 GetClosestPoint(Vector2 position)
        {
            float dstSq = float.PositiveInfinity;
            int id = 0;
            for (int i = 0; i < points.Length; ++i)
            {
                float d = (points[i] - position).sqrMagnitude;
                if ( d < dstSq)
                {
                    dstSq = d;
                    id = i;
                }
            }
            return points[id];
        }
        public int GetClosestPointId(Vector2 position)
        {
            float dstSq = float.PositiveInfinity;
            int id = 0;
            for (int i = 0; i < points.Length; ++i)
            {
                float d = (points[i] - position).sqrMagnitude;
                if (d < dstSq)
                {
                    dstSq = d;
                    id = i;
                }
            }
            return id;
        }
        public float GetClosestPointDistanceSq(Vector2 position)
        {
            float dstSq = float.PositiveInfinity;
            for (int i = 0; i < points.Length; ++i)
            {
                float d = (points[i] - position).sqrMagnitude;
                if (d < dstSq)
                {
                    dstSq = d;
                }
            }
            return dstSq;
        }
    }


    public PatrolPath[] paths = new PatrolPath[0];
    public PatrolPath GetRandomUnusedPath(Vector2 position, float maxDistance)
    {
        if (paths.Length == 0)
            return null;

        const int nMaxTries = 5;
        for(int i = 0; i < nMaxTries; ++i)
        {
            int id = Random.Range(0, paths.Length);
            var path = paths[id];
            float dist = path.GetClosestPointDistanceSq(position);

            if ( dist < maxDistance*maxDistance)
            {
                return path;
            }
        }
        return null;
    }

    static Color[] gizmoColors = new Color[] { Color.yellow, Color.red, Color.magenta, Color.white, Color.grey, Color.green, Color.cyan, Color.blue, Color.black };
    private void OnDrawGizmosSelected()
    {
        int id = 0;
        foreach (var path in paths)
        {
            Gizmos.color = gizmoColors[id % gizmoColors.Length];
            for(int i =0; i < path.points.Length-1; ++i)
            {
                Gizmos.DrawLine((Vector2)transform.position + path.points[i], (Vector2)transform.position + path.points[i + 1]);
            }
            ++id;
        }
    }
}
