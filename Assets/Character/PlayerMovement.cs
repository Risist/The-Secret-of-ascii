using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class PlayerMovement : MonoBehaviour
{
    public float movementSpeed = 1.0f;
    [Range(0.0f, 1.0f)]
    public float rotationSpeed = 1.0f;
    Rigidbody2D body;
    InputManager input;

    [Space]
    public bool moveToDirection = true;
    public bool rotateToDirection = true;

    public void StopCurrentMovement()
    {
        body.velocity = Vector2.zero;
    }
    public void StopCurrentRotation()
    {
        input.SetLastInput( Quaternion.Euler(0,0,body.rotation) * Vector2.up );
    }

    bool externalRotationApplied = false;
    public void ApplyExternalRotation(float rotation)
    {
        body.rotation = rotation;
        externalRotationApplied = true;
    }
    public void ApplyRotationToMouse()
    {
        Vector2 mouseDir = input.GetMouseDirection();
        ApplyExternalRotation(Vector2.Angle(Vector2.up, mouseDir ) * (mouseDir.x > 0 ? -1 : 1));
        input.SetLastInput(mouseDir);
    }

    void Start()
    {
        body = GetComponent<Rigidbody2D>();
        input = GetComponent<InputManager>();
    }

    private void LateUpdate()
    {
        if (!enabled)
            return;

        if (!externalRotationApplied && rotateToDirection)
            body.rotation = Mathf.LerpAngle(body.rotation, Vector2.Angle(Vector2.up, input.GetLastPositionInput()) * (input.GetLastPositionInput().x > 0 ? -1 : 1), rotationSpeed);

        externalRotationApplied = false;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (!enabled)
            return;
        //if (moveToDirection || rotateToDirection)
        {
            
            if (!moveToDirection)
                return;

            Vector2 _input = input.UpdateLastPositionInput();

            if (input.IsAtMove())
                body.AddForce(_input.normalized * movementSpeed);
        }
    }
}
    