using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ReAi;



class StateMachineInterruption
{
    struct MemoryInterruption
    {
        public BehaviourHolder entry;
        public EMemoryEvent eventType;
        public EMemoryState eventState;
    }
    List<MemoryInterruption> memoryInterruptions = new List<MemoryInterruption>();
    public StateMachineInterruption AddMemoryInterruption(BehaviourHolder entry, EMemoryEvent eventType, EMemoryState eventState)
    {
        MemoryInterruption s = new MemoryInterruption();
        s.entry = entry;
        s.eventState = eventState;
        s.eventType = eventType;

        memoryInterruptions.Add(s);
        return this;
    }

    public BehaviourHolder neutralEntry;

    int lastInterruptionType = 0;
    public void Update(BehaviourStateMachine stateMachine)
    {
        for(int i = 1; i <= memoryInterruptions.Count; ++i)
        {
            var it = memoryInterruptions[i - 1];

            MemoryEvent ev = stateMachine.memory.SearchInMemory(it.eventType);

            if (ev != null && ev.GetState() == it.eventState)
            {
                //Debug.Log(i + "; " + it.eventType + "; " + it.eventState);
                stateMachine.target = ev;
                if (lastInterruptionType != i)
                {
                    lastInterruptionType = i;
                    stateMachine.SetCurrentBehaviour(it.entry);
                }
                return;
            }
        }

        
        {
            //Debug.Log("neutral");
            stateMachine.target = null;

            if (lastInterruptionType != 0)
            {
                lastInterruptionType = 0;
                stateMachine.SetCurrentBehaviour(neutralEntry);
            }
        }

    }
}




public class InputControllerAi : InputManagerExternal
{
    AiPerceptionHolder memory;

    BehaviourStateMachine stateMachine = new BehaviourStateMachine();
    StateMachineInterruption interruption = new StateMachineInterruption();

    new private void Start()
    {
        base.Start();
        memory = GetComponent<AiPerceptionHolder>();
        if (!memory)
        {
            Debug.LogError("No PerceptionHolder in : " + gameObject);
        }

        /// init state machines
        /// 
        InitMachine(stateMachine);

        InitNeutral();
        InitTest();

        //InitEnemyKnowledge();
        //InitEnemyPainKnowledge();
        //InitEnemyShade();
        //InitNoiseKnowledge();
        //InitNoiseShade();
    }

    int lastState = 0;
    new private void Update()
    {
        interruption.Update(stateMachine);
        stateMachine.Update();
        base.Update();
    }

    #region Init
    void InitMachine(BehaviourStateMachine machine)
    {
        machine.data = inputData;
        machine.memory = memory;
        machine.character = GetComponentInParent<CharacterStateController>();
    }

    void InitTest()
    {
        var entry = stateMachine.AddNewBehaviour();
        var observe = stateMachine.AddNewBehaviour();
        var stayInRange = stateMachine.AddNewBehaviour();

        observe
            .AddFilter(new BFilterBlackboardUpdateDistanceAccumulator(10000))
            .AddFilter(new BFilterResetInput(true, true, false))
            .AddFilter(new BFilterRotationToAim(0.1f))
            .AddTransition(observe)
            .AddTransition(stayInRange, 5f)
            ;

        stayInRange 
            .AddFilter(new BFilterBlackboardUpdateDistanceAccumulator(10000))
            .AddFilter(new BFilterResetInput(false, true, true, true))
            .AddFilter(new BFilterStayInRange(3f, 8f))
            .AddFilter(new BFilterTimedExecution(0.75f, 1f))

            .AddTransition(observe);
            ;

        entry.AddFilter(new BFilterBlackboardResetDistanceAccumulator())
            .AddTransition(observe);

        interruption.AddMemoryInterruption(entry, EMemoryEvent.EEnemy, EMemoryState.EKnowledge);

    }
    void InitNeutral()
    {
        var neutralEntry = stateMachine.AddNewBehaviour();
        interruption.neutralEntry = neutralEntry;

        var patrol = stateMachine.AddNewBehaviour();
        var search = stateMachine.AddNewBehaviour();
        var lookAround = stateMachine.AddNewBehaviour();
        var idle = stateMachine.AddNewBehaviour();


        patrol
            .AddFilter(new BFilterResetInput(false, false, true, true))
            .AddFilter(new BFilterPatrol(20f))
            .AddFilter(new BFilterTimedExecution(2f, 10f))
            .AddFilter(new BFilterKeyPressIndicator())
            .AddTransition(neutralEntry);
        ;

        idle.AddFilter(new BFilterResetInputBeggin(true, true, true, true))
            .AddFilter(new BFilterTimedExecution(0.5f,1.5f))
            .AddTransition(neutralEntry)
            ;

        search
            .AddFilter(new BFilterResetInput(false, false, true, true))
            .AddFilter(new BFilterTimedExecution(1.5f, 3f))
            .AddFilter(new BFilterSearch().SetBaseSearchArea(10f))
            .AddFilter(new BFilterKeyPressIndicator())
            .AddFilter(new BFilterAllignDirectionToPosition(0.2f))

            .AddTransition(search, 1f)
            .AddTransition(idle, 2f)
            .AddTransition(lookAround, 2f)
            ;

        lookAround
            .AddFilter(new BFilterResetInputBeggin(true, true, false, true))
            .AddFilter(new BFilterTimedExecution(0.75f, 1f))
            .AddFilter(new BFilterLookAround(0.1f, BehaviourFilterBase.EBehaviourStateReturn.EStillExecute)
                .SetCloseAngle(3f, BehaviourFilterBase.EBehaviourStateReturn.ENextStateIfAll))


            .AddTransition(idle, 1f)
            .AddTransition(search, 3f)
            .AddTransition(lookAround, 1f)
                ;

        neutralEntry
            .AddFilter(new BFilterSwitchAnimationIndicator(2, false))
            .AddFilter(new BFilterSwitchAnimationIndicator(3, false))
            .AddTransition(idle,4f)
            .AddTransition(patrol,0.5f)
            .AddTransition(search, 0.125f)
            .AddTransition(lookAround, 1.25f)
            ;

        stateMachine.SetCurrentBehaviour(neutralEntry);
    }
    void InitEnemyKnowledge()
    {
        var stayInRange = stateMachine.AddNewBehaviour();
        var dashAway = stateMachine.AddNewBehaviour();
        var pushAway = stateMachine.AddNewBehaviour();

        var observe = stateMachine.AddNewBehaviour();
        interruption.AddMemoryInterruption(observe, EMemoryEvent.EEnemy, EMemoryState.EKnowledge);

        var atack0      = stateMachine.AddNewBehaviour();
        var atack1      = stateMachine.AddNewBehaviour();
        var atack2      = stateMachine.AddNewBehaviour();
        var atackDash   = stateMachine.AddNewBehaviour();

        var atackThrow1 = stateMachine.AddNewBehaviour();
        var atackThrow2 = stateMachine.AddNewBehaviour();

        atackThrow1
            .AddFilter(new BFilterResetInput(false, false, true, true))
            .AddFilter(new BFilterRequireCharacterState(5).SetRequireTransition(true))
            .AddFilter(new BFilterRequireCharacterState(1)
                .AddComboTransition(atackThrow2))
            .AddFilter(new BFilterKeyPress(0))
            .AddFilter(new BFilterPositionToAim(0.1f))
            .AddFilter(new BFilterDirectionToAim(0.5f))
            .AddFilter(new BFilterDistanceFromAim(10, 35))

            .AddTransition(atackThrow2, 50f)
            .AddTransition(dashAway, 25f)
            .AddTransition(pushAway, 25f)
            .AddTransition(observe)
            ;

        atackThrow2
            .AddFilter(new BFilterResetInput(false, false, true, true))
            .AddFilter(new BFilterRequireCharacterState(5))
            .AddFilter(new BFilterKeyPress(2))
            .AddFilter(new BFilterPositionToAim(0.1f))
            .AddFilter(new BFilterDirectionToAim(0.5f))
            .AddFilter(new BFilterDistanceFromAim(7, 35))

            .AddTransition(observe)
            ;


        atack0
            .AddFilter(new BFilterResetInput(false, false, true, true))
            .AddFilter(new BFilterRequireCharacterState(1)
                .AddComboTransition(atack1))
            .AddFilter(new BFilterKeyPress(0))
            .AddFilter(new BFilterPositionToAim(0.1f))
            .AddFilter(new BFilterDirectionToAim(0.5f))
            .AddFilter(new BFilterDistanceFromAim(0, 5))
            
            .AddTransition(atack1)
            .AddTransition(atackThrow2)
            .AddTransition(observe)
            ;
        atack1
            .AddFilter(new BFilterResetInput(false, false, true, true))
            .AddFilter(new BFilterRequireCharacterState(2)
                .AddComboTransition(atack0)
                .AddComboTransition(atack2))
            .AddFilter(new BFilterKeyPress(1))
            .AddFilter(new BFilterPositionToAim(0.1f))
            .AddFilter(new BFilterDirectionToAim(0.5f))
            .AddFilter(new BFilterDistanceFromAim(1, 60))

            .AddTransition(atack0)
            .AddTransition(atack2)
            .AddTransition(observe)
            ;
        atack2
            .AddFilter(new BFilterResetInput(false, false, true, true))
            .AddFilter(new BFilterRequireCharacterState(3)
                .AddComboTransition(atack0)
                .AddComboTransition(atack1))
            .AddFilter(new BFilterKeyPress(2))
            .AddFilter(new BFilterPositionToAim(0.1f))
            .AddFilter(new BFilterDirectionToAim(0.5f))
            .AddFilter(new BFilterDistanceFromAim(0, 2))

            .AddTransition(atack0)
            .AddTransition(atack1)
            .AddTransition(observe)
            ;
        atackDash
            .AddFilter(new BFilterResetInput(false, false, true, true))
            .AddFilter(new BFilterRequireCharacterState(4)
                .AddComboTransition(atack0)
                .AddComboTransition(atack1)
                .AddComboTransition(atack2))
            .AddFilter(new BFilterKeyPress(3))
            .AddFilter(new BFilterPositionToAim(0.1f))
            .AddFilter(new BFilterDirectionToAim(0.5f))
            .AddFilter(new BFilterDistanceFromAim(4, 60))

            .AddTransition(atack0)
            .AddTransition(atack1)
            .AddTransition(atack2)
            .AddTransition(observe)
            ;

        stayInRange
            .AddFilter(new BFilterTimedExecution(0.75f, 1f))
            .AddFilter(new BFilterResetInput(false, false, true, true))
            .AddFilter(new BFilterStayInRange(3f,8f))
            .AddFilter(new BFilterAllignDirectionToPosition(0.2f))
            .AddFilter(new BFilterKeyPressIndicator())

            .AddTransition(observe)
            ;

        dashAway
            .AddFilter(new BFilterResetInput(false, false, true, true))
            .AddFilter(new BFilterRequireCharacterState(4)
                .AddComboTransition(pushAway) )
            .AddFilter(new BFilterDistanceFromAim(0f, 4f))
            .AddFilter(new BFilterPositionToAim(0.1f, -1))
            .AddFilter(new BFilterDirectionToAim(0.5f, -1))
            .AddFilter(new BFilterKeyPress(3))

            .AddTransition(pushAway, 1f)
            .AddTransition(observe)
            ;
        pushAway
            .AddFilter(new BFilterResetInput(false, false, true, true))
            .AddFilter(new BFilterRequireCharacterState(2))
            .AddFilter(new BFilterDistanceFromAim(0f, 4f))
            .AddFilter(new BFilterPositionToAim(0.1f, -1))
            .AddFilter(new BFilterDirectionToAim(0.5f, -1))
            .AddFilter(new BFilterKeyPress(1))

            .AddTransition(observe)
            ;

        observe
            .AddFilter(new BFilterSwitchAnimationIndicator(2,false))
            .AddFilter(new BFilterSwitchAnimationIndicator(3,true))
            .AddFilter(new BFilterResetInput(true, true, false))
            .AddFilter(new BFilterRotationToAim(0.1f))
            .AddFilter(new BFilterTimedExecution(0.05f, 0.1f))

            .AddTransition(atack0, 1f)
            .AddTransition(atack1, 1f)
            .AddTransition(atack2, 1.25f)
            .AddTransition(atackDash,1f)
            .AddTransition(atackThrow1, 1f)

            .AddTransition(dashAway, 4f)
            .AddTransition(pushAway, 3f)
            .AddTransition(stayInRange, 6f)

            .AddTransition(observe,1.5f)
            ;
    }
    void InitEnemyShade()
    {
        var entry = stateMachine.AddNewBehaviour();
        interruption.AddMemoryInterruption(entry, EMemoryEvent.EEnemy, EMemoryState.EShade);
        interruption.AddMemoryInterruption(entry, EMemoryEvent.EEnemy_Pain, EMemoryState.EShade);

        var search = stateMachine.AddNewBehaviour();
        var lookAround = stateMachine.AddNewBehaviour();

        
        lookAround
            .AddFilter(new BFilterResetInputBeggin(true, true, false, true))
            .AddFilter(new BFilterTimedExecution(0.75f, 1.25f))
            .AddFilter(new BFilterLookAround(0.1f)
               .SetTChangeAngle(0.75f, 1f)
               .SetTargetDifference(55f, 160f))

            .AddTransition(search, 3f)
            .AddTransition(lookAround, 1f)
                ;
        search
            .AddFilter(new BFilterResetInput(false, false, true, true))
            .AddFilter(new BFilterTimedExecution(1.5f, 3f))
            .AddFilter(new BFilterSearch())
            .AddFilter(new BFilterKeyPressIndicator())
            .AddFilter(new BFilterAllignDirectionToPosition(0.2f))

            .AddTransition(search, 3f)
            .AddTransition(lookAround, 1f)
            ;

        entry
            .AddFilter(new BFilterSwitchAnimationIndicator(2, true))
            .AddFilter(new BFilterSwitchAnimationIndicator(3, false))
            .AddTransition(search, 2f)
            .AddTransition(lookAround, 1f)
            ;
    }

    void InitEnemyPainKnowledge()
    {
        var observe = stateMachine.AddNewBehaviour();
        interruption.AddMemoryInterruption(observe, EMemoryEvent.EEnemy_Pain, EMemoryState.EKnowledge);

        observe
            .AddFilter(new BFilterSwitchAnimationIndicator(2, false))
            .AddFilter(new BFilterSwitchAnimationIndicator(3, true))
            .AddFilter(new BFilterResetInput(true, true, false))
            .AddFilter(new BFilterRotationToAim(0.2f))
            .AddFilter(new BFilterPositionToAim(0.15f, -1))
            ;
    }

    void InitNoiseKnowledge()
    {
        
        var lookAt = stateMachine.AddNewBehaviour();
        var lookAround = stateMachine.AddNewBehaviour();
        interruption.AddMemoryInterruption(lookAt, EMemoryEvent.ENoise, EMemoryState.EKnowledge);

        lookAt
            .AddFilter(new BFilterSwitchAnimationIndicator(2, true))
            .AddFilter(new BFilterSwitchAnimationIndicator(3, false))
            .AddFilter(new BFilterResetInput(true, true, false))
            .AddFilter(new BFilterRotationToAim(0.2f))
            .AddFilter(new BFilterTimedExecution(0.5f, 1.5f))
            .AddTransition(lookAround)
            ;
       
        lookAround
            .AddFilter(new BFilterResetInputBeggin(true, true, false, true))
            .AddFilter(new BFilterTimedExecution(0.75f, 1.25f))
            .AddFilter(new BFilterLookAround(0.1f)
               .SetTChangeAngle(0.75f, 1f)
               .SetTargetDifference(55f, 160f))
            ;

    }

    void InitNoiseShade()
    {
        var moveTo = stateMachine.AddNewBehaviour();
        var lookAround = stateMachine.AddNewBehaviour();
        interruption.AddMemoryInterruption(moveTo, EMemoryEvent.ENoise, EMemoryState.EShade);

        moveTo
           .AddFilter(new BFilterResetInput(false, false, true, true))
           .AddFilter(new BFilterTimedExecution(1.5f, 3f))
           .AddFilter(new BFilterSearch())
           .AddFilter(new BFilterKeyPressIndicator())
           .AddFilter(new BFilterAllignDirectionToPosition(0.2f))
           .AddTransition(moveTo,2f)
           .AddTransition(lookAround)
           ;

        lookAround
            .AddFilter(new BFilterResetInputBeggin(true, true, false, true))
            .AddFilter(new BFilterTimedExecution(0.75f, 1.25f))
            .AddFilter(new BFilterLookAround(0.1f)
               .SetTChangeAngle(0.75f, 1f)
               .SetTargetDifference(55f, 160f))

            .AddTransition(moveTo, 2f)
            .AddTransition(lookAround)
            ;

    }



    /*void InitEnemyKnowledge()
    {
        var entry = entryEnemyKnowledge = stateMachine.AddNewBehaviour(new BehaviourBase(), true);

        var lookAt = stateMachine.AddNewBehaviour(new BehaviourLookAt(0.075f, 0.15f))
            .SetTimeChangeBehaviour(0.75f, 1.0f)
            .AddTransition(entry)
            ;

        var atack1 = stateMachine.AddNewBehaviour(new BehaviourUseSkill(new float[]{1f,0.0f,0f,0f}))
            .SetDirectionInputParam(0.5f)
            .SetPositionInputParam(0.1f)
            .SetTimeChangeBehaviour(0.3f,0.4f)
            .AddTransition(entry);

        var atack2 = stateMachine.AddNewBehaviour(new BehaviourUseSkill(new float[] { 0f, 1f, 0f, 0f }))
            .SetDirectionInputParam(0.5f)
            .SetPositionInputParam(0.1f)
            .SetTimeChangeBehaviour(0.3f, 0.4f)
            .AddTransition(entry);

        var atack3 = stateMachine.AddNewBehaviour(new BehaviourUseSkill(new float[] { 0f, 0.0f, 1f, 0f }))
            .SetDirectionInputParam(0.5f)
            .SetPositionInputParam(0.1f)
            .SetTimeChangeBehaviour(0.3f, 0.4f)
            .AddTransition(entry);

        var atackDash = stateMachine.AddNewBehaviour(new BehaviourUseSkill(new float[] { 0f, 0.0f, 0f, 1f }))
            .SetDirectionInputParam(0.5f)
            .SetPositionInputParam(0.1f)
            .SetTimeChangeBehaviour(0.3f, 0.4f)
            .AddTransition(atack1)
            .AddTransition(atack2)
            .AddTransition(atack3)
            .AddTransition(entry);

        atack1.AddTransition(atack3);

        var stayInRange = stateMachine.AddNewBehaviour(new BehaviourStayInRange(0.1f, 0.1f))
            .SetTimeChangeBehaviour(1f, 2f).AddTransition(entry);

        entry
            //.AddTransition(lookAt)
            .AddTransition(stayInRange, 2f)
            .AddTransition(atack1)
            .AddTransition(atack2)
            .AddTransition(atack3)
            .AddTransition(atackDash)
            //.AddTransition(atack2)
            //.AddTransition(atack3)
            ;
    }
    void InitEnemyShade()
    {
        var entry = entryEnemyShade = stateMachine.AddNewBehaviour(new BehaviourBase(), true);
        var lookAround = stateMachine.AddNewBehaviour(new BehaviourLookAround(0.125f))
            .SetTimeChangeBehaviour(0.4f, 0.7f)
            .SetTimeChangeBehaviour(0.5f, 0.75f)
            .AddTransition(entry)
            ;

        var search = stateMachine.AddNewBehaviour(new BehaviourSearch(0.03f, 0.0125f))
            .SetOffset(1f, 1f)
            .SetStopDistance(0.25f)
            .SetTimeChangeBehaviour(2f, 3f)
            ;

        search
            .AddTransition(search, 3f)
            .AddTransition(lookAround, 1f)
            ;

        entry.AddTransition(lookAround, 1f)
            .AddTransition(search, 2f);
        
    }
    void InitNoiseKnowledge()
    {
        
        var entry = entryNoiseKnowledge = stateMachine.AddNewBehaviour(new BehaviourBase(), true);

         var moveToShort = stateMachine.AddNewBehaviour(new BehaviourCheckSource(0.1f))
            .SetTimeChangeBehaviour(1.0f, 4f);

        var lookAt = stateMachine.AddNewBehaviour(new BehaviourLookAt(0.05f, 0.025f))
            .SetTimeChangeBehaviour(0.75f, 0.5f)
            .AddTransition(moveToShort)
            ;

        entry.AddTransition(lookAt, 3f)
            .AddTransition(moveToShort);
    }
    void InitNoiseShade()
    {
        InitMachine(stateMachine);

        var entry = entryNoiseShade = stateMachine.AddNewBehaviour(new BehaviourBase(), true);

        var search = stateMachine.AddNewBehaviour(new BehaviourSearch(0.03f, 0.0125f))
            .SetOffset(1f, 1f)
            .SetStopDistance(0.25f)
            .SetTimeChangeBehaviour(2f, 3f);
        
        /*var lookAt = noiseShadeMachinge.AddNewBehaviour(new BehaviourLookAt(0.05f, 0.0125f) )
            .SetOffset(1f,1f)
            .SetTimeChangeBehaviour(0.75f, 0.75f)
            .AddTransition(search);*

        var idleShort = stateMachine.AddNewBehaviour(new BehaviourIdle())
            .SetTimeChangeBehaviour(0.35f,0.75f)
            .AddTransition(search);

        var lookAround = stateMachine.AddNewBehaviour(new BehaviourLookAround(0.05f))
            .SetTimeChangeBehaviour(0.75f, 1f)
            .SetTimeChangeBehaviour(0.75f, 1.25f)
            .AddTransition(search)
            ;

        search.AddTransition(lookAround, 3f)
            .AddTransition(idleShort, 0.5f)
            .AddTransition(search, 0.75f)
            ;

        entry.AddTransition(search, 3f)
            .AddTransition(lookAround, 1f)
            ;
    }
    void InitNeutral()
    {

        var idleShort = entryNeutral = stateMachine.AddNewBehaviour(new BehaviourIdle(), true)
            .SetTimeChangeBehaviour(0.15f, 0.35f);
        /*var idleLong = stateMachine.AddNewBehaviour(new BehaviourIdle())
            .SetTimeChangeBehaviour(0.45f, 1.25f);
        var randomMoveShort = stateMachine.AddNewBehaviour(new BehaviourRandomPosition(1f, 60f, 1f, 2f))
            .SetTimeChangeBehaviour(0.15f, 0.35f);
        var randomMoveLong = stateMachine.AddNewBehaviour(new BehaviourRandomPosition(1f, 100f, 1f, 4f))
            .SetTimeChangeBehaviour(0.35f, 1.25f);

        idleShort
            .AddTransition(idleShort, 4f)
            .AddTransition(randomMoveShort, 2f)
            .AddTransition(randomMoveLong, 1f);
        idleLong
            .AddTransition(idleShort, 2f)
            .AddTransition(randomMoveShort, 2f)
            .AddTransition(randomMoveLong, 1f);

        randomMoveShort
            .AddTransition(randomMoveShort, 1f)
            .AddTransition(idleShort, 3f)
            .AddTransition(idleLong, 2f)
            ;
        randomMoveLong
            .AddTransition(randomMoveShort, 1f)
            .AddTransition(idleShort, 3f)
            .AddTransition(idleLong, 3f)
            ;*
    }*/
    #endregion Init
}
