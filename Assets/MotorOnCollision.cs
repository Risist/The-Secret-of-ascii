using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MotorOnCollision : MonoBehaviour {

    public new Rigidbody2D rigidbody;
    public float movementSpeed;
    bool b = true;

    CharacterStateController controller;
    private void Start()
    {
        if (rigidbody == null)
            rigidbody = GetComponentInParent<Rigidbody2D>();
    }
    private void OnEnable()
    {
        b = false;
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        if (!controller && collision.isTrigger)
            return;
        b = true;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        /*if (b && rigidbody)
            rigidbody.AddForce(
                new Vector2(transform.up.x, transform.up.y)
                * movementSpeed
        );*/
    }
}
