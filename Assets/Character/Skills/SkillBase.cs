using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using ReAnim;

public abstract class SkillBase : MonoBehaviour {

    protected EnergyController resource;
    protected InputManagerBase input;
    protected Animator animator;
    protected new AudioSource audio;
    protected PlayerMovement movement;
    protected SkillManagament skillManager;
    [NonSerialized] public bool bufferedInput = false;

    /// TODO move to dictionary when performance will be a bottle neck
    public Transition[] transitions;

    public int keyId;
    public float energyCost;
    public Timer cd = new Timer();
    public bool canBePlayedFromIdle = true;

    public Transition GetTransition(SkillBase skill)
    {
        foreach (var it in transitions)
            if (it.target == skill)
                return it;
        return null;
    }

   

    

    protected void Start()
    {
        resource = GetComponentInParent<EnergyController>();
        input = GetComponentInParent<InputManagerBase>();
        animator = GetComponentInParent<Animator>();
        audio = GetComponentInParent<AudioSource>();
        movement = GetComponentInParent<PlayerMovement>();
        skillManager = GetComponentInParent<SkillManagament>();
    }

    ///////////////////////////////////////////////////

    public virtual bool CanEnter()
    {
        return input.IsInputDown(keyId) && cd.isReady();
    }
    public virtual void InitPlayback(Transition transition)
    {
        resource.SpendClamp(energyCost);
        cd.restart();
        skillManager.appliedAnimationCount++;
        skillManager.SetCurrentSkill(this);
    }

    public virtual void OnAnimationBeggin(AnimatorStateInfo stateInfo) { }
    public virtual void OnAnimationUpdate(AnimatorStateInfo stateInfo) { }
    public virtual void OnAnimationEnd(AnimatorStateInfo stateInfo)    { skillManager.appliedAnimationCount--; }
    



    //////////////////////////////////////////////////////////////////

    protected void PlaySound()
    {
        if (audio)
            audio.Play();
    }
    protected void PlayAnimation(int animCode)
    {
        if (animator)
            animator.SetTrigger(animCode);
    }
    protected void PlayAnimation(string animCode)
    {
        if (animator)
        {
            animator.SetTrigger(animCode);
        }
    }
}
