using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
 * when character touches trigger area spring joint is connected to the character 
 * thus the character can pull the track
 * 
 * when character touches area again is freed
 * there is also a break force - should allow to pull while moving but when you dash/ ect break
 */
public class TrackPull : MonoBehaviour {

    Rigidbody2D body;
    public CharacterStateController controller;


    public Timer tPull;
    public float distance;
    public float breakDistance;
    public float forceDistanceRatioMy;
    public float forceDistanceRatioOther;
    [Range(0f,1f)]
    public float otherSlow;
    private void Start()
    {
        body = GetComponentInParent<Rigidbody2D>();
        Debug.Assert(body);
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.isTrigger)
            return;

        if (!collision.attachedRigidbody)
            return;

        

        /// only marked objects can interact
        var _controller = collision.attachedRigidbody.GetComponent<CharacterStateController>();

        if (!_controller )
            return;

        if (controller && _controller.GetInput().IsInputUp(4) && tPull.IsReadyRestart())
        {
            controller = null;
        }
        else if(!controller && _controller.GetInput().IsInputDown(4) && tPull.IsReadyRestart() )
            controller = _controller;

    }

    private void FixedUpdate()
    {
        if (!controller)
            return;

        var otherBody = controller.GetBody();

        Vector2 toOther = (Vector2)transform.position - otherBody.position;

        if (toOther.sqrMagnitude < distance * distance)
            return;

        if (toOther.sqrMagnitude > breakDistance * breakDistance)
        {
            controller = null;
            return;
        }

        body.AddForceAtPosition(toOther * -forceDistanceRatioMy,transform.position);
        otherBody.AddForce( toOther * forceDistanceRatioOther);
        otherBody.velocity = otherBody.velocity * otherSlow;
    }
}
