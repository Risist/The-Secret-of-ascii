using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkillAnimation : SkillBase {

    public string animCode;
    [Space]
    public bool stopMovement;

    public bool rotateToMouse;
    public Timer mouseRotationApplyTimer = new Timer(0f);

    new void Start () {
        base.Start();
	}
	
	void LateUpdate ()
    {
        if(!mouseRotationApplyTimer.isReady())
            ApplyRotationtoMouse();

        if (IsActivatedStart() && CastSkill())
        {
            PlayAnimation(animCode);
            PlaySound();

            mouseRotationApplyTimer.restart();

            if (stopMovement)
                movement.StopCurrentMovement();
            if(rotateToMouse && !input.IsAtMove())
                ApplyRotationtoMouse();
        }
       
    }
}
