using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeflectionArea : MonoBehaviour {

    public int pose;
    public bool agressive;
    CharacterStateController controller;
    new Collider2D collider;

    int anim = 0;

    public int GetAnim() { return anim; }
    public int GetAnimReset()
    {
        var v = anim;
        anim = 0;
        return v;
    }

	// Use this for initialization
	void Start () {
        controller = GetComponentInParent<CharacterStateController>();
        collider = GetComponent<Collider2D>();
	}


    private void OnTriggerStay2D(Collider2D collision)
    {

        var other = collision.GetComponent<DeflectionArea>();
        if (!other || !controller)
            return;

        if (!agressive && !other.agressive)
            return;

        if (other.pose < pose)
            return;

        /*Vector2 a = -transform.right;
        Vector2 b = Vector2.zero;


        var contactFilter = new ContactFilter2D();
        contactFilter.NoFilter();
        ContactPoint2D[] contacts = new ContactPoint2D[5];
        int n = Physics2D.GetContacts(collider, collision, contactFilter, contacts);
        for (int i = 0; i < n; ++i)
            b += contacts[i].point;

        b /= n;
        b -= (Vector2)transform.position;

        
        var cosAngle = Vector2.Dot(a, b);
        anim = cosAngle > 0.0f ? 1 : -1;*/
        anim = 1;

    }
}
