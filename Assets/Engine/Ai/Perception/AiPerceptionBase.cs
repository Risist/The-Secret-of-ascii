using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class AiPerceptionBase : MonoBehaviour
{

    protected AiPerceptionHolder holder;
    protected AiPerceiveUnit myUnit;

    private void Start()
    {
        holder = GetComponent<AiPerceptionHolder>();
        myUnit = GetComponentInParent<AiPerceiveUnit>();
    }
}