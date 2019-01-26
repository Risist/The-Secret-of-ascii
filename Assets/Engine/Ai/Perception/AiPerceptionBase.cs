using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class AiPerceptionBase : MonoBehaviour
{

    [System.NonSerialized]
    public AiPerceptionHolder holder;

    [System.NonSerialized]
    public AiPerceiveUnit myUnit;
    
    public float shadeTime = 5f;
    public float memoryTime = 5f;
    public float matureTime = 0f;
    [Space]
    public float priority = 1.0f;

    private void Start()
    {
        holder = GetComponent<AiPerceptionHolder>();
        myUnit = GetComponentInParent<AiPerceiveUnit>();
    }
}