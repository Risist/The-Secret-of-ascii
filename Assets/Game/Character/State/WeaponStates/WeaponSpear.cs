using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using ReAnim;
using Character;

/*
 * Base class for all weapons
 */
public class WeaponSpear : WeaponBase
{
    /// sets up CharacterStateController data
    public override void InitWeapon(CharacterStateController ctrl)
    {
        ctrl.GetAnimator().SetTrigger("WeaponChange");
        ctrl.GetAnimator().SetInteger("WeaponId", 1);

        var state_pose1 = ctrl.AddState();               //  0
        var stateLightAtack_pose1 = ctrl.AddState();     //  1

        var state_pose2 = ctrl.AddState();               //  2
        var stateLightAtack_pose2 = ctrl.AddState();     //  3

        var stateHeavyAtack_to2 = ctrl.AddState();       //  4
        var stateHeavyAtack_to1 = ctrl.AddState();       //  5

        var stateSwitchPose_to2 = ctrl.AddState();       //  6
        var stateSwitchPose_to1 = ctrl.AddState();       //  7

        var statePush_pose1 = ctrl.AddState();           //  8
        var statePush_pose2 = ctrl.AddState();           //  9

        var stateBack_pose1 = ctrl.AddState();           //  10
        var stateBack_pose2 = ctrl.AddState();           //  11

        var state_idle = ctrl.AddState(true);            //  12
        var state_pain = ctrl.AddState();                //  13

        /// TODO: Add block on 4th button
        /// middle pose staf pushing forward with dash?


        int intPose = ctrl.AddCommonInt(0);

        int cdId_lightAtack = ctrl.AddCd(0.0f);
        int cdId_HeavyAtack_p1 = ctrl.AddCd(0f);
        int cdId_HeavyAtack_p2 = ctrl.AddCd(0f);
        int cdId_back_p1 = ctrl.AddCd(0f);
        int cdId_back_p2 = ctrl.AddCd(0f);
        int cdId_Push = ctrl.AddCd(0.0f);
        int cdId_Pain = ctrl.AddCd(0.2f);
        int cdId_idle = ctrl.AddCd(0.1f);

        float atackRotationSpeed = 0.8f;
        float freeRot = 0.75f;

        //ctrl.AddTransitionAll(state_pain, new Period(0.0f, 1.0f));

        state_pain
            .AddComponent(new CStateRandomAnimation(new string[] { "pain1", "pain2" }))
            .AddComponent(new CStateDamageShake())
            .AddComponent(new CStateDamage(10.0f, 0.0f))
            .AddComponent(new CStateCd(cdId_Pain))

            .AddComponent(new CStateAutoTransition(state_idle))
            //.AddComponent(new CStateAutoTransitionPose(intPose, state_idle, new State[] { state_pose1, state_pose2}, new string[] { "pose1", "pose2"}))
            .AddTransition(state_pain)

            .AddComponent(new CStateAlias()
                .AddCondition(new CStateIntEq(intPose, 0))

                .AddAliased(new CStateTransition(statePush_pose1, new Period(0.9f)))
                .AddAliased(new CStateTransition(stateBack_pose1, new Period(0.75f)))
                //.AddAliased(new CStateTransition(stateLightAtack_pose1, new Period(0.9f)))
                )
            .AddComponent(new CStateAlias()
                .AddCondition(new CStateIntEq(intPose, 1))

                .AddAliased(new CStateTransition(statePush_pose2, new Period(0.9f)))
                .AddAliased(new CStateTransition(stateBack_pose2, new Period(0.75f)))
                //.AddAliased(new CStateTransition(stateLightAtack_pose2, new Period(0.9f)))
                )
        ;

        { /// pose transition

            float movementSpeedAtack = 650.0f;

            /// heavy atack
            stateHeavyAtack_to2
                .AddComponent(new CStateSetInt(intPose, 1))
                .AddComponent(new CStateInput(0))
                .AddComponent(new CStateAnimation("heavy-2"))
                .AddComponent(new CStateMaxStateInstances())
                .AddComponent(new CStateCd(cdId_HeavyAtack_p2))
                .AddComponent(new CStateCd(cdId_lightAtack, CStateCd.EMode.ERestartOnly))
                .AddComponent(new CStateInitDirectionSmooth(atackRotationSpeed, freeRot, new Period(0, 1.0f)))
                .AddComponent(new CStateMotor(Vector2.up * movementSpeedAtack, freeRot, new Period(0.2f, 0.4f)))
                .AddComponent(new CStateEnergy(0, 15f, CStateEnergy.Mode.EConsumeOnly))

                .AddComponent(new CStateAutoTransition(state_pose2))


                //.AddTransition(stateBack_pose2, new Period(0.85f))
                //.AddTransition(statePush_pose1, new Period(0.2f,0.55f))

                .AddTransition(state_pain, new Period(0f, 0.1f), false)
                .AddTransition(state_pain, new Period(0.45f, 1f), false)

                .AddTransition(statePush_pose2, new Period(0.65f))
            ;
            stateHeavyAtack_to1
                .AddComponent(new CStateSetInt(intPose, 0)) // add period within which pose is switched - otherwise animation breaks
                .AddComponent(new CStateInput(0))
                .AddComponent(new CStateAnimation("heavy-1"))
                .AddComponent(new CStateMaxStateInstances())
                .AddComponent(new CStateCd(cdId_HeavyAtack_p1))
                .AddComponent(new CStateCd(cdId_lightAtack, CStateCd.EMode.ERestartOnly))
                .AddComponent(new CStateInitDirectionSmooth(atackRotationSpeed, freeRot, new Period(0, 1.0f)))
                .AddComponent(new CStateMotor(Vector2.up * movementSpeedAtack, freeRot, new Period(0.2f, 0.4f)))
                .AddComponent(new CStateEnergy(0, 15f, CStateEnergy.Mode.EConsumeOnly))

                .AddComponent(new CStateAutoTransition(state_pose1))

                .AddTransition(state_pain, new Period(0f, 0.1f), false)
                .AddTransition(state_pain, new Period(0.45f, 1f), false)

                .AddTransition(statePush_pose1, new Period(0.65f))
            ;
        }

        // push
        {
            float movementSpeedAtack = 680.0f;

            statePush_pose1
                .AddComponent(new CStateSetInt(intPose, 0))
                .AddComponent(new CStateInput(1))
                .AddComponent(new CStateAnimation("push-pose1"))
                .AddComponent(new CStateMaxStateInstances())
                .AddComponent(new CStateCd(cdId_Push))
                .AddComponent(new CStateInitDirectionSmooth(atackRotationSpeed, freeRot, new Period(0, 1.0f)))
                .AddComponent(new CStateMotor(Vector2.up * movementSpeedAtack, freeRot, new Period(0.2f, 0.4f)))
                .AddComponent(new CStateEnergy(0, 12.5f, CStateEnergy.Mode.EConsumeOnly))

                .AddComponent(new CStateAutoTransition(state_pose1))
                .AddTransition(state_pain)
                .AddTransition(stateBack_pose1, new Period(0.6f))
                .AddTransition(stateHeavyAtack_to2, new Period(0.75f))
            //.AddTransition(stateLightAtack_pose1, new Period(0.35f))
            ;
            statePush_pose2
                .AddComponent(new CStateSetInt(intPose, 1))
                .AddComponent(new CStateInput(1))
                .AddComponent(new CStateAnimation("push-pose2"))
                .AddComponent(new CStateMaxStateInstances())
                .AddComponent(new CStateCd(cdId_Push))
                .AddComponent(new CStateInitDirectionSmooth(atackRotationSpeed, freeRot, new Period(0, 1.0f)))
                .AddComponent(new CStateMotor(Vector2.up * movementSpeedAtack, freeRot, new Period(0.2f, 0.4f)))
                .AddComponent(new CStateEnergy(0, 12.5f, CStateEnergy.Mode.EConsumeOnly))

                .AddComponent(new CStateAutoTransition(state_pose2))
                .AddTransition(state_pain)
                .AddTransition(stateBack_pose2, new Period(0.6f))
                .AddTransition(stateHeavyAtack_to1, new Period(0.75f))
            //.AddTransition(stateLightAtack_pose2, new Period(0.35f))
            ;
        }
        // back
        {
            float movementSpeedAtack = -800.0f;
            float movementSpeedAtack2 = -2400.0f;

            stateBack_pose1
                .AddComponent(new CStateSetInt(intPose, 0))
                .AddComponent(new CStateInput(3))
                .AddComponent(new CStateAnimation("back-pose1"))
                .AddComponent(new CStateMaxStateInstances())
                .AddComponent(new CStateCd(cdId_back_p1))
                .AddComponent(new CStateInitDirectionSmooth(atackRotationSpeed, freeRot, new Period(0, 1.0f)))
                .AddComponent(new CStateMotor(Vector2.up * movementSpeedAtack, freeRot, new Period(0.0f, 0.15f)))
                .AddComponent(new CStateMotor(Vector2.up * movementSpeedAtack2, freeRot, new Period(0.0f, 0.07f)))
                .AddComponent(new CStateEnergy(0, 7.5f, CStateEnergy.Mode.EConsumeOnly))

                .AddComponent(new CStateAutoTransition(state_pose1))

                //.AddTransition(state_pain)
                .AddTransition(statePush_pose1, new Period(0.175f))
                .AddTransition(stateHeavyAtack_to2, new Period(0.175f))
                .AddTransition(state_idle, new Period(0.6f))
            ;
            stateBack_pose2
                .AddComponent(new CStateSetInt(intPose, 1))
                .AddComponent(new CStateInput(3))
                .AddComponent(new CStateAnimation("back-pose2"))
                .AddComponent(new CStateMaxStateInstances())
                .AddComponent(new CStateCd(cdId_back_p2))
                .AddComponent(new CStateInitDirectionSmooth(atackRotationSpeed, freeRot, new Period(0, 1.0f)))
                .AddComponent(new CStateMotor(Vector2.up * movementSpeedAtack, freeRot, new Period(0.0f, 0.15f)))
                .AddComponent(new CStateMotor(Vector2.up * movementSpeedAtack2, freeRot, new Period(0.0f, 0.05f)))
                .AddComponent(new CStateEnergy(0, 7.5f, CStateEnergy.Mode.EConsumeOnly))

                .AddComponent(new CStateAutoTransition(state_pose2))

                //.AddTransition(state_pain)
                .AddTransition(statePush_pose2, new Period(0.175f))
                .AddTransition(stateHeavyAtack_to1, new Period(0.25f))
                .AddTransition(state_idle, new Period(0.6f))
            ;
        }

        state_pose1

            .AddTransition(state_pain)
            .AddTransition(stateBack_pose1)
            .AddTransition(statePush_pose1)
            .AddTransition(stateHeavyAtack_to2)
            .AddTransition(state_idle)

            .AddComponent(new CStateAlias().AddCondition(new CStateInput(1)).AddAliased(new CStateBlockRotation()).AddAliased(new CStateBlockMovement()))
            .AddComponent(new CStateAlias().AddCondition(new CStateInput(0)).AddAliased(new CStateBlockRotation()).AddAliased(new CStateBlockMovement()))
            ;

        state_pose2

            .AddTransition(state_pain)
            .AddTransition(stateBack_pose2)
            .AddTransition(statePush_pose2)
            .AddTransition(stateHeavyAtack_to1)
            .AddTransition(state_idle)

            .AddComponent(new CStateAlias().AddCondition(new CStateInput(1)).AddAliased(new CStateBlockRotation()).AddAliased(new CStateBlockMovement()))
            .AddComponent(new CStateAlias().AddCondition(new CStateInput(0)).AddAliased(new CStateBlockRotation()).AddAliased(new CStateBlockMovement()))
            ;

        state_idle
            .AddComponent(new CStateConditionAtMove())
            .AddComponent(new CStateAnimation("AtMove"))
            .AddComponent(new CStateCd(cdId_idle))

            .AddComponent(new CStateAlias()
                .AddCondition(new CStateIntEq(intPose, 0))
                .AddAliased(new CStateTransition(stateHeavyAtack_to2))
                .AddAliased(new CStateTransition(statePush_pose1))
                .AddAliased(new CStateTransition(stateBack_pose1))
                )
            .AddComponent(new CStateAlias()
                .AddCondition(new CStateIntEq(intPose, 1))
                .AddAliased(new CStateTransition(stateHeavyAtack_to1))
                .AddAliased(new CStateTransition(statePush_pose2))
                .AddAliased(new CStateTransition(stateBack_pose2))
                )

            .AddTransition(state_pain)
            ;
    }
    /// cleans up CharacterStateController data
    public override void CleanUpWeapon(CharacterStateController controller)
    {
        controller.ClearStates();
    }
}