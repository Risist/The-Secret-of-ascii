using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InputManagerMK : InputManagerBase
{
    [Space]
    [SerializeField]
    protected string[] keyInputs = new string[] { "Fire1", "Fire2", "Fire3", "Movement" };
    [Space]
    [SerializeField]
    protected string xAxisCode = "Horizontal";
    [SerializeField]
    protected string yAxisCode = "Vertical";

    /// returns current position axis input
    public override Vector2 GetPositionInput() { return new Vector2(Input.GetAxisRaw(xAxisCode), Input.GetAxisRaw(yAxisCode)); }

    /// returns mouse position input (e.g. the direction character is targeting to, the direction character will shoot or strike)
    public override Vector2 GetDirectionInput()
    {
        return (Camera.main.ScreenToWorldPoint(Input.mousePosition) - transform.position);
    }

    public override bool IsInputPressed(int id)
    {
        return Input.GetButton(keyInputs[id]);
    }
    public override bool IsInputDown(int id)
    {
        return Input.GetButtonDown(keyInputs[id]);
    }
    public override bool IsInputUp(int id)
    {
        return Input.GetButtonUp(keyInputs[id]);
    }

}
