using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using ReAnim;
using Character;


public class CharacterStateController : MonoBehaviour
{
    #region State Managament
    [NonSerialized] public int appliedStatesCount;

    List<State> states = new List<State>();
    State currentState;
    State previousState;

    public bool logStateId = false;

    #region Damage accumulator
    [Range(0, 1)]
    public float damageAccumulatorDamping;
    float damageAccumulator = 0;
    float painAccumulator = 0;
    Vector2 damageDirection = Vector2.zero;

    void OnReceiveDamage(HealthController.DamageData data)
    {
        if (data.damage < 0)
        {
            damageAccumulator += data.damage;
            if (data.causer)
                damageDirection += ((Vector2)data.causer.transform.position - (Vector2)transform.position).normalized *data.damage;
        }

        if( data.pain < 0)
            painAccumulator += data.pain;
    }
    public void OnDeath(HealthController.DamageData data)
    {
        OnReceiveDamage(data);
    }
    public float GetDamageAccumulator()
    {
        return damageAccumulator;
    }
    public void ModifyDamageAccumulator(float f)
    {
        damageAccumulator += f;
    }
    public float GetPainAccumulator()
    {
        return painAccumulator;
    }
    public void ModifyPainAccumulator(float f)
    {
        painAccumulator += f;
    }
    #endregion Damage accumulator

    void StateUpdate()
    {
        var stateInfo = animator.GetCurrentAnimatorStateInfo(0);
        float time = stateInfo.normalizedTime;


        Debug.Assert(currentState != null);
        currentState.Update(time);
    }

    void StateFixedUpdate()
    {
        var stateInfo = animator.GetCurrentAnimatorStateInfo(0);
        float time = stateInfo.normalizedTime;


        Debug.Assert(currentState != null);
        currentState.FixedUpdate(time);
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
    public void SetCurrentState(State state)
    {
        previousState = currentState;
        currentState = state;
    }
    public State GetCurrentState() { return currentState; }
    public State GetPreviousState() { return previousState; }
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


    SkillAnimationBehaviour currentAnimatorBehaviour;
    public void SetCurrentAnimatorBehaviour(SkillAnimationBehaviour s)
    {
        currentAnimatorBehaviour = s;
    }
    public bool validateCurrentAnimatorBehaviour()
    {
        return currentAnimatorBehaviour && currentState != null && currentAnimatorBehaviour.skillId == currentState.stateId;
    }
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
        input = GetComponentInChildren<InputManagerBase>();
        animator = GetComponent<Animator>();
        movement = GetComponent<PlayerMovement>();
        body = GetComponent<Rigidbody2D>();

        //InitDagger();


    }
   
    
    
    private void LateUpdate()
    {
        StateUpdate();
        if (logStateId)
            Debug.Log(currentState.stateId);
            //Debug.Log(damageAccumulator < -0.1 ? damageAccumulator : 0.0f);
    }
    private void FixedUpdate()
    {
        damageAccumulator *= damageAccumulatorDamping;
        painAccumulator *= damageAccumulatorDamping;

        StateFixedUpdate();
    }

    #endregion Events

    #region Getters
    public InputManagerBase GetInput() { return input; }
    public PlayerMovement GetMovement() { return movement; }
    public Rigidbody2D GetBody() { return body; }
    public Animator GetAnimator() { return animator; }
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

    #region CommonInt
    List<int> commonInts = new List<int>();
    public int AddCommonInt(int var = 0)
    {
        commonInts.Add(var);
        return commonInts.Count-1;
    }
    public int GetCommonInt(int id)
    {
        return commonInts[id];
    }
    public void SetCommonInt(int id, int var)
    {
        commonInts[id] = var;
    }
    #endregion CommonInt

    #region Resource
    public ResourceController[] resources;
    #endregion Resource

}


