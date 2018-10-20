using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DirectionIndicator : MonoBehaviour {

    [Range(0f,1f)]
    public float fadeRatio;

    SpriteRenderer[] renders;
    InputManagerBase input;
    float position;
    float angle;
    float alpha;
	// Use this for initialization
	void Start () {
        input = GetComponentInParent<InputManagerBase>();
        position = transform.parent.position.y - transform.position.y;
        renders = GetComponentsInChildren<SpriteRenderer>();
        alpha = renders[0].color.a;
	}
	
	// Update is called once per frame
	void Update () {
        if (!transform.parent)
            return;

        if (input.isDirectionInputApplied())
        {
            angle = -Vector2.SignedAngle(input.GetDirectionInput(), Vector2.up);
            transform.position = transform.parent.position + Quaternion.Euler(0, 0, angle) * Vector2.up;
            transform.rotation = Quaternion.Euler(0, 0, angle + 90);
            foreach(var r in renders)
            {
                r.color = new Color(r.color.r, r.color.g, r.color.b, Mathf.Lerp(r.color.a, alpha, fadeRatio)); 
            }
        }
        else
        {
            foreach (var r in renders)
            {
                r.color = new Color(r.color.r, r.color.g, r.color.b, Mathf.Lerp(r.color.a, 0, fadeRatio));
            }
        }

    }
}
