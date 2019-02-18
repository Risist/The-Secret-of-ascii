using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Animations;

[RequireComponent(typeof(Animator))]
public class AnimationTest : MonoBehaviour {

    public AnimationClip clip;
    public AnimationClip clip2;

    PlayableGraph playableGraph;
    AnimationMixerPlayable mixer;
    AnimationClipPlayable clipPlayable;
    AnimationClipPlayable clipPlayable2;

    [Range(0.0f, 1.0f)]
    public float weight;


    // Use this for initialization
    void Start () {
        playableGraph = PlayableGraph.Create("My Graph");
        playableGraph.SetTimeUpdateMode(DirectorUpdateMode.GameTime);
        var playableOutput = AnimationPlayableOutput.Create(playableGraph, "animation output", GetComponent<Animator>());
        

        clipPlayable = AnimationClipPlayable.Create(playableGraph, clip);
        clipPlayable2 = AnimationClipPlayable.Create(playableGraph, clip2);
        mixer = AnimationMixerPlayable.Create(playableGraph, 2);
        
        playableOutput.SetSourcePlayable(mixer,0);
        playableGraph.Connect(clipPlayable2, 0, mixer, 0);
        playableGraph.Connect(clipPlayable, 0, mixer, 1);

        //mixer.AddInput(clipPlayable, 0);
        //mixer.AddInput(clipPlayable2, 0);

        var p = Playable.Create(playableGraph);

        // Plays the Graph.

        playableGraph.Play();

    }
    void Update()
    {
        weight = Mathf.Clamp01(weight);

        Debug.Log(mixer.GetPlayState());
        if (Input.GetKey(KeyCode.Q))
        clipPlayable2.SetDelay(5.0f);

        mixer.SetInputWeight(0, 1.0f - weight);
        mixer.SetInputWeight(1, weight);
    }

    void OnDisable()
    {
        playableGraph.Destroy();
    }
}
