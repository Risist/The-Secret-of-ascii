using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeflectionArea : MonoBehaviour {

    public int pose;
    CharacterStateController controller;

    int anim = 0;

    public int GetAnim()
    {
        var v = anim;
        anim = 0;
        return v;
    }

	// Use this for initialization
	void Start () {
        controller = GetComponentInParent<CharacterStateController>();
	}


    private void OnTriggerEnter2D(Collider2D collision)
    {
        var other = collision.GetComponent<DeflectionArea>();
        if (!other)
            return;

        Vector2 toOther = other.transform.position - controller.transform.position;
        var cosAngle = Vector2.Dot(controller.transform.right, toOther);

        anim = cosAngle > 0.0f ? 1 : -1;
    }
}
