using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkillBase : MonoBehaviour {

    public int keyId;
    public float energyCost;
    public Timer cd;

    protected EnergyController resource;
    protected InputManager input;
    protected Animator animator;
    protected new AudioSource audio;
    protected PlayerMovement movement;

    protected void Start()
    {
        resource = GetComponentInParent<EnergyController>();
        input = GetComponentInParent<InputManager>();
        animator = GetComponentInParent<Animator>();
        audio = GetComponentInParent<AudioSource>();
        movement = GetComponentInParent<PlayerMovement>();
    }

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
            animator.SetTrigger(animCode);
    }

    protected bool IsActivatedStart()
    {
        return input.IsInputDown(keyId);
    }
    protected bool IsActivated()
    {
        return input.IsInputPressed(keyId);
    }
    protected bool IsActivatedEnd()
    {
        return input.IsInputDown(keyId);
    }

    protected bool CastSkill()
    {
        if(cd.isReady() && resource.HasEnough(energyCost))
        {
            resource.Spend(energyCost);
            cd.restart();
            return true;
        }
        return false;
    }

    protected Vector2 ApplyRotationtoMouse()
    {
        movement.ApplyRotationToMouse();
        return input.GetLastPositionInput();
    }
    protected void OverrideRotation(float rotation)
    {
        movement.ApplyExternalRotation(rotation);
    }
    protected void OverrideRotation(Vector2 _input)
    {
        input.SetLastInput(_input);
        movement.ApplyExternalRotation(Vector2.Angle(Vector2.up, _input) * (_input.x > 0 ? -1 : 1));
    }
    protected void TurnMovement(bool b, bool stopCurrentMovement = false)
    {
        movement.moveToDirection = b;

        if (!b && stopCurrentMovement)
            movement.StopCurrentMovement();
    }
    protected void TurnRotation(bool b, bool stopCurrentRotation = false)
    {
        movement.rotateToDirection = b;

        if (!b && stopCurrentRotation)
            movement.StopCurrentRotation();
    }
    protected void ApplyForce(Vector2 force)
    {
        ///TODO
    }


}
