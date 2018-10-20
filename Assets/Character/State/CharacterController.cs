using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using ReAnim;
using Character;


public class CharacterController : MonoBehaviour
{
    #region State Managament
    [NonSerialized] public int appliedStatesCount;

    List<State> states = new List<State>();
    State currentState;

    public bool logStateId = false;


    void StateUpdate()
    {
        var stateInfo = animator.GetCurrentAnimatorStateInfo(0);
        float time = stateInfo.normalizedTime;


        Debug.Assert(currentState != null);
        currentState.Update(time);
    }

    #region Init Functions
    public State AddState( bool isCurrentState = false) {
        State state = new State();
        states.Add(state);
        state.controller = this;
        state.stateId = GetLastStateId();
        if (isCurrentState) currentState = state;
        return state;
    }
    public int GetLastStateId()
    {
        return states.Count - 1;
    }
    public State GetState(int id) { return states[id]; }
    public void SetCurrentState(State state) { currentState = state; }
    public State GetCurrentState() { return currentState; }
    #endregion Init Functions;

    #region Utility Functions
    public void AddTransitionAll(State towards, Period period)
    {
        StateTransition transition = new StateTransition();
        transition.target = towards;
        transition.period = period;

        foreach (var it in states)
            if(it != towards)
                it.AddTransition(transition);
    }
    public void ResetInputBuffer()
    {
        foreach (var it in states)
            it.bufferedInput = false;
    }
    #endregion Utility Functions

    #endregion State Managament

    #region Mono
    protected EnergyController resource;
    protected InputManagerBase input;
    protected PlayerMovement movement;
    protected Rigidbody2D body;
    protected Animator animator;

    #region Events
    protected void Start()
    {
        resource = GetComponent<EnergyController>();
        input = GetComponent<InputManagerBase>();
        animator = GetComponent<Animator>();
        movement = GetComponent<PlayerMovement>();
        body = GetComponent<Rigidbody2D>();

        InitDagger();


    }
    void InitDagger()
    {
        var state_idle = AddState(true);        // 0
        var state_swing = AddState();           // 1
        var state_push = AddState();            // 2
        var state_pull = AddState();            // 3
        var state_dash = AddState();            // 4
        var state_throw = AddState();           // 5
        var state_pain = AddState();            // 6
        var state_faint = AddState();

        int cd_swing = AddCd(0.55f);
        int cd_push = AddCd(1.25f);
        int cd_pull = AddCd(1.25f);
        int cd_dash = AddCd(0.55f);
        int cd_pain = AddCd(0.4f);
        int cd_faint = AddCd(5.0f);

        float atackRotationSpeed = 0.45f;
        float freeRotFar = 1f;
        float freeRotClose = 1f;

        AddTransitionAll(state_pain, new Period(0f,1f));

        state_swing
            .AddComponent(new CStateInput(0))
            .AddComponent(new CStateAnimation("swing"))
            .AddComponent(new CStateMaxStateInstances())
            .AddComponent(new CStateCd(cd_swing))
            .AddComponent(new CStateInitDirectionSmooth(atackRotationSpeed, freeRotClose, new Period(0, 0.65f)))

            .AddComponent(new CStateAutoTransition(state_idle))

            .AddTransition(state_push, new Period(0.55f, 1f))
            .AddTransition(state_throw, new Period(0.45f, 0.5f))
            .AddTransition(state_faint, new Period(0.325f,0.375f))
        ;
        state_push
            .AddComponent(new CStateInput(1))
            .AddComponent(new CStateAnimation( "push"))
            .AddComponent(new CStateMaxStateInstances())
            .AddComponent(new CStateCd(cd_push))
            .AddComponent(new CStateInitDirectionSmooth(atackRotationSpeed, freeRotClose, new Period(0, 0.65f)))
            
            .AddComponent(new CStateAutoTransition(state_idle))
            
            .AddTransition(state_pull, new Period(0.225f, 1f))
            .AddTransition(state_swing, new Period(0.25f, 1f))
            .AddTransition(state_faint, new Period(0.3f, 0.45f))
        ;
        state_pull
            .AddComponent(new CStateInput(2))
            .AddComponent(new CStateAnimation( "pull"))
            .AddComponent(new CStateMaxStateInstances())
            .AddComponent(new CStateCd(cd_pull))
            .AddComponent(new CStateInitDirectionSmooth(atackRotationSpeed, freeRotClose, new Period(0, 0.65f)))
            
            .AddComponent(new CStateAutoTransition(state_idle))
            
            .AddTransition(state_push, new Period(0.25f, 0.5f))
            .AddTransition(state_swing, new Period(0.3f, 1f))
            .AddTransition(state_faint, new Period(0.3f, 0.45f))
        ;
        state_dash
            .AddComponent(new CStateInput(3))
            .AddComponent(new CStateAnimation("dash"))
            .AddComponent(new CStateMaxStateInstances())
            .AddComponent(new CStateCd(cd_dash))
            .AddComponent(new CStateInitDirectionSmooth(atackRotationSpeed, freeRotFar, new Period(0, 0.65f)))

            .AddComponent(new CStateAutoTransition(state_idle))


            .AddTransition(state_swing, new Period(0.2f, 0.7f))
            .AddTransition(state_push, new Period(0.25f, 1f))
            .AddTransition(state_pull, new Period(0.2f, 1f))
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
            .AddComponent(new CStateCd(cd_pull))
            .AddComponent(new CStateInitDirectionSmooth(atackRotationSpeed*3, 1.5f, new Period(0.35f, 0.65f)))
            .AddComponent(new CStateSpawn(0, new Period(0.45f,1f) ))

            .AddComponent(new CStateAutoTransition(state_idle))
        ;
        state_faint
            .AddComponent(new CStateInput(3))
            .AddComponent(new CStateAnimationTrigger("faint"))
            .AddComponent(new CStateCd(cd_dash, CStateCd.EMode.ERestartOnly))
            .AddComponent(new CStateCd(cd_faint))

            .AddComponent(new CStateInstantTransition(state_idle))
            ;


        state_pain
            .AddComponent(new CStateRandomAnimation(new string[] { "pain", "pain2" }))
            .AddComponent(new CStateDamageShake())
            .AddComponent(new CStateStagger(
                new CStateStagger.CdRestartStruct[] {
                    new CStateStagger.CdRestartStruct(cd_swing, 0.5f),
                    //new CStateStagger.CdRestartStruct(cd_push, -0.5f),
                    //new CStateStagger.CdRestartStruct(cd_pull, -0.5f),
                    new CStateStagger.CdRestartStruct(cd_dash, -1.25f),
                } ) )
            .AddComponent(new CStateAutoTransition(state_idle))
        ;



    }
    void InitSpear()
    {
        /// Test Init
        {
            int cd1Id_lightAtack = AddCd(0.75f);
            int cd1Id_HeavyAtack = AddCd(3.0f);
            int cd1Id_switchPose = AddCd(1.15f);

            var state_pose1 = AddState(true);           //  0
            var stateLightAtack_pose1 = AddState();     //  1

            var state_pose2 = AddState();               //  2
            var stateLightAtack_pose2 = AddState();     //  3

            var stateHeavyAtack_to2 = AddState();       //  4
            var stateHeavyAtack_to1 = AddState();       //  5

            var stateSwitchPose_to2 = AddState();       //  6
            var stateSwitchPose_to1 = AddState();       //  7

            float atackRotationSpeed = 0.15f;

            /// Basic atacks (light)
            {
                stateLightAtack_pose1
                    .AddComponent(new CStateInput(0))
                    .AddComponent(new CStateSequentionalAnimation(new string[] { "slash-1-left", "slash-1-right" }))
                    .AddComponent(new CStateMaxStateInstances())
                    .AddComponent(new CStateCd(cd1Id_lightAtack))
                    .AddComponent(new CStateCd(cd1Id_switchPose, CStateCd.EMode.EConditionOnly))
                    .AddComponent(new CStateInitDirectionSmooth(atackRotationSpeed, 1.5f))
                    .AddComponent(new CStateMotor(Vector2.up * 300, 1.5f, new Period(0.2f, 0.4f)))

                    .AddComponent(new CStateAutoTransition(state_pose1))

                    //.AddTransition(stateHeavyAtack_to2, new Period(0.5f))
                    .AddTransition(stateLightAtack_pose1, new Period(0.9f))
                ;

                stateLightAtack_pose2
                    .AddComponent(new CStateInput(1))
                    .AddComponent(new CStateSequentionalAnimation(new string[] { "slash-2-left", "slash-2-right" }))
                    .AddComponent(new CStateMaxStateInstances())
                    .AddComponent(new CStateCd(cd1Id_lightAtack))
                    .AddComponent(new CStateCd(cd1Id_switchPose, CStateCd.EMode.EConditionOnly))
                    .AddComponent(new CStateInitDirectionSmooth(atackRotationSpeed, 1.5f))
                    .AddComponent(new CStateMotor(Vector2.up * 300, 1.5f, new Period(0.2f, 0.4f)))

                    .AddComponent(new CStateAutoTransition(state_pose2))

                    //.AddTransition(stateHeavyAtack_to1, new Period(0.5f))
                    .AddTransition(stateLightAtack_pose2, new Period(0.9f))
                ;
            }
            { /// pose transition

                /// heavy atack
                stateHeavyAtack_to2
                    .AddComponent(new CStateInput(1))
                    .AddComponent(new CStateAnimation("heavy-2"))
                    .AddComponent(new CStateMaxStateInstances())
                    .AddComponent(new CStateCd(cd1Id_HeavyAtack))
                    .AddComponent(new CStateCd(cd1Id_switchPose, CStateCd.EMode.ERestartOnly))
                    .AddComponent(new CStateCd(cd1Id_lightAtack, CStateCd.EMode.ERestartOnly))
                    .AddComponent(new CStateInitDirectionSmooth(atackRotationSpeed, 1.5f))
                    .AddComponent(new CStateMotor(Vector2.up * 300, 1.5f, new Period(0.2f, 0.4f)))

                    .AddComponent(new CStateAutoTransition(state_pose2));
                ;
                stateHeavyAtack_to1
                    .AddComponent(new CStateInput(0))
                    .AddComponent(new CStateAnimation("heavy-1"))
                    .AddComponent(new CStateMaxStateInstances())
                    .AddComponent(new CStateCd(cd1Id_HeavyAtack))
                    .AddComponent(new CStateCd(cd1Id_switchPose, CStateCd.EMode.ERestartOnly))
                    .AddComponent(new CStateCd(cd1Id_lightAtack, CStateCd.EMode.ERestartOnly))
                    .AddComponent(new CStateInitDirectionSmooth(atackRotationSpeed, 1.5f))
                    .AddComponent(new CStateMotor(Vector2.up * 300, 1.5f, new Period(0.2f, 0.4f)))

                    .AddComponent(new CStateAutoTransition(state_pose1))
                ;

                // pose transition
                stateSwitchPose_to2
                    .AddComponent(new CStateInput(1))
                    .AddComponent(new CStateAnimation("switch-2"))
                    .AddComponent(new CStateCd(cd1Id_switchPose, CStateCd.EMode.EConditionOnly))
                    .AddComponent(new CStateMaxStateInstances())
                    .AddComponent(new CStateInitDirectionSmooth(atackRotationSpeed, 1.5f))
                    .AddComponent(new CStateExclusiveReady(stateLightAtack_pose1))
                    //.AddComponent(new CStateExclusiveReady(stateHeavyAtack_to1))
                    //.AddComponent(new CStateExclusiveReady(stateHeavyAtack_to2))
                    .AddComponent(new CStateAutoTransition(state_pose2))
                ;
                stateSwitchPose_to1
                    .AddComponent(new CStateInput(0))
                    .AddComponent(new CStateAnimation("switch-1"))
                    .AddComponent(new CStateCd(cd1Id_switchPose, CStateCd.EMode.EConditionOnly))
                    .AddComponent(new CStateMaxStateInstances())
                    .AddComponent(new CStateInitDirectionSmooth(atackRotationSpeed, 1.5f))
                    .AddComponent(new CStateExclusiveReady(stateLightAtack_pose2))
                    //.AddComponent(new CStateExclusiveReady(stateHeavyAtack_to1))
                    //.AddComponent(new CStateExclusiveReady(stateHeavyAtack_to2))
                    .AddComponent(new CStateAutoTransition(state_pose1))
                ;
            }


            state_pose1
                //.AddComponent(new CStateBlockRotation(0.1f))

                //.AddTransition(stateHeavyAtack_to2)
                .AddTransition(stateLightAtack_pose1)
                .AddTransition(stateSwitchPose_to2, false)
                ;

            state_pose2
                //.AddComponent(new CStateBlockRotation(0.1f))

                //.AddTransition(stateHeavyAtack_to1)
                .AddTransition(stateLightAtack_pose2)
                .AddTransition(stateSwitchPose_to1, false)
                ;
        }
    }

    private void LateUpdate()
    {
        StateUpdate();
        if(logStateId)
            Debug.Log(currentState.stateId);
    }

    #endregion Events

    #region Getters
    public InputManagerBase GetInput() { return input; }
    public PlayerMovement GetMovement() { return movement; }
    public Rigidbody2D GetBody() { return body; }
    /// animator - provided functions only
    /// energy - TODO, probably same as animator
    #endregion Getters

    #region Utility
    public AnimatorStateInfo GetAnimationStateInfo()
    {
        return animator.GetCurrentAnimatorStateInfo(0);
    }
    public void PlayAnimation(int animCode)
    {
        animator.SetTrigger(animCode);
    }
    public void PlayAnimation(string animCode)
    {
        animator.SetBool(animCode, true);
    }
    public void PlayAnimationTrigger(string animCode)
    {
        animator.SetTrigger(animCode);
    }
    public void ResetAnimation(string animCode)
    {
        animator.SetBool(animCode, false);
    }
    #endregion Utility


    #endregion Mono

    #region Cd
    public class CdRecord
    {
        public Timer timer = new Timer();
        /// TODO - possibility for stacking cd which reduces it's value over time
    }
    List<CdRecord> cds = new List<CdRecord>();

    #region Init
    /// return: id of newly created cd
    public int AddCd(float cd = 0, bool restart = true)
    {
        var _cd = new CdRecord();
        _cd.timer = new Timer(cd);
        if (restart)
            _cd.timer.restart();
        cds.Add(_cd);
        return cds.Count-1;
    }
    #endregion Init

    #region Utility
    public Timer GetCdTimer(int id) { return cds[id].timer; }
    public bool IsCdReady(int id) { return cds[id].timer.isReady(); }
    public void RestartCd(int id, float state = 0f) { cds[id].timer.restart(state); }
    public void RestartCdAll(float state = 0f) {
        foreach(var it in cds)
            it.timer.restart(state);
    }
    #endregion Utility

    #endregion Cd

    #region Prefabs
    [Serializable]
    public struct PrefabData
    {
        public GameObject prefab;
        public Transform spawnPoint;
    }
    [SerializeField] PrefabData[] prefabs;
    public GameObject SpawnPrefab(int id)
    {
        var obj = Instantiate(prefabs[id].prefab, prefabs[id].spawnPoint.position, prefabs[id].spawnPoint.rotation);
        return obj;
    }
    public GameObject SpawnPrefab(int id, Vector2 position, float rotation)
    {
        var obj = Instantiate(prefabs[id].prefab, prefabs[id].spawnPoint.position + (Vector3)position, 
            Quaternion.Euler(0,0, prefabs[id].spawnPoint.rotation.eulerAngles.z + rotation));
        return obj;
    }
    #endregion Prefabs


}


