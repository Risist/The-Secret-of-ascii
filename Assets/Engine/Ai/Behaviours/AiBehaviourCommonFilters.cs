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
    public class BFilterRequireCharacterState : BehaviourFilterBase
    {
        public BFilterRequireCharacterState(int stateId)
        {
            id = stateId;
        }
        int id;
        public Character.State state;

        List<BehaviourHolder> comboTransitions = new List<BehaviourHolder>();
        public BFilterRequireCharacterState AddComboTransition(BehaviourHolder s) { comboTransitions.Add(s); return this; }
        bool ComboTransition()
        {
            foreach (var it in comboTransitions)
                if (it.CanEnter(target))
                    return true;
            return false;
        }

        int delay;

        public bool requireTransition;
        public BFilterRequireCharacterState SetRequireTransition(bool b)
        {
            requireTransition = b;
            return this;
        }

        public override bool CanEnter(MemoryEvent target)
        {
            Character.State state = character.GetState(id);

            return
                /// can transition happen from current state?
                (requireTransition || character.GetCurrentState().GetTransition(state) != null) && 
                /// can enter atm?
                state.CanEnterSoft();
        }

        public override void Beggin()
        {
            state = character.GetState(id);
            delay = 5;
        }

        

        public override EBehaviourStateReturn Update()
        {
            if (requireTransition)
                return EBehaviourStateReturn.ENextStateIfAll;

            if(delay > 0)
            {
                delay--;
            }else if ( state != character.GetCurrentState() || ComboTransition() )
            {
                return EBehaviourStateReturn.ENextStateIfAll;
            }
            return EBehaviourStateReturn.EStillExecute;
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
            return character.GetIndicators().environmentIndicators[indicatorId].use &&
                character.GetIndicators().environmentIndicators[indicatorId].rayDistance < maximalDistance;
        }
        public override EBehaviourStateReturn Update()
        {
            if (CanJump())
            {
                if(press)
                    data.keys[id] = Random.value <= chance;
                return stateReturn;
            }
            return EBehaviourStateReturn.ENextStateIfAll;
        }
    }

    public class BFilterSwitchAnimationIndicator : BehaviourFilterBase
    {
        public BFilterSwitchAnimationIndicator(int id, bool b)
        {
            this.id = id;
            bNewValue = b;
        }
        public int id;
        public bool bNewValue;
        public override void Beggin()
        {
            character.GetIndicators().animationIndicators[id].use = bNewValue;
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
        }

        public override EBehaviourStateReturn Update()
        {
            /// if is too close to aim this behaviour has finished execution
            if (((Vector2)transform.position - targetPosition).sqrMagnitude < stopDistance * stopDistance)
                return EBehaviourStateReturn.ENextStateImmidiate;


            /*if (AiNavmesh.instance)
            {
                 Vector2 desired = AiNavmesh.instance.EvaluateAttractionDir(transform.position,
                    targetPosition,
                    AnimationCurve.Linear(0, 1.0f, 1.0f, 0.0f));

                data.positionInput = Vector2.Lerp(data.positionInput, desired, positionInputChange);
            }else*/
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
        public BFilterLookAround SetTargetDifference(float min, float max) { minTargetAngleDifference = min; maxTargetAngleDifference = max; return this; }
        public float minTargetAngleDifference = 45;
        public float maxTargetAngleDifference = 120;


        public float tChangeAngleMin = 1f;
        public float tChangeAngleMax = 1f;
        public BFilterLookAround SetTChangeAngle(float min, float max)
        {
            tChangeAngleMin = min;
            tChangeAngleMax = max;
            return this;
        }
        public BFilterLookAround SetTChangeAngle(float s)
        {
            tChangeAngleMin = s;
            tChangeAngleMax = s;
            return this;
        }
        Timer tChangeDesiredAngle = new Timer();

        /// current angle the agent wants to look at
        float angle;
        float GetNewAngle()
        {
            //return angle + minTargetAngleDifference + Random.value * (360 - 2 * minTargetAngleDifference);
            if(Random.value > 0.5f)
                return angle + Random.Range(minTargetAngleDifference, maxTargetAngleDifference);
            else
                return angle - Random.Range(minTargetAngleDifference, maxTargetAngleDifference);
        }
        public override void Beggin()
        {
            angle = transform.rotation.eulerAngles.z;
            angle = GetNewAngle();
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

            if (tChangeDesiredAngle.IsReadyRestart())
            {
                tChangeDesiredAngle.cd = Random.Range(tChangeAngleMin, tChangeAngleMax);

                angle = transform.rotation.eulerAngles.z;
                angle = GetNewAngle();
            }

            data.rotationInput = data.rotationInput * (1 - rotationInputChange) +
                desiredRotationInput * rotationInputChange;
            
            if ( Mathf.Abs(Mathf.DeltaAngle(transform.rotation.eulerAngles.z, angle)) < minAngleDifference)
            {
                return stateReturnClose;
            }

            return stateReturnNormal;
        }
    }

    public class BFilterPatrol : BehaviourFilterBase
    {
        public BFilterPatrol(float maxPatrolPathDistance, float positionInputChange = 0.1f, float closeDistance = 1.5f)
        {
            this.maxPatrolPathDistance = maxPatrolPathDistance;
            this.closeDistance = closeDistance;
            this.positionInputChange = positionInputChange;
        }

        PatrolManager.PatrolPath currentPath;
        PatrolManager.PatrolPath pretendingPath;
        int pointId;
        int direction;

        public float maxPatrolPathDistance;

        public float positionInputChange = 0.1f;
        public float closeDistance;

        public override bool CanEnter(MemoryEvent target)
        {
            if (!PatrolManager.instance)
            {
                Debug.LogWarning("No Patrol Manager on map");
                return false;
            }

            pretendingPath = PatrolManager.instance.GetRandomUnusedPath(transform.position - PatrolManager.instance.transform.position, maxPatrolPathDistance);

            return pretendingPath != null;
        }

        public override void Beggin()
        {
            currentPath = pretendingPath;
            pointId = currentPath.GetClosestPointId(transform.position- PatrolManager.instance.transform.position);
            if (pointId == 0)
                direction = 1;
            else if (pointId == currentPath.points.Length - 1)
                direction = -1;
            else direction = Random.Range(0, 2) * 2 - 1; // +1 or -1
        }

        public override EBehaviourStateReturn Update()
        {


            Vector2 toPoint = currentPath.points[pointId] - (Vector2)transform.position + (Vector2)PatrolManager.instance.transform.position;
            /*if(AiNavmesh.instance)
            {
                toPoint = AiNavmesh.instance.EvaluateAttractionDir(transform.position,
                    (Vector2)PatrolManager.instance.transform.position + currentPath.points[pointId],
                    AnimationCurve.Linear(0, 1.0f, 1.0f, 0.0f));
            }*/

            data.positionInput = Vector2.Lerp(data.positionInput, toPoint, positionInputChange);

            if (toPoint.sqrMagnitude < closeDistance * closeDistance)
            {
                pointId += direction;
                if((direction == 1 && pointId == currentPath.points.Length) || 
                    (direction == -1 && pointId == 0))
                    return EBehaviourStateReturn.ENextStateImmidiate;
            }

            return EBehaviourStateReturn.ENextStateIfAll;
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
    

}