using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using ReAnim;
using Character;

/*
 * Base class for all weapons
 */
public class WeaponBow : WeaponBase
{
    /// sets up CharacterStateController data
    public override void InitWeapon(CharacterStateController ctrl)
    {
        ctrl.GetAnimator().SetTrigger("WeaponChange");
        ctrl.GetAnimator().SetInteger("WeaponId", 2);

        var state_idle = ctrl.AddState(true);        // 0
        var state_pain = ctrl.AddState();            // 1
        var state_dash = ctrl.AddState();            // 2
        var state_dashBack = ctrl.AddState();        // 3
        var state_bowHit = ctrl.AddState();          // 4
        var state_arrowHit = ctrl.AddState();        // 5
        var state_loadArrow = ctrl.AddState();       // 6

        var state_drawArrow = ctrl.AddState();       // 7
        var state_holdArrow = ctrl.AddState();       // 8
        var state_releaseArrow = ctrl.AddState();    // 9
        var state_shootArrow = ctrl.AddState();      // 10
        var state_releaseArrow_fake = ctrl.AddState();// 11
        var state_jump = ctrl.AddState();            // 12


        int cd_dash = ctrl.AddCd(0.3f);
        int cd_dash_long = ctrl.AddCd(0.5f);
        int cd_bowHit = ctrl.AddCd(0.3f);
        int cd_shoot = ctrl.AddCd(0.3f);
        int cd_loadArrow = ctrl.AddCd(0.35f);


        const float atackRotationSpeed = 0.45f;
        const float arrowHoldRotationSpeed = 0.15f;
        const float freeRotFar = 0.25f;
        const float freeRotClose = 0.25f;

        ctrl.AddTransitionAll(state_pain, new Period(0f, 1f));




        state_idle
            .AddComponent(new CStateAnimationAtMove("AtMove"))

            .AddTransition(state_dash)
            .AddTransition(state_bowHit)
            .AddTransition(state_drawArrow)
            .AddTransition(state_loadArrow)
        ;

        state_drawArrow
            .AddComponent(new CStateCanUseArrows())
            .AddComponent(new CStateInput(0))
            .AddComponent(new CStateAnimation("drawArrow"))
            .AddComponent(new CStateMaxStateInstances())
            .AddComponent(new CStateInitDirectionSmooth(atackRotationSpeed * 0.75f, freeRotClose, new Period(0, 0.25f)))
            .AddComponent(new CStateInitDirectionUpdate(arrowHoldRotationSpeed, freeRotClose, 8.75f, new Period(0.25f, float.PositiveInfinity)))
            .AddComponent(new CStateCd(cd_shoot))

            .AddComponent(new CStateAutoTransition(state_holdArrow))

            .AddTransition(state_bowHit, new Period(0.5f))
            .AddTransition(state_dash, new Period(0.5f))
            .AddTransition(state_releaseArrow_fake, new Period(0.35f))
        ;
        state_holdArrow
            .AddComponent(new CStateMaxStateInstances())
            .AddComponent(new CStateInitDirectionUpdate(arrowHoldRotationSpeed, freeRotClose, 7.75f, new Period(0, float.PositiveInfinity)))

            .AddTransition(state_bowHit)
            .AddTransition(state_dash)
            .AddTransition(state_releaseArrow)
            .AddTransition(state_shootArrow)
        ;
        state_releaseArrow
             .AddComponent(new CStateInput(2))
             .AddComponent(new CStateAnimation("releaseArrow"))
             .AddComponent(new CStateMaxStateInstances())

             .AddComponent(new CStateAutoTransition(state_idle))

            .AddTransition(state_bowHit, new Period(0.85f))
            .AddTransition(state_dash, new Period(0.85f))
         ;

        state_releaseArrow_fake
             .AddComponent(new CStateInput(0, true))
             .AddComponent(new CStateAnimation("releaseArrow"))
             .AddComponent(new CStateMaxStateInstances())

             .AddComponent(new CStateAutoTransition(state_idle))

             .AddTransition(state_bowHit, new Period(0.85f))
             .AddTransition(state_dash, new Period(0.85f))
         ;
        state_shootArrow
             .AddComponent(new CStateUseArrows(0.45f))
             .AddComponent(new CStateInput(0, true))
             .AddComponent(new CStateAnimation("shootArrow"))
             .AddComponent(new CStateMaxStateInstances())
             .AddComponent(new CStateCd(cd_shoot, CStateCd.EMode.ERestartOnly))

             .AddComponent(new CStateAutoTransition(state_idle))
         ;

        state_loadArrow
            .AddComponent(new CStateInput(2))
            .AddComponent(new CStateAnimation("loadArrow"))
            .AddComponent(new CStateMaxStateInstances())
            .AddComponent(new CStateLoadArrow(0.55f))
            .AddComponent(new CStateCd(cd_loadArrow))
            //.AddComponent(new CStateInitDirectionSmooth(atackRotationSpeed, freeRotClose, new Period(0, 0.65f)))
            //.AddComponent(new CStateEnergy(0, 20f, CStateEnergy.Mode.EConsumeOnly))

            .AddComponent(new CStateAutoTransition(state_idle))
            .AddTransition(state_dash, new Period(0.3f))

        ;

        state_dash
            .AddComponent(new CStateInput(3))
            .AddComponent(new CStateAnimation("dash"))
            .AddComponent(new CStateMaxStateInstances())
            .AddComponent(new CStateCd(cd_dash))
            .AddComponent(new CStateCd(cd_dash_long, CStateCd.EMode.EConditionOnly))
            .AddComponent(new CStateInitDirectionSmooth(atackRotationSpeed, freeRotFar, new Period(0, 0.65f)))
            //.AddComponent(new CStateEnergy(0, 10f, CStateEnergy.Mode.EConsumeOnly))

            .AddComponent(new CStateAutoTransition(state_idle))
            .AddTransition(state_arrowHit, new Period(0.1f, 0.7f))
            .AddTransition(state_jump, new Period(0.125f, 0.95f), false)
            //.AddTransition(state_dashBack, new Period(0.325f, 0.9f), false)
            ;

        state_bowHit
            .AddComponent(new CStateInput(1))
            .AddComponent(new CStateAnimation("bowHit"))
            .AddComponent(new CStateMaxStateInstances())
            .AddComponent(new CStateCd(cd_bowHit))
            .AddComponent(new CStateCd(cd_dash_long, CStateCd.EMode.EConditionOnly))
            .AddComponent(new CStateInitDirectionSmooth(atackRotationSpeed, freeRotClose, new Period(0, 0.65f)))
            //.AddComponent(new CStateEnergy(0, 20f, CStateEnergy.Mode.EConsumeOnly))

            .AddComponent(new CStateAutoTransition(state_idle))
            .AddTransition(state_dashBack, new Period(0.375f, 0.8f), false)

        ;
        state_arrowHit
            .AddComponent(new CStateInput(1))
            .AddComponent(new CStateAnimation("arrowHit"))
            .AddComponent(new CStateMaxStateInstances())
            .AddComponent(new CStateCd(cd_bowHit))
            .AddComponent(new CStateInitDirectionSmooth(atackRotationSpeed, freeRotClose, new Period(0, 0.65f)))
            //.AddComponent(new CStateEnergy(0, 20f, CStateEnergy.Mode.EConsumeOnly))

            .AddComponent(new CStateAutoTransition(state_idle))
            .AddTransition(state_dashBack, new Period(0.375f, 0.75f), false)
        ;

        state_dashBack
            .AddComponent(new CStateInput(3))
            .AddComponent(new CStateAnimation("dashBack"))
            .AddComponent(new CStateMaxStateInstances())
            .AddComponent(new CStateCd(cd_dash_long, CStateCd.EMode.ERestartOnly))
            .AddComponent(new CStateCd(cd_bowHit, CStateCd.EMode.ERestartOnly))

            .AddComponent(new CStateReflectedDirection(0.65f, 0.75f, atackRotationSpeed, freeRotFar, new Period(0, 0.65f)).SetRaySeparation(0.35f))
            .AddComponent(new CStateEnergy(0, 5f, CStateEnergy.Mode.EConsumeOnly))

            .AddComponent(new CStateAutoTransition(state_idle))
        ;

        state_jump
            .AddComponent(new CStateInput(3))
            .AddComponent(new CStateAnimation("jump"))
            .AddComponent(new CStateMaxStateInstances())
            .AddComponent(new CStateCd(cd_dash_long, CStateCd.EMode.ERestartOnly))
            .AddComponent(new CStateCd(cd_bowHit, CStateCd.EMode.ERestartOnly))
            .AddComponent(new CStateDashThroughWall(0.51f, 0.95f, 0.35f, 0.375f, 0.1825f))

            .AddComponent(new CStateEnergy(0, 5f, CStateEnergy.Mode.EConsumeOnly))

            .AddComponent(new CStateAutoTransition(state_idle))
            .AddTransition(state_drawArrow, new Period(0.7f))
            .AddTransition(state_arrowHit, new Period(0.7f))
        ;


        state_pain
            .AddComponent(new CStateRandomAnimation(new string[] { "pain1", "pain2" }))
            .AddComponent(new CStateDamageShake())
            .AddComponent(new CStateDamage(10.0f, 0.0f))
            /*.AddComponent(new CStateCdReduce(
                new CStateCdReduce.CdRestartStruct[] {
                    new CStateCdReduce.CdRestartStruct(cd_swing, 0.1f),
                    new CStateCdReduce.CdRestartStruct(cd_push, 0.1f),
                    new CStateCdReduce.CdRestartStruct(cd_pull, 0.1f),
                    new CStateCdReduce.CdRestartStruct(cd_dash, -1.35f),
                }))*/
            .AddComponent(new CStateAutoTransition(state_idle))

        ;

    }
    /// cleans up CharacterStateController data
    public override void CleanUpWeapon(CharacterStateController controller)
    {
        controller.ClearStates();
    }
}
