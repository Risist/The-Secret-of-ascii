using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InputManagerExternal : InputManagerBase
{
    public bool[] input = new bool[4] { false, false, false, false };
    public Vector2 positionInput;
    public Vector2 directionInput;

    public override Vector2 GetPositionInput() { return positionInput; }

    public override Vector2 GetDirectionInput()
    {
        return directionInput;
    }

    public override bool IsInputPressed(int id)
    {
        return input[id];
    }
    public override bool IsInputDown(int id)
    {
        return input[id];
    }
    public override bool IsInputUp(int id)
    {
        return input[id];
    }

}