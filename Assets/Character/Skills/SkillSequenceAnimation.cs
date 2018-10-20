using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ReAnim;

public class SkillSequenceAnimation : SkillBase
{

    public string[] animCodes;
    public Timer cdRestartSequence;
    
    [Space]
    public EOverrideDirection preferDirection = EOverrideDirection.EPrefferDirection;
    EOverrideDirection preferDirectionRuntime;
    int currentAnim = 0;
    


    public override void InitPlayback(Transition transition)
    {
        base.InitPlayback(transition);
        

        PlayAnimation(animCodes[currentAnim]);
        currentAnim = (currentAnim + 1) % animCodes.Length;

        Debug.Log(animCodes[currentAnim]);

        /*if (transition != null)
        {
            preferDirectionRuntime = transition.overrideDirection;
        }
        else*/
        preferDirectionRuntime = preferDirection;
    }

    public override void OnAnimationBeggin(AnimatorStateInfo stateInfo)
    {
        base.OnAnimationBeggin(stateInfo);
        PlaySound();
        if (preferDirectionRuntime == EOverrideDirection.EPrefferDirection ||
            (/*preferDirectionRuntime == EOverrideDirection.EPrefferMovement &&*/ !input.IsAtMove()))
        {
            movement.ApplyRotationToDirection();
        }
    }
    public override void OnAnimationUpdate(AnimatorStateInfo stateInfo)
    {
        base.OnAnimationUpdate(stateInfo);
    }
    public override void OnAnimationEnd(AnimatorStateInfo stateInfo)
    {
        base.OnAnimationEnd(stateInfo);
    }
}
