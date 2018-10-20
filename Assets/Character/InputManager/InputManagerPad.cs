using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InputManagerPad : InputManagerMK {
    
    [Space]
    public int playerId = 0;
    [SerializeField]
    protected string xAxisDirCode = "Horizontal_Dir";
    [SerializeField]
    protected string yAxisDirCode = "Vertical_Dir";

    public override Vector2 GetPositionInput() {
        return new Vector2(
            Input.GetAxisRaw(xAxisCode + GetPadSuffix()), 
            Input.GetAxisRaw(yAxisCode + GetPadSuffix()));
    }
    public override Vector2 GetDirectionInput() {
         var v = new Vector2(
            Input.GetAxisRaw(xAxisDirCode + GetPadSuffix()),
            Input.GetAxisRaw(yAxisDirCode + GetPadSuffix()));
        return v;
    }

    public override bool IsInputPressed(int id)
    {
        if (id == 0)
            return Input.GetAxisRaw(keyInputs[id] + GetPadSuffix()) > 0.25f;
        if (id == 1)
            return Input.GetAxisRaw(keyInputs[id] + GetPadSuffix()) < -0.25f;
        else
            return Input.GetButton(keyInputs[id] + GetPadSuffix());
    }
    public override bool IsInputDown(int id)
    {
        if (id == 0)
            return Input.GetAxisRaw(keyInputs[id] + GetPadSuffix()) > 0.25f;
        if (id == 1)
            return Input.GetAxisRaw(keyInputs[id] + GetPadSuffix()) < -0.25f;
        else return Input.GetButton(keyInputs[id] + GetPadSuffix());
    }
    public override bool IsInputUp(int id)
    {
        if (id == 0)
            return Input.GetAxisRaw(keyInputs[id] + GetPadSuffix()) > 0.25f;
        if (id == 1)
            return Input.GetAxisRaw(keyInputs[id] + GetPadSuffix()) < -0.25f;
        else return Input.GetButton(keyInputs[id] + GetPadSuffix());
    }

    string GetPadSuffix() { return "_Pad" + playerId; }
}
