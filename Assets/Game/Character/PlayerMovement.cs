using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class PlayerMovement : MonoBehaviour
{
    public float movementSpeed = 1.0f;
    [Range(0.0f, 1.0f)]
    public float rotationSpeed = 1.0f;

    Rigidbody2D body;
    InputManagerBase input
    {
        get
        {
            return controller.GetInput();
        }
    }
    CharacterStateController controller;

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
    public void ApplyExternalRotation(Vector2 direction)
    {
        input.SetLastInput(direction);
        body.rotation = Vector2.SignedAngle(Vector2.up, direction);
        externalRotationApplied = true;
    }
    public void ApplyExternalRotation(float rotation)
    {
        body.rotation = rotation;
        externalRotationApplied = true;
    }
    public void ApplyRotationToDirection()
    {
        Vector2 mouseDir = input.GetDirectionInput();
        ApplyExternalRotation(Vector2.Angle(Vector2.up, mouseDir ) * (mouseDir.x > 0 ? -1 : 1));
        input.SetLastInput(mouseDir);
    }

    void Start()
    {
        body = GetComponent<Rigidbody2D>();
        controller = GetComponent<CharacterStateController>();
    }

    private void LateUpdate()
    {        
        externalRotationApplied = false;
    }

    // Update is called once per frame
    void FixedUpdate()
    {

        //animator.SetBool(atMoveTrigger, false);
        if (!enabled || !controller.GetInput())
            return;
        if (!moveToDirection)
            return;

        float velocitySq = body.velocity.sqrMagnitude;

        if (!externalRotationApplied && rotateToDirection)
        {
            Vector2 rotationInput = input.GetLastPositionInput();
            body.rotation = Mathf.LerpAngle(body.rotation, Vector2.SignedAngle(Vector2.up, rotationInput), rotationSpeed);
        }

        if (input.IsAtMove())
        {
            if (moveToDirection)
            {
                body.AddForce(input.GetLastPositionInput().normalized * movementSpeed);
            }
        }
    }
}
    