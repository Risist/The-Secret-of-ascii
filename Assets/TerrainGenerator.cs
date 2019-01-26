using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TerrainGenerator : MonoBehaviour {

    [System.Serializable]
    public struct SpawnData
    {
        public GameObject prefab;
        public float minRadius;
        public float maxRadius;
        public int minN;
        public int maxN;
    }

    public bool spawn = false;
    public SpawnData[] spawners;
    
	void Awake () {
        if(spawn)
            Spawn();
        Destroy(this);
	}
	[ContextMenu("Spawn")]
	void SpawnI()
    {
        DespawnI();
        Spawn();
    }

    void Spawn()
    {
        foreach (var spawner in spawners)
        {
            int n = Random.Range(spawner.minN, spawner.maxN);
            for (int i = 0; i < n; ++i)
            {
                var obj = Instantiate(spawner.prefab,
                    (Vector2)transform.position + Random.insideUnitCircle * Random.Range(spawner.minRadius, spawner.maxRadius),
                    Quaternion.Euler(0, 0, Random.Range(0, 360))
                    );
                obj.transform.parent = transform;
            }

        }
    }

    [ContextMenu("Despawn")]
    void DespawnI()
    {
        while(transform.childCount > 0)
            DestroyImmediate(transform.GetChild(0).gameObject);
    }

    void Despawn()
    {
        for (int i = 0; i < transform.childCount; ++i)
            Destroy(transform.GetChild(i).gameObject);
    }
}
