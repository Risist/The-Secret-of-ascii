using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PushOnCollision : MonoBehaviour {

    public float force;

    private void OnTriggerStay2D(Collider2D collision)
    {

        Vector2 toOther = collision.transform.position - transform.position;
        var rb = collision.GetComponent<Rigidbody2D>();
        if(rb)
            rb.AddForce(toOther.normalized * force);

    }
}
