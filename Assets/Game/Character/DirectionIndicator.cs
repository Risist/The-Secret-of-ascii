using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DirectionIndicator : MonoBehaviour {

    [Range(0f,1f)]
    public float fadeRatio;

    public GameObject viewfinder;
    public bool useViewFinder;

    SpriteRenderer[] renders;
    SpriteRenderer[] rendersViewFinder;
    CharacterStateController controller;
    InputManagerBase input
    {
        get
        {
            return controller.GetInput();
        }
     
    }
    float angle;
    //[System.NonSerialized]
    public float alpha;

    float distanceViewfinder;

	void Start () {
        controller = GetComponentInParent<CharacterStateController>();
        renders = GetComponentsInChildren<SpriteRenderer>();

        if (viewfinder)
        {
            distanceViewfinder = (viewfinder.transform.position - transform.position).magnitude;
            rendersViewFinder = viewfinder.GetComponentsInChildren<SpriteRenderer>();
        }
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

            if (rendersViewFinder != null)
            {
                viewfinder.transform.position = transform.parent.position + Quaternion.Euler(0, 0, angle) * Vector2.up*distanceViewfinder;
                viewfinder.transform.rotation = Quaternion.Euler(0, 0, angle);
                
                foreach (var r in rendersViewFinder)
                {
                    r.color = new Color(r.color.r, r.color.g, r.color.b, Mathf.Lerp(r.color.a, useViewFinder ? alpha : 0f, fadeRatio));
                }
            }
        }
        else
        {
            foreach (var r in renders)
            {
                r.color = new Color(r.color.r, r.color.g, r.color.b, Mathf.Lerp(r.color.a, 0, fadeRatio));
            }
            if (rendersViewFinder != null)
                foreach (var r in rendersViewFinder)
                {
                    r.color = new Color(r.color.r, r.color.g, r.color.b, Mathf.Lerp(r.color.a, 0f, fadeRatio));
                }

        }

    }
}
