using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class F_PlayerMovement : MonoBehaviour {

    public float movementSpeed;
    public float dashMovementSpeed;
    public Timer dashTime;
    public Timer dashCd;

    public float rotationSpeed;
    public float rotationSpeedAtack;
    public float maxRotationInput;
    Rigidbody2D body;

	// Use this for initialization
	void Start () {
        body = GetComponent<Rigidbody2D>();

        dashTime.restart();
        dashCd.restart();
	}

    private void Update()
    {
        if(Input.GetButton("Movement") )//&& dashCd.isReadyRestart())
        {
            dashTime.restart();
        }


    }

    private void FixedUpdate()
    {
        float _rotationSpeed = Input.GetButton("Fire1") || Input.GetButton("Fire2") ? rotationSpeedAtack : rotationSpeed;
        float _movementSpeed = movementSpeed;

        Debug.Log(Input.GetAxisRaw("Vertical"));

        body.AddForce(
            (transform.up       *   Input.GetAxisRaw("Vertical")  +
            transform.right    *   Input.GetAxisRaw("Horizontal") ).normalized * _movementSpeed  
            );
        
        body.AddTorque(-Mathf.Clamp(Input.GetAxisRaw("Mouse X"), -maxRotationInput, maxRotationInput) * _rotationSpeed);

    }
}
