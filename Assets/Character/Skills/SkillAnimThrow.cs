using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ReAnim;

public class SkillAnimThrow : SkillAnimation {

    public GameObject prefab;
    public Transform spawnpoint;
    [Range(0f, 1f)] public float spawnTime;
    bool thrown;

    public override void InitPlayback(Transition transition)
    {
        base.InitPlayback(transition);
    }
    public override void OnAnimationBeggin(AnimatorStateInfo stateInfo)
    {
        base.OnAnimationBeggin(stateInfo);
        thrown = false;
    }
    public override void OnAnimationUpdate(AnimatorStateInfo stateInfo)
    {
        base.OnAnimationUpdate(stateInfo);
        if ( !thrown && stateInfo.normalizedTime >= spawnTime)
        {
            Instantiate(prefab, spawnpoint.position, spawnpoint.rotation);
            thrown = true;
        }
    }
    public override void OnAnimationEnd(AnimatorStateInfo stateInfo)
    {
        base.OnAnimationEnd(stateInfo);
        skillManager.ResetInputBuffer();
    }

}
