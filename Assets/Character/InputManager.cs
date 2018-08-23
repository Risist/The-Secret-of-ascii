using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InputManager : MonoBehaviour
{
    public float minimalInputStrength;

    [SerializeField]
    string xAxisCode = "Horizontal";
    [SerializeField]
    string yAxisCode = "Vertical";
    [Space]
    [SerializeField]
    string[] keyInputs = new string[4];

    Vector2 lastInput;
    bool atMove;
    public bool IsAtMove() { return atMove; }

    public Vector2 GetPositionInput()
    {
        return new Vector2(Input.GetAxisRaw(xAxisCode), Input.GetAxisRaw(yAxisCode));
    }
    public Vector2 GetLastPositionInput()
    {
        return lastInput;
    }
    public Vector2 UpdateLastPositionInput()
    {
        atMove = false;
        Vector2 input = GetPositionInput();
        if (input.sqrMagnitude > minimalInputStrength * minimalInputStrength)
        {
            lastInput = input;
            atMove = true;
        }
        return lastInput;
    }
    public void SetLastInput(Vector2 s)
    {
        lastInput = s;
    }

    public bool IsInputPressed(int id)
    {
        return Input.GetButton(keyInputs[id]);
    }
    public bool IsInputDown(int id)
    {
        return Input.GetButtonDown(keyInputs[id]);
    }
    public bool IsInputUp(int id)
    {
        return Input.GetButtonUp(keyInputs[id]);
    }

    public Vector2 GetMouseDirection()
    {
        return (Camera.main.ScreenToWorldPoint(Input.mousePosition) - transform.position).normalized;
    }

}
