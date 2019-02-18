using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ReAnim;
using Character;


public class PlayerSpawner : MonoBehaviour {

    public int teamId;
    public int inputTypeId;
    public int characterId;
    public bool insertToCamera = false;
    public bool respawn = false;
    GameObject spawned;

    private void Start()
    {
        spawned = GameManager.instance.SpawnPlayer(transform, teamId, inputTypeId, characterId);

        if(characterId == 0)
            InitDagger(spawned);
        else if(characterId == 1)
        {
            InitSpear(spawned);
        }
        else if (characterId == 2)
        {
            InitBow(spawned);
        }else if(characterId == 3)
        {
            InitShield(spawned);
        }


        if (insertToCamera)
        {
            Camera.main.GetComponent<MultiCameraController>().AddToCamera(spawned.transform,true);
        }
    }

    private void Update()
    {
        if (respawn && !spawned && Input.GetKeyDown(KeyCode.Escape))
        {
            Start();
            Debug.Log("Respawn");
        }
    }

    void InitShield(GameObject gameObject)
    {
        var ctrl = gameObject.GetComponent<CharacterStateController>();

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

    void InitBow(GameObject gameObject)
    {
        var ctrl = gameObject.GetComponent<CharacterStateController>();

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
            .AddTransition(state_releaseArrow_fake, new Period(0.35f, 0.75f))
        ;
        state_holdArrow
            .AddComponent(new CStateMaxStateInstances())
            .AddComponent(new CStateInitDirectionUpdate(arrowHoldRotationSpeed, freeRotClose, 7.75f, new Period(0, float.PositiveInfinity)))

            .AddTransition(state_bowHit)
            .AddTransition(state_dash)
            .AddTransition(state_shootArrow)
            
        ;
        state_releaseArrow
             .AddComponent(new CStateInput(2))
             .AddComponent(new CStateAnimation("releaseArrow"))
             .AddComponent(new CStateMaxStateInstances())
             
             .AddComponent(new CStateAutoTransition(state_idle))

            .AddTransition(state_bowHit , new Period(0.85f))
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
             .AddComponent(new CStateInput(0,true))
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
            .AddTransition(state_drawArrow, new Period(0.65f))
            ;

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
            
            .AddComponent(new CStateReflectedDirection(0.6f, 0.45f, atackRotationSpeed, freeRotFar, new Period(0, 0.65f)).SetRaySeparation(0.35f))
            .AddComponent(new CStateEnergy(0, 5f, CStateEnergy.Mode.EConsumeOnly))

            .AddComponent(new CStateAutoTransition(state_idle))
        ;

        state_jump
            //.AddComponent(new CStateInput(3))
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
    void InitDagger(GameObject gameObject)
    {
        var ctrl = gameObject.GetComponent<CharacterStateController>();

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
            //.AddComponent(new CStateInput(3))
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

    void InitSpear(GameObject gameObject)
    {
        var ctrl = gameObject.GetComponent<CharacterStateController>();

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

    void InitSpear_ref(GameObject gameObject)
    {
        var ctrl = gameObject.GetComponent<CharacterStateController>();

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
                .AddAliased(new CStateTransition(stateLightAtack_pose1, new Period(0.9f)))
                )
            .AddComponent(new CStateAlias()
                .AddCondition(new CStateIntEq(intPose, 1))

                .AddAliased(new CStateTransition(statePush_pose2, new Period(0.9f)))
                .AddAliased(new CStateTransition(stateBack_pose2, new Period(0.75f)))
                .AddAliased(new CStateTransition(stateLightAtack_pose2, new Period(0.9f)))
                )
        ;

        /// Basic atacks (light)
        {

            float movementSpeedAtack = 440.0f;

            stateLightAtack_pose1
                .AddComponent(new CStateSetInt(intPose, 0))
                .AddComponent(new CStateInput(0))
                .AddComponent(new CStateSequentionalAnimation(new string[] { "slash-1-left", "slash-1-right" }))
                .AddComponent(new CStateMaxStateInstances())
                .AddComponent(new CStateCd(cdId_lightAtack))
                .AddComponent(new CStateInitDirectionSmooth(1, freeRot, new Period(0, 0.35f)))
                .AddComponent(new CStateMotor(Vector2.up * movementSpeedAtack, freeRot, new Period(0.2f, 0.4f)))
                .AddComponent(new CStateEnergy(0, 5f, CStateEnergy.Mode.EConsumeOnly))

                .AddComponent(new CStateAutoTransition(state_pose1))

                .AddTransition(state_pain)
                .AddTransition(stateBack_pose1, new Period(0.9f), false)
                .AddTransition(statePush_pose1, new Period(0.5f))
                .AddTransition(stateHeavyAtack_to2, new Period(0.5f), false)
                .AddTransition(stateSwitchPose_to2, new Period(0.75f), false)
            //.AddTransition(stateLightAtack_pose1, new Period(0.9f))
            ;

            stateLightAtack_pose2
                .AddComponent(new CStateSetInt(intPose, 1))
                .AddComponent(new CStateInput(0))
                .AddComponent(new CStateSequentionalAnimation(new string[] { "slash-2-left", "slash-2-right" }))
                .AddComponent(new CStateMaxStateInstances())
                .AddComponent(new CStateCd(cdId_lightAtack))
                .AddComponent(new CStateInitDirectionSmooth(1, freeRot, new Period(0, 0.35f)))
                .AddComponent(new CStateMotor(Vector2.up * movementSpeedAtack, freeRot, new Period(0.2f, 0.4f)))
                .AddComponent(new CStateEnergy(0, 5f, CStateEnergy.Mode.EConsumeOnly))

                .AddComponent(new CStateAutoTransition(state_pose2))

                .AddTransition(state_pain)
                .AddTransition(stateBack_pose2, new Period(0.9f), false)
                .AddTransition(statePush_pose2, new Period(0.5f))
                .AddTransition(stateHeavyAtack_to1, new Period(0.5f), false)
                .AddTransition(stateSwitchPose_to1, new Period(0.75f), false)
            //.AddTransition(stateLightAtack_pose2, new Period(0.9f))
            ;
        }
        { /// pose transition

            float movementSpeedAtack = 650.0f;

            /// heavy atack
            stateHeavyAtack_to2
                .AddComponent(new CStateSetInt(intPose, 1))
                .AddComponent(new CStateInput(2))
                .AddComponent(new CStateAnimation("heavy-2"))
                .AddComponent(new CStateMaxStateInstances())
                .AddComponent(new CStateCd(cdId_HeavyAtack_p2))
                .AddComponent(new CStateCd(cdId_lightAtack, CStateCd.EMode.ERestartOnly))
                .AddComponent(new CStateInitDirectionSmooth(atackRotationSpeed, freeRot, new Period(0, 0.35f)))
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
                .AddComponent(new CStateInput(2))
                .AddComponent(new CStateAnimation("heavy-1"))
                .AddComponent(new CStateMaxStateInstances())
                .AddComponent(new CStateCd(cdId_HeavyAtack_p1))
                .AddComponent(new CStateCd(cdId_lightAtack, CStateCd.EMode.ERestartOnly))
                .AddComponent(new CStateInitDirectionSmooth(atackRotationSpeed, freeRot, new Period(0, 0.35f)))
                .AddComponent(new CStateMotor(Vector2.up * movementSpeedAtack, freeRot, new Period(0.2f, 0.4f)))
                .AddComponent(new CStateEnergy(0, 15f, CStateEnergy.Mode.EConsumeOnly))

                .AddComponent(new CStateAutoTransition(state_pose1))

                .AddTransition(state_pain, new Period(0f, 0.1f), false)
                .AddTransition(state_pain, new Period(0.45f, 1f), false)

                .AddTransition(statePush_pose1, new Period(0.65f))
            ;

            // pose transition
            stateSwitchPose_to2
                .AddComponent(new CStateSetInt(intPose, 1))
                .AddComponent(new CStateInput(2))
                .AddComponent(new CStateAnimation("switch-2"))
                .AddComponent(new CStateMaxStateInstances())
                .AddComponent(new CStateInitDirectionSmooth(atackRotationSpeed, freeRot))
                .AddComponent(new CStateExclusiveReady(stateLightAtack_pose1))
                .AddComponent(new CStateAutoTransition(state_pose2))
                .AddComponent(new CStateEnergy(0, 2.5f, CStateEnergy.Mode.EConsumeOnly))

                .AddTransition(state_pain)
            ;
            stateSwitchPose_to1
                .AddComponent(new CStateSetInt(intPose, 0))
                .AddComponent(new CStateInput(2))
                .AddComponent(new CStateAnimation("switch-1"))
                .AddComponent(new CStateMaxStateInstances())
                .AddComponent(new CStateInitDirectionSmooth(atackRotationSpeed, freeRot))
                .AddComponent(new CStateExclusiveReady(stateLightAtack_pose2))
                .AddComponent(new CStateAutoTransition(state_pose1))
                .AddComponent(new CStateEnergy(0, 2.5f, CStateEnergy.Mode.EConsumeOnly))

                .AddTransition(state_pain)
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
                .AddComponent(new CStateInitDirectionSmooth(atackRotationSpeed, freeRot, new Period(0, 0.35f)))
                .AddComponent(new CStateMotor(Vector2.up * movementSpeedAtack, freeRot, new Period(0.2f, 0.4f)))
                .AddComponent(new CStateEnergy(0, 12.5f, CStateEnergy.Mode.EConsumeOnly))

                .AddComponent(new CStateAutoTransition(state_pose1))
                .AddTransition(state_pain)
                .AddTransition(stateBack_pose1, new Period(0.6f))
                .AddTransition(stateHeavyAtack_to2, new Period(0.75f))
                .AddTransition(stateLightAtack_pose1, new Period(0.35f))
            ;
            statePush_pose2
                .AddComponent(new CStateSetInt(intPose, 1))
                .AddComponent(new CStateInput(1))
                .AddComponent(new CStateAnimation("push-pose2"))
                .AddComponent(new CStateMaxStateInstances())
                .AddComponent(new CStateCd(cdId_Push))
                .AddComponent(new CStateInitDirectionSmooth(atackRotationSpeed, freeRot, new Period(0, 0.35f)))
                .AddComponent(new CStateMotor(Vector2.up * movementSpeedAtack, freeRot, new Period(0.2f, 0.4f)))
                .AddComponent(new CStateEnergy(0, 12.5f, CStateEnergy.Mode.EConsumeOnly))

                .AddComponent(new CStateAutoTransition(state_pose2))
                .AddTransition(state_pain)
                .AddTransition(stateBack_pose2, new Period(0.6f))
                .AddTransition(stateHeavyAtack_to1, new Period(0.75f))
                .AddTransition(stateLightAtack_pose2, new Period(0.35f))
            ;
        }
        // back
        {
            float movementSpeedAtack = -700.0f;
            float movementSpeedAtack2 = -2300.0f;

            stateBack_pose1
                .AddComponent(new CStateSetInt(intPose, 0))
                .AddComponent(new CStateInput(3))
                .AddComponent(new CStateAnimation("back-pose1"))
                .AddComponent(new CStateMaxStateInstances())
                .AddComponent(new CStateCd(cdId_back_p1))
                .AddComponent(new CStateInitDirectionSmooth(atackRotationSpeed, freeRot, new Period(0, 0.35f)))
                .AddComponent(new CStateMotor(Vector2.up * movementSpeedAtack, freeRot, new Period(0.0f, 0.15f)))
                .AddComponent(new CStateMotor(Vector2.up * movementSpeedAtack2, freeRot, new Period(0.0f, 0.05f)))
                .AddComponent(new CStateEnergy(0, 7.5f, CStateEnergy.Mode.EConsumeOnly))

                .AddComponent(new CStateAutoTransition(state_pose1))

                //.AddTransition(state_pain)
                .AddTransition(statePush_pose1, new Period(0.175f))
                .AddTransition(stateHeavyAtack_to2, new Period(0.25f))
                .AddTransition(stateLightAtack_pose1, new Period(0.35f))

            //.AddTransition(stateBack_pose2, new Period(0.75f))
            //.AddTransition(stateLightAtack_pose2, new Period(0.3f))
            ;
            stateBack_pose2
                .AddComponent(new CStateSetInt(intPose, 1))
                .AddComponent(new CStateInput(3))
                .AddComponent(new CStateAnimation("back-pose2"))
                .AddComponent(new CStateMaxStateInstances())
                .AddComponent(new CStateCd(cdId_back_p2))
                .AddComponent(new CStateInitDirectionSmooth(atackRotationSpeed, freeRot, new Period(0, 0.35f)))
                .AddComponent(new CStateMotor(Vector2.up * movementSpeedAtack, freeRot, new Period(0.0f, 0.15f)))
                .AddComponent(new CStateMotor(Vector2.up * movementSpeedAtack2, freeRot, new Period(0.0f, 0.05f)))
                .AddComponent(new CStateEnergy(0, 7.5f, CStateEnergy.Mode.EConsumeOnly))

                .AddComponent(new CStateAutoTransition(state_pose2))

                //.AddTransition(state_pain)
                .AddTransition(statePush_pose2, new Period(0.175f))
                .AddTransition(stateHeavyAtack_to1, new Period(0.25f))
                .AddTransition(stateLightAtack_pose2, new Period(0.35f))
            //.AddTransition(stateBack_pose1, new Period(0.75f))
            //.AddTransition(stateLightAtack_pose1, new Period(0.3f))
            ;
        }

        state_pose1

            .AddTransition(state_pain)
            .AddTransition(stateBack_pose1)
            .AddTransition(statePush_pose1)
            .AddTransition(stateLightAtack_pose1)
            .AddTransition(stateHeavyAtack_to2)
            .AddTransition(stateSwitchPose_to2, false)
            .AddTransition(state_idle)

            .AddComponent(new CStateAlias().AddCondition(new CStateInput(1)).AddAliased(new CStateBlockRotation()).AddAliased(new CStateBlockMovement()))
            .AddComponent(new CStateAlias().AddCondition(new CStateInput(0)).AddAliased(new CStateBlockRotation()).AddAliased(new CStateBlockMovement()))
            ;

        state_pose2

            .AddTransition(state_pain)
            .AddTransition(stateBack_pose2)
            .AddTransition(statePush_pose2)
            .AddTransition(stateLightAtack_pose2)
            .AddTransition(stateHeavyAtack_to1)
            .AddTransition(stateSwitchPose_to1, false)
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
                .AddAliased(new CStateTransition(stateLightAtack_pose1))
                .AddAliased(new CStateTransition(stateSwitchPose_to2))
                .AddAliased(new CStateTransition(statePush_pose1))
                .AddAliased(new CStateTransition(stateBack_pose1))
                )
            .AddComponent(new CStateAlias()
                .AddCondition(new CStateIntEq(intPose, 1))
                .AddAliased(new CStateTransition(stateLightAtack_pose2))
                .AddAliased(new CStateTransition(stateSwitchPose_to1))
                .AddAliased(new CStateTransition(statePush_pose2))
                .AddAliased(new CStateTransition(stateBack_pose2))
                )

            .AddTransition(state_pain)
            ;
    }
}
