using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using ReAnim;
using Character;

/*
 * Base class for all weapons
 */
public class WeaponSwordShield : WeaponBase
{
    /// sets up CharacterStateController data
    public override void InitWeapon(CharacterStateController ctrl)
    {
        ctrl.GetAnimator().SetTrigger("WeaponChange");
        ctrl.GetAnimator().SetInteger("WeaponId", 3);

        var state_idle = ctrl.AddState(true);        // 0
        var state_dash = ctrl.AddState();            // 1
        var state_blockInit = ctrl.AddState();       // 2
        var state_blockStay = ctrl.AddState();       // 3
        var state_blockBack = ctrl.AddState();       // 4
        var state_shieldBash = ctrl.AddState();      // 5
        var state_slashLoad = ctrl.AddState();       // 6
        var state_slashStay = ctrl.AddState();       // 7
        var state_slashBack = ctrl.AddState();       // 8
        var state_push = ctrl.AddState();            // 9
        var state_pain = ctrl.AddState();            // 10

        int cd_dash = ctrl.AddCd(0.3f);
        int cd_block = ctrl.AddCd(0.1f);
        int cd_slash = ctrl.AddCd(0.25f);

        const float atackRotationSpeed = 0.45f;
        const float freeRotFar = 0.25f;
        const float freeRotClose = 0.25f;
        const float blockRotationSpeed = 0.1f;

        state_pain
            .AddComponent(new CStateRandomAnimation(new string[] { "Pain1", "Pain2" }))
            .AddComponent(new CStateDamageShake())
            .AddComponent(new CStateDamage(10.0f, 0.0f))
            .AddComponent(new CStateAutoTransition(state_idle))
        ;

        state_idle
            .AddComponent(new CStateAnimationAtMove("AtMove"))

            .AddTransition(state_dash)
            .AddTransition(state_blockInit)
            .AddTransition(state_slashLoad)
            .AddTransition(state_push)
        ;


        ctrl.AddTransitionAll(state_pain, new Period(0f, 1f));

        state_dash
            .AddComponent(new CStateInput(3))
            .AddComponent(new CStateAnimation("Dash"))
            .AddComponent(new CStateMaxStateInstances())
            .AddComponent(new CStateCd(cd_dash))
            .AddComponent(new CStateInitDirectionSmooth(atackRotationSpeed, freeRotFar, new Period(0, 0.65f)))

            .AddComponent(new CStateAutoTransition(state_idle))

            //.AddTransition(state_slashBack)
            .AddTransition(state_shieldBash, new Period(0.45f, 0.6f), false);
        ;

        state_push
            .AddComponent(new CStateInput(1))
            .AddComponent(new CStateAnimation("Push"))
            .AddComponent(new CStateMaxStateInstances())

            .AddComponent(new CStateInitDirectionSmooth(atackRotationSpeed, freeRotFar, new Period(0, 0.65f)))

            .AddComponent(new CStateAutoTransition(state_idle))
            ;

        state_shieldBash
            .AddComponent(new CStateInput(1))
            .AddComponent(new CStateAnimation("ShieldBash"))
            .AddComponent(new CStateMaxStateInstances())

            .AddComponent(new CStateInitDirectionSmooth(atackRotationSpeed, freeRotFar, new Period(0, 0.65f)))

            .AddComponent(new CStateAutoTransition(state_idle))
            //.AddTransition(state_blockInit, new Period(0.8f))
            ;

        state_blockInit
            .AddComponent(new CStateInput(2))
            .AddComponent(new CStateAnimation("Block"))
            .AddComponent(new CStateMaxStateInstances())
            .AddComponent(new CStateInitDirectionSmooth(atackRotationSpeed * 0.75f, freeRotClose, new Period(0, 0.25f)))
            .AddComponent(new CStateInitDirectionUpdate(blockRotationSpeed, freeRotClose, 8.75f, new Period(0.25f, float.PositiveInfinity)))
            .AddComponent(new CStateCd(cd_block))

            .AddComponent(new CStateAutoTransition(state_blockStay))
        ;
        state_blockStay
            .AddComponent(new CStateMaxStateInstances())
            .AddComponent(new CStateInitDirectionUpdate(blockRotationSpeed, freeRotClose, 10.75f, new Period(0, float.PositiveInfinity)))

            .AddTransition(state_blockBack)
            .AddTransition(state_dash)
            //.AddTransition(state_shieldBash)
            .AddTransition(state_push)
        ;

        state_blockBack
            .AddComponent(new CStateInput(2, true))
            .AddComponent(new CStateAnimation("BlockBack"))
            .AddComponent(new CStateMaxStateInstances())

            .AddComponent(new CStateAutoTransition(state_idle))
            .AddTransition(state_shieldBash)
            .AddTransition(state_dash)
        ;

        state_slashLoad
            .AddComponent(new CStateInput(0))
            .AddComponent(new CStateAnimation("Slash"))
            .AddComponent(new CStateMaxStateInstances())
            .AddComponent(new CStateInitDirectionSmooth(atackRotationSpeed * 0.75f, freeRotClose, new Period(0, 0.25f)))
            .AddComponent(new CStateInitDirectionUpdate(blockRotationSpeed, freeRotClose, 8.75f, new Period(0.25f, float.PositiveInfinity)))
            .AddComponent(new CStateCd(cd_slash))

            .AddComponent(new CStateAutoTransition(state_slashStay))

        //.AddTransition(state_dash)
        //.AddTransition(state_shieldBash)
        ;

        state_slashStay
            .AddComponent(new CStateMaxStateInstances())
            .AddComponent(new CStateInitDirectionUpdate(blockRotationSpeed, freeRotClose, 3.75f, new Period(0, float.PositiveInfinity)))

            .AddTransition(state_dash)
            .AddTransition(state_shieldBash)
            .AddTransition(state_slashBack)
        ;
        state_slashBack
            .AddComponent(new CStateInput(0, true))
            .AddComponent(new CStateAnimation("SlashBack"))
            .AddComponent(new CStateMaxStateInstances())

            .AddComponent(new CStateAutoTransition(state_idle))
            .AddTransition(state_blockInit, new Period(0.5f))
        //.AddTransition(state_dash, new Period(0.9f))
        //.AddTransition(state_shieldBash, new Period(0.9f))
        ;
    }
    /// cleans up CharacterStateController data
    public override void CleanUpWeapon(CharacterStateController controller)
    {
        controller.ClearStates();
    }
}