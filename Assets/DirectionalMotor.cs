using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DirectionalMotor : MonoBehaviour {

    CharacterStateController controller;
    public float movementSpeed;


    // Use this for initialization
    void Start()
    {
        controller = GetComponentInParent<CharacterStateController>();
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        var rb = controller.GetBody();
        if (rb)
        {
            var input = controller.GetInput();
            var v = input.GetDirectionInput();

            if (input.isDirectionInputApplied())
                rb.AddForce(
                    input.GetDirectionInput().normalized
                        * movementSpeed);
            else
                rb.AddForce(
                    new Vector2(rb.transform.up.x, rb.transform.up.y)
                        * movementSpeed);
        }
    }

}
