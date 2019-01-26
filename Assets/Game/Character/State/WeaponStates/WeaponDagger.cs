using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using ReAnim;
using Character;

/*
 * Base class for all weapons
 */
public class WeaponDagger : WeaponBase
{
    /// sets up CharacterStateController data
    public override void InitWeapon(CharacterStateController ctrl)
    {
        ctrl.GetAnimator().SetTrigger("WeaponChange");
        ctrl.GetAnimator().SetInteger("WeaponId", 0);

        var state_idle = ctrl.AddState(true);        // 0
        var state_swing = ctrl.AddState();           // 1
        var state_push = ctrl.AddState();            // 2
        var state_pull = ctrl.AddState();            // 3
        var state_dash = ctrl.AddState();            // 4
        var state_throw = ctrl.AddState();           // 5
        var state_pain = ctrl.AddState();            // 6
        var state_dashBack = ctrl.AddState();        // 7
        var state_jump = ctrl.AddState();            // 8

        int cd_swing = ctrl.AddCd(0.25f);            // 1
        int cd_push = ctrl.AddCd(0.35f);             // 2
        int cd_pull = ctrl.AddCd(0.35f);             // 3
        int cd_throw = ctrl.AddCd(0.4f);             // 4
        int cd_dash = ctrl.AddCd(0.25f);             // 5
        int cd_pain = ctrl.AddCd(0.55f);             // 6

        float atackRotationSpeed = 0.45f;
        float freeRotFar = 0.25f;
        float freeRotClose = 0.25f;

        ctrl.AddTransitionAll(state_pain, new Period(0f, 1f));


        state_jump
            .AddComponent(new CStateInput(3))
            .AddComponent(new CStateAnimation("jump"))
            .AddComponent(new CStateMaxStateInstances())
            .AddComponent(new CStateCd(cd_dash, CStateCd.EMode.ERestartOnly))
            .AddComponent(new CStateDashThroughWall(0.51f, 0.95f, 0.35f, 0.185f, 0.25f))

            .AddComponent(new CStateAutoTransition(state_idle))
        //.AddTransition(state_drawArrow, new Period(0.7f))
        //.AddTransition(state_arrowHit, new Period(0.7f))
        ;
        state_swing
            .AddComponent(new CStateInput(0))
            .AddComponent(new CStateAnimation("swing"))
            .AddComponent(new CStateMaxStateInstances())
            .AddComponent(new CStateCd(cd_swing))
            .AddComponent(new CStateInitDirectionSmooth(atackRotationSpeed, freeRotClose, new Period(0, 0.65f)))
            .AddComponent(new CStateEnergy(0, 15f, CStateEnergy.Mode.EConsumeOnly))

            .AddComponent(new CStateAutoTransition(state_idle))

            //.AddTransition(state_dashBack, new Period(0.45f, 0.95f))
            .AddTransition(state_push, new Period(0.55f, 1f))
            .AddTransition(state_throw, new Period(0.45f, 0.5f))
        ;
        state_push
            .AddComponent(new CStateInput(1))
            .AddComponent(new CStateAnimation("push"))
            .AddComponent(new CStateMaxStateInstances())
            .AddComponent(new CStateCd(cd_push))
            .AddComponent(new CStateInitDirectionSmooth(atackRotationSpeed, freeRotClose, new Period(0, 0.65f)))
            .AddComponent(new CStateEnergy(0, 20f, CStateEnergy.Mode.EConsumeOnly))

            .AddComponent(new CStateAutoTransition(state_idle))

            //.AddTransition(state_dashBack, new Period(0.45f, 0.95f))
            .AddTransition(state_pull, new Period(0.375f, 0.55f))
            .AddTransition(state_swing, new Period(0.25f, 1f))
        ;
        state_pull
            .AddComponent(new CStateInput(2))
            .AddComponent(new CStateAnimation("pull"))
            .AddComponent(new CStateMaxStateInstances())
            .AddComponent(new CStateCd(cd_pull))
            .AddComponent(new CStateCd(cd_throw, CStateCd.EMode.ERestartOnly))
            .AddComponent(new CStateInitDirectionSmooth(atackRotationSpeed, freeRotClose, new Period(0, 0.65f)))
            .AddComponent(new CStateEnergy(0, 25f, CStateEnergy.Mode.EConsumeOnly))

            .AddComponent(new CStateAutoTransition(state_idle))

            .AddTransition(state_push, new Period(0.25f, 0.5f))
            .AddTransition(state_swing, new Period(0.3f, 1f))
        ;
        state_dash
            .AddComponent(new CStateInput(3))
            .AddComponent(new CStateAnimation("dash"))
            .AddComponent(new CStateMaxStateInstances())
            .AddComponent(new CStateCd(cd_dash))
            .AddComponent(new CStateInitDirectionSmooth(atackRotationSpeed, freeRotFar, new Period(0, 0.65f)))
            .AddComponent(new CStateEnergy(0, 10f, CStateEnergy.Mode.EConsumeOnly))

            .AddComponent(new CStateAutoTransition(state_idle))

            //.AddTransition(state_dashBack, new Period(0.1f, 0.75f))
            .AddTransition(state_swing, new Period(0.2f, 0.7f))
            .AddTransition(state_push, new Period(0.25f, 1f))
            .AddTransition(state_pull, new Period(0.2f, 1f))
            .AddTransition(state_jump, new Period(0.2f, 0.8f), false)
        ;
        state_dashBack
            .AddComponent(new CStateInput(3))
            .AddComponent(new CStateInput(2))
            .AddComponent(new CStateAnimation("dash-back"))
            .AddComponent(new CStateMaxStateInstances())
            .AddComponent(new CStateCd(cd_dash, CStateCd.EMode.ERestartOnly))
            .AddComponent(new CStateInitDirectionSmooth(atackRotationSpeed, freeRotFar, new Period(0, 0.65f)))
            .AddComponent(new CStateCollisionInFront(1.0f, new Vector2(-0.5f, 1.25f)))
            .AddComponent(new CStateCollisionInFront(1.0f, new Vector2(0.5f, 1.25f)))
            .AddComponent(new CStateEnergy(0, 5f, CStateEnergy.Mode.EConsumeOnly))

            .AddComponent(new CStateAutoTransition(state_idle))
        ;


        state_idle
            .AddTransition(state_swing)
            .AddTransition(state_push)
            .AddTransition(state_pull)
            .AddTransition(state_dash)
        ;
        state_throw
            .AddComponent(new CStateInput(2))
            .AddComponent(new CStateAnimation("throw"))
            .AddComponent(new CStateMaxStateInstances())
            .AddComponent(new CStateCd(cd_pull, CStateCd.EMode.ERestartOnly))
            .AddComponent(new CStateCd(cd_throw))
            .AddComponent(new CStateInitDirectionSmooth(atackRotationSpeed * 3, 1.5f, new Period(0.35f, 0.65f)))
            .AddComponent(new CStateSpawn(0, new Period(0.45f, 1f)))
            .AddComponent(new CStateEnergy(0, 15f, CStateEnergy.Mode.EConsumeOnly))

            .AddComponent(new CStateAutoTransition(state_idle))
        ;


        state_pain
            //.AddComponent(new CStateResetAnimation(new string[] { "painWall3", "painWall2", "painWall1" }))
            .AddComponent(new CStateRandomAnimation(new string[] { "pain", "pain2" }))
            .AddComponent(new CStateDamageShake())
            .AddComponent(new CStateDamage(10.0f, 0.0f))
            .AddComponent(new CStateCdReduce(
                new CStateCdReduce.CdRestartStruct[] {
                    new CStateCdReduce.CdRestartStruct(cd_swing, 0.1f),
                    new CStateCdReduce.CdRestartStruct(cd_push, 0.1f),
                    new CStateCdReduce.CdRestartStruct(cd_pull, 0.1f),
                    new CStateCdReduce.CdRestartStruct(cd_dash, -1.35f),
                }))
            .AddComponent(new CStateAutoTransition(state_idle))

        //.AddTransition(state_painWall, new Period(0f, 1f));
        ;

    }
    /// cleans up CharacterStateController data
    public override void CleanUpWeapon(CharacterStateController controller)
    {
        controller.ClearStates();
    }
}