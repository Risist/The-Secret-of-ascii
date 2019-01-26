using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class InputManagerBase : MonoBehaviour {

    public float minimalPositionInputStrength = 0.1f;
    public float minimalDirectionInputStrength = 0.1f;
    public float minimalRotationInputStrength = 0.1f;

    protected Vector2 lastPositionInput;
    protected bool atMove;

    public bool IsAtMove() { return atMove; }

    /// returns current position axis input
    public abstract Vector2 GetPositionInput();
    /// returns last non-zero position axis input
    public Vector2 GetLastPositionInput()
    {
        return lastPositionInput;
    }
    /// sets externaly last position input
    public void SetLastInput(Vector2 s) { lastPositionInput = s; }
    /// updates last position input and returns it
    void UpdateLastPositionInput()
    {
        atMove = false;
        Vector2 input = GetPositionInput();
        if (input.sqrMagnitude > minimalPositionInputStrength * minimalPositionInputStrength)
        {
            lastPositionInput = input;
            atMove = true;
        }
    }

    /// returns mouse position input (e.g. the direction character is targeting to, the direction character will shoot or strike)
    public abstract Vector2 GetDirectionInput();
    
    public virtual Vector2 GetRotationInput() { return Vector2.zero; }

    public bool isDirectionInputApplied()
    {
        return GetDirectionInput().sqrMagnitude > minimalDirectionInputStrength * minimalDirectionInputStrength;
    }

    public abstract bool IsInputPressed(int id);
    public abstract bool IsInputDown(int id);
    public abstract bool IsInputUp(int id);


    protected void Start()
    {
        Rigidbody2D rb = GetComponentInParent<Rigidbody2D>();
        if(rb)
            lastPositionInput = rb.transform.up;
    }
    protected void Update()
    {
        UpdateLastPositionInput();
    }
}
