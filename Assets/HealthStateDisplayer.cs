using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealthStateDisplayer : MonoBehaviour {

    HealthController controller;
    new SpriteRenderer renderer;
    Color color;

	// Use this for initialization
	void Start () {
        controller = GetComponentInParent<HealthController>();
        renderer = GetComponent<SpriteRenderer>();
        color = renderer.color;
	}
	
	// Update is called once per frame
	void Update () {
        float f = (controller.actual / controller.max);
        renderer.color = new Color(color.r * f, color.g * f, color.b * f, color.a);
	}
}
