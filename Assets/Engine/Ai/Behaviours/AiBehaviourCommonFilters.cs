using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ReAi
{

    /// conditions
    /// 

    public class BFilterDistanceFromAim : BehaviourFilterBase
    {
        public BFilterDistanceFromAim(float min, float max)
        {
            minDistance = min;
            maxDistance = max;
        }
        public float minDistance;
        public float maxDistance;

        public override bool CanEnter(MemoryEvent target)
        {
            float distSq = ((Vector2)transform.position - target.position).sqrMagnitude;
            return distSq >= minDistance * minDistance && distSq <= maxDistance * maxDistance;
        }
    }
    public class BFilterCharacterStateAvailable : BehaviourFilterBase
    {
        public BFilterCharacterStateAvailable(int stateId)
        {
            state = character.GetState(stateId);
        }
        public Character.State state;
        
        public override bool CanEnter(MemoryEvent target)
        {
            /// TODO character state can enter caching
            return
                /// can transition happen from current state?
                character.GetCurrentState().GetTransition(state) != null && 
                /// can enter atm?
                state.CanEnter();
        }
    }

    /// condition for target cone

    /// Execution time control
    /// 

    public class BFilterTimedExecution : BehaviourFilterBase
    {
        public BFilterTimedExecution(float min, float max, EBehaviourStateReturn stateReturn = EBehaviourStateReturn.ENextStateIfAll)
        {
            tMin = min;
            tMax = max;
            this.stateReturn = stateReturn;
        }

        public EBehaviourStateReturn stateReturn;
        public float tMin;
        public float tMax;

        Timer tExecutionTime = new Timer();

        public override void Beggin()
        {
            tExecutionTime.cd = Random.Range(tMin, tMax);
            tExecutionTime.Restart();
        }
        public override EBehaviourStateReturn Update()
        {
            return tExecutionTime.IsReady() ? stateReturn : EBehaviourStateReturn.EStillExecute;
        }
    }
    public class BFilterCharacterStateChange : BehaviourFilterBase
    {
        BFilterCharacterStateChange(EBehaviourStateReturn stateReturn = EBehaviourStateReturn.ENextStateIfAll)
        {
            this.stateReturn = stateReturn;
        }
        public EBehaviourStateReturn stateReturn;
        Character.State initState;
        public override void Beggin()
        {
            initState = character.GetCurrentState();
        }
        public override EBehaviourStateReturn Update()
        {
            return character.GetCurrentState() != initState ? stateReturn : EBehaviourStateReturn.EStillExecute;
        }
    }

    /// data

    public class BFilterResetInputBeggin : BehaviourFilterBase
    {
        public BFilterResetInputBeggin(bool position = true, bool direction = true, bool rotation = true, bool keys = true)
        {
            this.position = position;
            this.rotation = rotation;
            this.direction = direction;
            this.keys = keys;
        }
        public bool position;
        public bool rotation;
        public bool direction;
        public bool keys;

        public override void Beggin()
        {
            if (position)
                data.positionInput = Vector2.zero;
            if (rotation)
                data.rotationInput = Vector2.zero;
            if (direction)
                data.directionInput = Vector2.zero;
            if (keys)
                for (int i = 0; i < data.keys.Length; ++i)
                    data.keys[i] = false;
        }
    }
    public class BFilterResetInput : BehaviourFilterBase
    {
        public BFilterResetInput(bool position = true, bool direction = true, bool rotation = true, bool keys = true)
        {
            this.position = position;
            this.rotation = rotation;
            this.direction = direction;
            this.keys = keys;
        }
        public bool position;
        public bool rotation;
        public bool direction;
        public bool keys;

        public override EBehaviourStateReturn Update()
        {
            if (position)
                data.positionInput = Vector2.zero;
            if (rotation)
                data.rotationInput = Vector2.zero;
            if (direction)
                data.directionInput = Vector2.zero;
            if (keys)
                for(int i = 0; i < data.keys.Length; ++i)
                    data.keys[i] = false;

            return base.Update();
        }
    }

    public class BFilterPositionToAim : BehaviourFilterBase
    {
        public BFilterPositionToAim(float inputChange, float inputScale = 1f)
        {
            this.inputChange = inputChange;
            this.inputScale = inputScale;
        }
        public float inputChange = 0.1f;
        public float inputScale = 1f;

        public override EBehaviourStateReturn Update()
        {
            data.positionInput = data.positionInput * (1 - inputChange) +
                (target.position - (Vector2)transform.position) * inputChange * inputScale;

            return base.Update();
        }
    }
    public class BFilterDirectionToAim : BehaviourFilterBase
    {
        public BFilterDirectionToAim(float inputChange, float inputScale = 1f)
        {
            this.inputChange = inputChange;
            this.inputScale = inputScale;
        }
        public float inputChange = 0.1f;
        public float inputScale = 1f;

        public override EBehaviourStateReturn Update()
        {
            data.directionInput = data.directionInput * (1 - inputChange) +
                (target.position - (Vector2)transform.position) * inputChange * inputScale;

            return base.Update();
        }
    }
    public class BFilterRotationToAim : BehaviourFilterBase
    {
        public BFilterRotationToAim(float inputChange, float inputScale = 1f)
        {
            this.inputChange = inputChange;
            this.inputScale = inputScale;
        }
        public float inputChange = 0.1f;
        public float inputScale = 1f;

        public override EBehaviourStateReturn Update()
        {
            data.rotationInput = data.rotationInput * (1 - inputChange) +
                (target.position - (Vector2)transform.position) * inputChange * inputScale;

            return base.Update();
        }
    }

    public class BFilterAllignDirectionToPosition : BehaviourFilterBase
    {
        public BFilterAllignDirectionToPosition(float inputChange)
        {
            this.inputChange = inputChange;
        }
        public float inputChange;
        public override EBehaviourStateReturn Update()
        {
            data.directionInput = data.directionInput * (1 - inputChange) + data.positionInput * inputChange;
            return base.Update();
        }
    }
    public class BFilterAllignRotationToDirection : BehaviourFilterBase
    {
        public BFilterAllignRotationToDirection(float inputChange)
        {
            this.inputChange = inputChange;
        }
        public float inputChange;
        public override EBehaviourStateReturn Update()
        {
            data.rotationInput = data.rotationInput * (1 - inputChange) + data.directionInput * inputChange;
            return base.Update();
        }
    }

    public class BFilterKeyPress : BehaviourFilterBase
    {
        public BFilterKeyPress(int id, float chance = 1f)
        {
            this.id = id;
            this.chance = chance;
        }
        public int id;
        public float chance;

        public override EBehaviourStateReturn Update()
        {
            data.keys[id] = Random.value <= chance;
            return base.Update();
        }
    }
    public class BFilterKeysPress : BehaviourFilterBase
    {
        public BFilterKeysPress(float[] chance)
        {
            this.chance = chance;
        }
        public float[] chance;

        public override EBehaviourStateReturn Update()
        {
            for(int i = 0; i < chance.Length; ++i)
                data.keys[i] = Random.value <= chance[i];
            return base.Update();
        }
    }
    public class BFilterKeyPressIndicator : BFilterKeyPress
    {
        public BFilterKeyPressIndicator(int keyId = 3, int indicatorId = 1, float keyChance = 1f, float maximalDistance = 0.85f) : base(keyId, keyChance)
        {
            this.indicatorId = indicatorId;
            this.maximalDistance = maximalDistance;
        }
        CharacterUiIndicator indicators;
        public float maximalDistance = 0.85f;
        public int indicatorId = 1;

        public bool press = true;
        public EBehaviourStateReturn stateReturn = EBehaviourStateReturn.ENextStateIfAll;
        public BFilterKeyPressIndicator SetStateReturn(EBehaviourStateReturn stateReturn)
        {
            this.stateReturn = stateReturn; 
            return this;
        }
        public BFilterKeyPressIndicator SetPress(bool press)
        {
            this.press = press;
            return this;
        }


        bool CanJump()
        {
            return indicators.environmentIndicators[indicatorId].use &&
                indicators.environmentIndicators[indicatorId].rayDistance < maximalDistance;
        }
        public override void Init()
        {
            base.Init();
            indicators = character.GetIndicators();
        }
        public override EBehaviourStateReturn Update()
        {
            if (CanJump())
            {
                if(press)
                    data.keys[id] = data.keys[id] || Random.value <= chance;
                return stateReturn;
            }
            return base.Update();
        }
    }

    public class BFilterSearch : BehaviourFilterBase
    {
        public BFilterSearch(float positionInputChange = 0.1f, float stopDistance = 1.5f, float searchAreaScale = 1f)
        {
            this.positionInputChange = positionInputChange;
            this.stopDistance = stopDistance;
            this.searchAreaScale = searchAreaScale;
        }
        public float positionInputChange = 0.1f;
        public float stopDistance = 1.5f;

        public float searchAreaScale = 1f;

        public float baseSearchArea = 0f;
        public BFilterSearch SetBaseSearchArea(float s) { baseSearchArea = s; return this; }

        Vector2 targetPosition;
        Vector2 GetNewPosition()
        {
            if (target == null)
                return transform.position;

            Vector2 targetPosition = target.exactPosition +
                Random.insideUnitCircle
                * ( baseSearchArea + target.remainedTime.ElapsedTime() * target.direction.magnitude * searchAreaScale);
            return targetPosition;
        }

        public override void Beggin()
        {
            if (target == null)
                targetPosition = (Vector2)transform.position + Random.insideUnitCircle * baseSearchArea;
            else
                targetPosition = GetNewPosition();

            Debug.Log(targetPosition - (Vector2)transform.position);
        }

        public override EBehaviourStateReturn Update()
        {
            /// if is too close to aim this behaviour has finished execution
            if (((Vector2)transform.position - targetPosition).sqrMagnitude < stopDistance * stopDistance)
                return EBehaviourStateReturn.ENextStateImmidiate;

            data.positionInput =
                data.positionInput * (1 - positionInputChange) +
                (targetPosition - (Vector2)transform.position) * positionInputChange;

            return base.Update();
        }
    }
    public class BFilterLookAround : BehaviourFilterBase
    {
        public BFilterLookAround(float rotationChange = 0.1f, EBehaviourStateReturn stateReturnNormal = EBehaviourStateReturn.ENextStateIfAll)
        {
            rotationInputChange = rotationChange;
            this.stateReturnNormal = stateReturnNormal;
        }
        public float rotationInputChange;
        public BFilterLookAround SetMinTargetDifference(float s) { minTargetAngleDifference = s; return this; }
        public float minTargetAngleDifference = 45;
        
        /// current angle the agent wants to look at
        float angle;
        float GetNewAngle()
        {
            return angle + minTargetAngleDifference + Random.value * (360 - 2 * minTargetAngleDifference);
        }
        public override void Beggin()
        {
            angle = transform.rotation.eulerAngles.z;
            angle = GetNewAngle();
            Debug.Log(angle);
        }

        public BFilterLookAround SetCloseAngle(float minAngleDifference, EBehaviourStateReturn stateReturnClose = EBehaviourStateReturn.ENextStateIfAll)
        {
            this.minAngleDifference = minAngleDifference;
            this.stateReturnClose = stateReturnClose;
            return this;
        }
        public float minAngleDifference = 0f;
        public EBehaviourStateReturn stateReturnClose = EBehaviourStateReturn.ENextStateIfAll;
        public EBehaviourStateReturn stateReturnNormal = EBehaviourStateReturn.ENextStateIfAll;

        public override EBehaviourStateReturn Update()
        {
            Vector2 desiredRotationInput = Quaternion.Euler(0, 0, angle) * Vector2.up;
            
            data.rotationInput = data.rotationInput * (1 - rotationInputChange) +
                desiredRotationInput * rotationInputChange;
            
            if ( Mathf.Abs(Mathf.DeltaAngle(transform.rotation.eulerAngles.z, angle)) < minAngleDifference)
            {
                return stateReturnClose;
            }

            return stateReturnNormal;
        }
    }

    /// change target to one from memory
    public class BFilterReadMemory : BehaviourFilterBase
    {
        public BFilterReadMemory(EMemoryEvent eventType, EMemoryState eventState = EMemoryState.EKnowledge, int eventId = 0)
        {
            this.eventType = eventType;
            this.eventState = eventState;
            this.eventId = eventId;
        }
        ///which event to read form memory, in order of sort
        public int eventId;
        public EMemoryEvent eventType;
        public EMemoryState eventState;

        public BFilterReadMemory SetTarget(bool b)
        {
            setTarget = b;
            return this;
        }
        public bool setTarget = true;
        public BFilterReadMemory SetStateReturn(EBehaviourStateReturn find, EBehaviourStateReturn notFind)
        {
            stateReturnFind = find;
            stateReturnNotFind = notFind;
            return this;
        }
        public EBehaviourStateReturn stateReturnFind = EBehaviourStateReturn.ENextStateIfAll;
        public EBehaviourStateReturn stateReturnNotFind = EBehaviourStateReturn.ENextStateIfAll;

        public override EBehaviourStateReturn Update()
        {
            var ev = memory.SearchInMemory(eventType, eventId);
            if(ev != null && ev.GetState() == eventState)
            {
                target = ev;
                return stateReturnFind;
            }
            return stateReturnNotFind;
        }
    }

    /*
     * Manages state interruption
     * e.g. If character perception will find information about higher priority event
     * the behaviour will transition towards given entry point after a short delay
     */
    public class BFilterStateInteruption : BehaviourFilterBase
    {
        struct UpdateMemoryData
        {
            public BehaviourHolder entry;
            public EMemoryEvent eventType;
            public EMemoryState eventState;
            public ECurrentState lastState;
        }

        #region Transition Entry Points
        public BehaviourHolder enemyKnowledgeEntry;
        public BehaviourHolder enemyShadeEntry;
        public BehaviourHolder enemyPainEntry;
        public BehaviourHolder enemyPainShadeEntry;

        public BehaviourHolder noiseKnowledgeEntry;
        public BehaviourHolder noiseShadeEntry;
        public BehaviourHolder noiseTouchEntry;
        public BehaviourHolder noiseTouchShadeEntry;

        public BehaviourHolder neutralEntry;
        #endregion Transition Entry Points

        #region Delay
        public float tDelayMin = 0.1f;
        public float tDelayMax = 0.25f;
        Timer tDelay = new Timer();
        #endregion Delay

        #region Transition
        enum ECurrentState
        {
            EEnemyShade,
            EEnemyKnowledge,
            EEnemyPain,
            EEnemyPainShade,

            ENoiseShade,
            ENoiseKnowledge,
            ENoiseTouch,
            ENoiseTouchShade,
            
            ENeutral
        }
        ECurrentState currentState = ECurrentState.ENeutral;

        BehaviourHolder currentTransition;
        MemoryEvent targetTransition;
        void ScheduleTransition( BehaviourHolder towards, MemoryEvent target)
        {
            tDelay.cd = Random.Range(tDelayMin, tDelayMax);
            tDelay.Restart();
            currentTransition = towards;
            targetTransition = target;
        }

        void UpdateMemoryEnemy()
        {
            var ev = memory.SearchInMemory(EMemoryEvent.EEnemy);
            if (ev != null)
            {
                var state = ev.GetState();
                if (state == EMemoryState.EKnowledge && currentState != ECurrentState.EEnemyKnowledge)
                {
                    currentState = ECurrentState.EEnemyKnowledge;
                    ScheduleTransition(enemyKnowledgeEntry, ev);
                    return;
                }

                if (state == EMemoryState.EShade && currentState != ECurrentState.EEnemyKnowledge)
                {
                    currentState = ECurrentState.EEnemyKnowledge;
                    ScheduleTransition(enemyShadeEntry, ev);
                    return;
                }
            }
            else
                UpdateMemoryNoise();
        }
        void UpdateMemoryPain()
        {
            var ev = memory.SearchInMemory(EMemoryEvent.EEnemy_Pain);
            if (ev != null)
            {
                var state = ev.GetState();
                if (state == EMemoryState.EKnowledge && currentState != ECurrentState.EEnemyPain)
                {
                    currentState = ECurrentState.EEnemyPain;
                    ScheduleTransition(enemyPainEntry, ev);
                    return;
                }

                if (state == EMemoryState.EShade && currentState != ECurrentState.EEnemyPainShade)
                {
                    currentState = ECurrentState.EEnemyPainShade;
                    ScheduleTransition(enemyPainShadeEntry, ev);
                    return;
                }
            }
            else
                UpdateMemoryNoise();
        }
        void UpdateMemoryNoise()
        {
            var ev = memory.SearchInMemory(EMemoryEvent.ENoise);
            if (ev != null)
            {
                var state = ev.GetState();
                if (state == EMemoryState.EKnowledge && currentState != ECurrentState.ENoiseKnowledge)
                {
                    currentState = ECurrentState.ENoiseKnowledge;
                    ScheduleTransition(noiseKnowledgeEntry, ev);
                    return;
                }

                if (state == EMemoryState.EShade && currentState != ECurrentState.ENoiseShade)
                {
                    currentState = ECurrentState.ENoiseShade;
                    ScheduleTransition(noiseShadeEntry, ev);
                    return;
                }
            }
            else
                UpdateMemoryTouch();
        }
        void UpdateMemoryTouch()
        {
            var ev = memory.SearchInMemory(EMemoryEvent.ENoise_Touch);
            if( ev != null)
            {
                var state = ev.GetState();
                if(state == EMemoryState.EKnowledge && currentState != ECurrentState.ENoiseTouch)
                {
                    currentState = ECurrentState.ENoiseTouch;
                    ScheduleTransition(noiseTouchEntry, ev);
                    return;
                }

                if (state == EMemoryState.EShade && currentState != ECurrentState.ENoiseTouchShade)
                {
                    currentState = ECurrentState.ENoiseTouchShade;
                    ScheduleTransition(noiseTouchShadeEntry, ev);
                    return;
                }
            }
            else
                UpdateMemoryNeutral();
        }
        void UpdateMemoryNeutral()
        {
            if(currentState != ECurrentState.ENeutral)
            {
                currentState = ECurrentState.ENeutral;
                ScheduleTransition(neutralEntry, null);
            }
        }
        #endregion Transition

        public override EBehaviourStateReturn Update()
        {
            if(currentTransition != null && tDelay.IsReady())
            {
                /// Do transition 
                machine.SetCurrentBehaviour(currentTransition);
                target = targetTransition;

                return EBehaviourStateReturn.EDoNotTransition;
            }
            else /// check if there is a higher priority event happening
            {
                UpdateMemoryEnemy();
            }

            return EBehaviourStateReturn.ENextStateIfAll;
        }
    }

}