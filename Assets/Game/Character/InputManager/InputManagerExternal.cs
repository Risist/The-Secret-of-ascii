using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InputManagerExternal : InputManagerBase
{
    /*
    * Input meaning: 
    * 
    * 0, 1, 2 - ordinary abilities highly depended on weapon you hold
    * 3 - dash movement with special interaction with environment
    * 4 - ability to interact with environment
    */
    public class InputData
    {
        public bool[] keys = new bool[5] { false, false, false, false, false };
        public Vector2 positionInput;
        public Vector2 directionInput;
        public Vector2 rotationInput;
    }
    protected InputData inputData = new InputData();

    public override Vector2 GetPositionInput() { return inputData.positionInput; }

    public override Vector2 GetDirectionInput()
    {
        return inputData.directionInput;
    }
    public override Vector2 GetRotationInput()
    {
        return inputData.rotationInput;
    }

    public override bool IsInputPressed(int id)
    {
        return inputData.keys[id];
    }
    public override bool IsInputDown(int id)
    {
        return inputData.keys[id];
    }
    public override bool IsInputUp(int id)
    {
        return inputData.keys[id];
    }
}