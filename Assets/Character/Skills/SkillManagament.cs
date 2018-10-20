using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using ReAnim;


public class SkillManagament : MonoBehaviour
{
    [NonSerialized]
    public int appliedAnimationCount;
    [SerializeField]
    SkillBase[] skills;
    Animator animator;
    SkillBase currentSkill;

    public string[] idleAnimations;

    public void SetCurrentSkill(SkillBase skill) { currentSkill = skill; }
    public SkillBase GetSkill(int id)
    {
        Debug.Assert(id < skills.Length);
        return skills[id];
    }

    public void ResetInputBuffer()
    {
        foreach (var it in skills)
            it.bufferedInput = false;
    }



    private void Start()
    {
        animator = GetComponent<Animator>();

        // temporary initial value to not get nullptr
        currentSkill = skills[0];
    }
    

    private void LateUpdate()
    {
        var stateInfo = animator.GetCurrentAnimatorStateInfo(0);
        float time = stateInfo.normalizedTime;

        foreach (var it in skills)
            if ( ( it.bufferedInput || it.CanEnter() ) && appliedAnimationCount <= 1)
            {
                bool b = time >= 1.0f;
                foreach (var anim in idleAnimations)
                    b |= stateInfo.IsName(anim);

                if ( b)
                {
                    if (it.canBePlayedFromIdle)
                    {
                        var t = currentSkill.GetTransition(it);
                        it.bufferedInput = false;
                        it.InitPlayback(t);
                    }
                }
                else
                {
                    var t = currentSkill.GetTransition(it);
                    if (t != null)
                    {
                        var p = t.period;

                        if (p.IsIn(time))
                        {
                            it.bufferedInput = false;
                            it.InitPlayback(t);
                        }
                        else if (time < p.min)
                            it.bufferedInput = true;
                        // else ignore
                    }
                }
            }
    }

}
