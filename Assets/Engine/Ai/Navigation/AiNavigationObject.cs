using System.Collections;
using System.Collections.Generic;
using UnityEngine;

class AiNavigationObject : MonoBehaviour    
{
    [SerializeField]
    AnimationCurve potentialField=AnimationCurve.Linear(0,1.0f,1.0f,0.0f);

    public float eval(Vector2 at) {
        Vector2 test = at - (Vector2)transform.position;
        float value = potentialField.Evaluate(test.magnitude);
        if (value > 1) return Mathf.Infinity;
        else if (value < -1) return Mathf.NegativeInfinity;
        return value;
    }
}
