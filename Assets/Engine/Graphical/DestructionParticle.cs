using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DestructionParticle : MonoBehaviour {

    public Vector2 force;
    [Range(0,1f)]
    public float damping;
    public Timer delayTime;

    new SpriteRenderer renderer;
    float initialColorAlpha;

	// Use this for initialization
	public void Start () {
        renderer = GetComponent<SpriteRenderer>();
        delayTime.restart();
        initialColorAlpha = renderer.color.a;
	}
    void Update()
    {
        if (delayTime.isReady())
            Destroy(gameObject);
        else
        {
            float alpha = renderer.color.a - initialColorAlpha * Time.deltaTime / delayTime.cd;
            renderer.color = new Color(renderer.color.r, renderer.color.g, renderer.color.b, alpha);
        }
    }

    // Update is called once per frame
    void FixedUpdate ()
    {
        transform.position += (Vector3)force*Time.fixedDeltaTime;
        force *= damping;
	}
}
