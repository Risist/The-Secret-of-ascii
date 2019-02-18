using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ReAi;

class AiTestObstacleAvoidance : InputManagerExternal
{
    [SerializeField]
    AnimationCurve goalField= AnimationCurve.Linear(0, 1.0f, 1.0f, 0.0f);
    new private void Update()
    {
        Vector2 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        Vector2 desired = AiNavmesh.instance.EvaluateAttractionDir(transform.position, mousePos, goalField);
        float l = 0.1f;
        inputData.positionInput = inputData.positionInput * (1 - l) + desired * l;
        inputData.directionInput = desired;//inputData.positionInput;

        //inputData.keys[1] = true;
        //inputData.keys[3] = true;

        Debug.Log(inputData.positionInput);
        base.Update();
    }
}
