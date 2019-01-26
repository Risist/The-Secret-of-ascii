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
        public bool[] input = new bool[5] { false, false, false, false, false };
        public Vector2 positionInput;
        public Vector2 directionInput;
        public Vector2 rotationInput;
    }
    [System.NonSerialized]
    public bool[] input = new bool[5] { false, false, false, false, false };
    [System.NonSerialized]
    public Vector2 positionInput;
    [System.NonSerialized]
    public Vector2 directionInput;
    [System.NonSerialized]
    public Vector2 rotationInput;

    public override Vector2 GetPositionInput() { return positionInput; }

    public override Vector2 GetDirectionInput()
    {
        return directionInput;
    }
    public override Vector2 GetRotationInput()
    {
        return rotationInput;
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