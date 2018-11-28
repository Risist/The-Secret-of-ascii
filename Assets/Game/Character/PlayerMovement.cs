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
    CharacterController controller;


    [Space]
    public bool moveToDirection = true;
    public bool rotateToDirection = true;
    public bool trackMouseRotation = true;

    //public string atMoveTrigger = "AtMove";
    //Animator animator;

    [Space]
    public float speedBeginDrag;
    public float speedDragIncreaseRatio;
    float baseDrag;

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
    public void ApplyRotationToDirection()
    {
        Vector2 mouseDir = input.GetDirectionInput();
        ApplyExternalRotation(Vector2.Angle(Vector2.up, mouseDir ) * (mouseDir.x > 0 ? -1 : 1));
        input.SetLastInput(mouseDir);
    }

    void Start()
    {
        body = GetComponent<Rigidbody2D>();
        //animator = GetComponent<Animator>();
        controller = GetComponent<CharacterController>();
        baseDrag = body.drag;
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
        float desiredDrag = baseDrag + 
            Mathf.Clamp(velocitySq - speedBeginDrag * speedBeginDrag, 0, float.PositiveInfinity)
            * speedDragIncreaseRatio;
        body.drag = desiredDrag;    



        if (!externalRotationApplied && rotateToDirection)
        {
            Vector2 rotationInput = input.GetLastPositionInput();
            body.rotation = Mathf.LerpAngle(body.rotation, Vector2.Angle(Vector2.up, rotationInput) * (rotationInput.x > 0 ? -1 : 1), rotationSpeed);
        }

        if (input.IsAtMove())
        {
            if (moveToDirection)
            {
                //animator.SetBool(atMoveTrigger, true);
                body.AddForce(input.GetLastPositionInput().normalized * movementSpeed);
            }
        }
        else if (trackMouseRotation)
            input.SetLastInput(input.GetDirectionInput().normalized);
    }
}
    