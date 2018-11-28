using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RunSequenceAnimator : MonoBehaviour {

    public float animatorDelay;
    Animator[] animators;
    float timeAccumulator;
    int currentAnimator;

	// Use this for initialization
	void Start () {
        timeAccumulator = 0;
        currentAnimator = 0;
        animators = GetComponentsInChildren<Animator>();
        foreach (var it in animators)
            it.enabled = false;
	}
	
	// Update is called once per frame
	void Update () {
        timeAccumulator += Time.deltaTime;

        if (currentAnimator >= animators.Length)
            Destroy(this);
        else if(timeAccumulator - animatorDelay* (currentAnimator) > 0)
        {
            animators[currentAnimator].enabled = true;
            currentAnimator++;
            timeAccumulator -= animatorDelay;
        }
	}
}
