using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ReAnim;

/*
 * dash - instant rotation, locks rotation, if keyboard key is pressed rotates to keyboard direction, otherwise towards mouse
 * swing - locks rotation at initial state. Rotates towards mouse always. allows movement
 * push - locks rotation at initial state.
 */


public class SkillAnimation : SkillBase {

    public string animCode;
    [Space]
    public EOverrideDirection preferDirection = EOverrideDirection.EPrefferDirection;
    EOverrideDirection preferDirectionRuntime;

    new void Start () {
        base.Start();
	}

    public override void InitPlayback(Transition transition)
    {
        base.InitPlayback(transition);
        PlayAnimation(animCode);


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
            (/*preferDirectionRuntime == EOverrideDirection.EPrefferMovement &&*/ !input.IsAtMove()) )
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
