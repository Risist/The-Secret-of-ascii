using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ReAnim;
using Character;


public class PlayerSpawner : MonoBehaviour {

    public int teamId;
    public int inputTypeId;
    public bool insertToCamera = false;

    private void Start()
    {
        var p = GameManager.instance.SpawnPlayer(transform, teamId, inputTypeId);
        InitDagger_testlight(p);
        if(insertToCamera)
        {
            Camera.main.GetComponent<MultiCameraController>().targets.Add(p.transform);
        }
    }

    void InitDagger_testlight(GameObject gameObject)
    {
        var ctrl = gameObject.GetComponent<CharacterController>();

        var state_idle          = ctrl.AddState(true);        // 0
        var state_swing_light   = ctrl.AddState();            // 1
        var state_push_light    = ctrl.AddState();            // 2
        var state_pain          = ctrl.AddState();            // 3
        var state_dash          = ctrl.AddState();            // 4
        var state_dash_back     = ctrl.AddState();            // 5
        var state_push_heavy    = ctrl.AddState();            // 6

        int cd_hit_light    = ctrl.AddCd(0.2f);
        int cd_push_heavy   = ctrl.AddCd(0.3f);
        int cd_pain         = ctrl.AddCd(0.55f);
        int cd_dash         = ctrl.AddCd(0.3f);

        ctrl.AddTransitionAll(state_pain, new Period(0f, 1f));

        int poseInt = ctrl.AddCommonInt(0);

        float atackRotationSpeed = 0.65f;
        float freeRotFar = 1f;
        float freeRotClose = 1f;

        state_push_heavy
            .AddComponent(new CStateInput(1))
            .AddComponent(new CStateAnimation("push"))
            .AddComponent(new CStateMaxStateInstances())
            .AddComponent(new CStateCd(cd_push_heavy))
            .AddComponent(new CStateInitDirectionSmooth(atackRotationSpeed, freeRotClose, new Period(0, 0.65f)))

            .AddComponent(new CStateAutoTransition(state_idle))
        ;

        state_swing_light
            .AddComponent(new CStateInput(0))
            .AddComponent(new CStateSequentionalAnimation( new string[] { "slash-light-left", "slash-light-right" }, poseInt))
            .AddComponent(new CStateMaxStateInstances())
            .AddComponent(new CStateCd(cd_hit_light))
            .AddComponent(new CStateInitDirectionSmooth(atackRotationSpeed, freeRotClose, new Period(0, 0.85f)))

            .AddComponent(new CStateAutoTransition(state_idle))


            //.AddTransition(state_push_light, new Period(0.5f, 0.75f))
            //.AddTransition(state_swing_light, new Period(0.85f, 1.0f), false)
        ;
        state_push_light
            .AddComponent(new CStateInput(1))
            .AddComponent(new CStateSequentionalAnimation(new string[] { "push-light-left", "push-light-right" }, poseInt))
            .AddComponent(new CStateMaxStateInstances())
            .AddComponent(new CStateCd(cd_hit_light))
            .AddComponent(new CStateInitDirectionSmooth(atackRotationSpeed, freeRotClose, new Period(0, 0.65f)))

            .AddComponent(new CStateAutoTransition(state_idle))


            //.AddTransition(state_swing_light, new Period(0.5f, 0.75f))
            //.AddTransition(state_push_light, new Period(0.95f, 1.0f),false)
        ;

        state_idle
            .AddTransition(state_dash)
            .AddTransition(state_swing_light)
            .AddTransition(state_push_light)    
            /// its a hack to prevent unity bug happening
            /// OnAnimationEnd sometimes is not called, so it will ensure that the animation is in sync witn state machine
            .AddComponent(new CStateAutoTransition(state_idle).ApplyOnBeggin().ApplyOnEnd(false));
        ;

        state_pain
            .AddComponent(new CStateRandomAnimation(new string[] { "pain", "pain2" }))
            .AddComponent(new CStateDamageShake())
            .AddComponent(new CStateDamage(1.5f, 0.0f))
            .AddComponent(new CStateCd(cd_pain))
            /*.AddComponent(new CStateCdReduce(
                new CStateCdReduce.CdRestartStruct[] {
                    new CStateCdReduce.CdRestartStruct(cd_swing, 0.1f),
                    new CStateCdReduce.CdRestartStruct(cd_push, 0.1f),
                    new CStateCdReduce.CdRestartStruct(cd_pull, 0.1f),
                    new CStateCdReduce.CdRestartStruct(cd_dash, -1.35f),
                }))*/
            .AddComponent(new CStateAutoTransition(state_idle))
            .AddTransition(state_pain)

        //.AddTransition(state_painWall, new Period(0f, 1f));
        ;

        state_dash
            .AddComponent(new CStateInput(3))
            .AddComponent(new CStateSequentionalAnimation(new string[] { "dash-left", "dash-right" }, poseInt))
            //.AddComponent(new CStateAnimation("dash-left"))
            .AddComponent(new CStateMaxStateInstances())
            .AddComponent(new CStateCd(cd_dash))
            //.AddComponent(new CStateCd(cd_swing_heavy, CStateCd.EMode.EConditionOnly))
            .AddComponent(new CStateInitDirectionSmooth(atackRotationSpeed, freeRotFar, new Period(0, 0.65f)))

            .AddComponent(new CStateAutoTransition(state_idle))

            .AddTransition(state_push_heavy, new Period(0.25f, 1f))
            .AddTransition(state_swing_light, new Period(0.0f, 0.85f))
            .AddTransition(state_dash_back, new Period(0.1f, 0.35f))
        ;

        state_dash_back
            .AddComponent(new CStateInput(3))
            //.AddComponent(new CStateInput(2))
            .AddComponent(new CStateAnimation("dash-back"))
            .AddComponent(new CStateMaxStateInstances())
            .AddComponent(new CStateCd(cd_dash, CStateCd.EMode.ERestartOnly))
            .AddComponent(new CStateInitDirectionSmooth(atackRotationSpeed*0.75f, freeRotFar, new Period(0, 0.7f), new Period(0.15f, 0.2f)))
            .AddComponent(new CStateCollisionInFront(0.5f, new Vector2(-0.35f, 0.75f)))
            .AddComponent(new CStateCollisionInFront(0.5f, new Vector2(0.35f, 0.75f)))

            .AddComponent(new CStateAutoTransition(state_idle))
            .AddTransition(state_push_heavy, new Period(0.0f, 0.8f))
            .AddTransition(state_swing_light, new Period(0f, 0.85f))
        ;

    }

    void InitDagger(GameObject gameObject)
    {
        var ctrl = gameObject.GetComponent<CharacterController>();

        var state_idle = ctrl.AddState(true);        // 0
        var state_swing = ctrl.AddState();           // 1
        var state_push = ctrl.AddState();            // 2
        var state_pull = ctrl.AddState();            // 3
        var state_dash = ctrl.AddState();            // 4
        var state_throw = ctrl.AddState();           // 5
        var state_pain = ctrl.AddState();            // 6
        var state_painWall = ctrl.AddState();        // 7
        var state_dashBack = ctrl.AddState();        // 8

        int cd_swing = ctrl.AddCd(0.25f);            // 1
        int cd_push = ctrl.AddCd(0.35f);             // 2
        int cd_pull = ctrl.AddCd(0.35f);             // 3
        int cd_throw = ctrl.AddCd(0.4f);             // 4
        int cd_dash = ctrl.AddCd(0.25f);             // 5
        int cd_pain = ctrl.AddCd(0.55f);             // 6
        int cd_painWall = ctrl.AddCd(0.4f);          // 7

        float atackRotationSpeed = 0.45f;
        float freeRotFar = 1f;
        float freeRotClose = 1f;

        ctrl.AddTransitionAll(state_pain, new Period(0f, 1f));
        ctrl.AddTransitionAll(state_painWall, new Period(0f, 1f));

        state_swing
            .AddComponent(new CStateInput(0))
            .AddComponent(new CStateAnimation("swing"))
            .AddComponent(new CStateMaxStateInstances())
            .AddComponent(new CStateCd(cd_swing))
            .AddComponent(new CStateInitDirectionSmooth(atackRotationSpeed, freeRotClose, new Period(0, 0.65f)))

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

            .AddComponent(new CStateAutoTransition(state_idle))

            .AddTransition(state_dashBack, new Period(0.1f, 0.75f))
            .AddTransition(state_swing, new Period(0.2f, 0.7f))
            .AddTransition(state_push, new Period(0.25f, 1f))
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

            .AddComponent(new CStateAutoTransition(state_idle))
        ;


        state_pain
            //.AddComponent(new CStateResetAnimation(new string[] { "painWall3", "painWall2", "painWall1" }))
            .AddComponent(new CStateRandomAnimation(new string[] { "pain", "pain2" }))
            .AddComponent(new CStateDamageShake())
            .AddComponent(new CStateDamage(1.75f, 0.0f))
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
        state_painWall
            //.AddComponent(new CStateResetAnimation(new string[] { "pain", "pain2", "painWall3", "painWall2", "painWall1" }))
            .AddComponent(new CStateWallStagger(8.0f, new string[] { "painWall3", "painWall2", "painWall1" }))
            .AddComponent(new CStateCd(cd_painWall))
            .AddComponent(new CStateCdReduce(
                new CStateCdReduce.CdRestartStruct[] {
                    new CStateCdReduce.CdRestartStruct(cd_swing, 0.35f),
                    new CStateCdReduce.CdRestartStruct(cd_push, 0.35f),
                    new CStateCdReduce.CdRestartStruct(cd_pull, 0.35f),
                    new CStateCdReduce.CdRestartStruct(cd_dash, 0.35f),
                }))
            //.AddComponent(new CStateDamageShake())

            .AddComponent(new CStateAutoTransition(state_idle))
        ;



    }
}
