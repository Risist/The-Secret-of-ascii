using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InputManagerExternal : InputManagerBase
{
    private void LateUpdate()
    {
        for (int i = 0; i < inputs.Length; ++i)
            inputs[i] = 0f;
    }


    public void SetPositionInput(Vector2 s){
        positionInput = s;
    }
    public void SetDirectionInput(Vector2 s)
    {
        directionInput = s;
    }

    public void PressInput(int id)
    {
        inputs[id] = 1f;
    }
    

    Vector2 positionInput;
    Vector2 directionInput;
    float[] inputs = new float[4];

    public override Vector2 GetPositionInput()
    {
        return positionInput;
    }

    public override Vector2 GetDirectionInput()
    {
        return directionInput;
    }

    public override bool IsInputPressed(int id)
    {
        return inputs[id] >= 1f;
    }
    public override bool IsInputDown(int id)
    {
        return false;
    }
    public override bool IsInputUp(int id)
    {
        return false;
    }

}