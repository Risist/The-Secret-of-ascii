using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ReAi
{
    /*
    public class BehaviourCheckSource : BehaviourJumpMovement
    {
        public BehaviourCheckSource(float _positionInputChange)
        {
            positionInputChange = _positionInputChange;
        }
        public float positionInputChange;
        /*public override void Beggin(MemoryEvent target)
        {
            base.Beggin(target);
        }*
        public override BehaviourBase Update()
        {
            for (int i = 0; i < data.keys.Length; ++i)
                data.keys[i] = false;

            data.positionInput = data.positionInput * (1 - positionInputChange) +
                (target.position - (Vector2)transform.position) * positionInputChange;
            data.directionInput = data.positionInput;
            data.rotationInput = Vector2.zero;
            return base.Update();
        }
    }

    /*
     * Agent walks towards choosen point
     * 
     * point is choosen as an average from two factors:
     * - random point in circle in forward to him
     * - random point around target exactPosition in circle of radius defined by targets movement speed and time elapsed
     * 
     *
    public class BehaviourSearch : BehaviourJumpMovement
    {
        public BehaviourSearch(float _positionInputChange, float _destinationChange)
        {
            positionInputChange = _positionInputChange;
            destinationChange = _destinationChange;
        }
        public float positionInputChange;
        public float forwardOffset;
        public float offsetRadius;
        public float stopDistance;
        public float searchAreaScale = 1f;
        /// how fast will destination be changed while in update
        public float destinationChange;

        public BehaviourSearch SetStopDistance( float s)
        {
            stopDistance = s;
            return this;
        }
        public BehaviourSearch SetSearchAreaScale(float s)
        {
            searchAreaScale = s;
            return this;
        }
        public BehaviourSearch SetOffset(float radius, float distance = 0f)
        {
            offsetRadius = radius;
            forwardOffset = distance;
            return this;
        }

        Vector2 targetPosition;

        /// exact desired position of the target
        Vector2 GetNewPosition(MemoryEvent target)
        {
            Vector2 targetPosition = target.exactPosition +
                Random.insideUnitCircle
                * target.remainedTime.ElapsedTime() * target.direction.magnitude * searchAreaScale;
            targetPosition += (Vector2)transform.position + Vector2.up * offsetRadius + Random.insideUnitCircle * offsetRadius;
            targetPosition *= 0.5f;
            
            return targetPosition;
        }

        public override void Beggin(MemoryEvent target)
        {
            base.Beggin(target);
            if (target == null)
            {
                targetPosition = transform.position;
                return;
            }
            targetPosition = GetNewPosition(target);
        }
        public override BehaviourBase Update()
        {
            for (int i = 0; i < data.keys.Length; ++i)
                data.keys[i] = false;

            
            targetPosition = targetPosition * (1 - destinationChange) + GetNewPosition(target) * destinationChange;
            data.positionInput =
                data.positionInput * (1 - positionInputChange) +
                (targetPosition - (Vector2)transform.position) * positionInputChange;

            
            data.directionInput = data.positionInput*2;
            data.rotationInput = Vector2.zero;
            
            
            if (data.positionInput.sqrMagnitude < stopDistance * stopDistance)
            {
                return GetNextBehaviour();
            }

            return base.Update();
        }
    }

    public class BehaviourLookAt : BehaviourTimedChange
    {
        public BehaviourLookAt(float _rotationInputChange, float _destinationChange)
        {
            rotationInputChange = _rotationInputChange;
            destinationChange = _destinationChange;
        }
        public float rotationInputChange;

        public float forwardOffset;
        public float offsetRadius;
        public float stopDistance;
        public float searchAreaScale = 1f;
        /// how fast will destination be changed while in update
        public float destinationChange;

        public BehaviourLookAt SetSearchAreaScale(float s)
        {
            searchAreaScale = s;
            return this;
        }
        public BehaviourLookAt SetOffset(float radius, float distance = 0f)
        {
            offsetRadius = radius;
            forwardOffset = distance;
            return this;
        }
        
        Vector2 targetPosition;

        Vector2 GetNewPosition(MemoryEvent target)
        {
            return target.position;
        }

        public override void Beggin(MemoryEvent target)
        {
            base.Beggin(target);
            if (target == null)
            {
                targetPosition = transform.position;
                return;
            }
            targetPosition = GetNewPosition(target);
        }
        public override BehaviourBase Update()
        {

            for (int i = 0; i < data.keys.Length; ++i)
                data.keys[i] = false;


            targetPosition = targetPosition * (1 - destinationChange) + GetNewPosition(target) * destinationChange;

            data.rotationInput =
                data.rotationInput * (1 - rotationInputChange) +
                (targetPosition - (Vector2)transform.position) * rotationInputChange;

            data.directionInput = Vector2.zero;
            //data.directionInput = data.rotationInput * 2;
            data.positionInput = Vector2.zero;

            return base.Update();
        }
    }

    /*
     * Agent will look at random positions around. Target location will change everytime by at least given angle
     *
    public class BehaviourLookAround : BehaviourTimedChange
    {
        public BehaviourLookAround(float _rotationInputChange)
        {
            rotationInputChange = _rotationInputChange;
        }
        public float rotationInputChange;

        /// after changing target angle it will change at least by given amount (in degrees)
        public float minTargetAngleDifference = 45;
        public BehaviourLookAround SetMinAngleDifference(float s)
        {
            minTargetAngleDifference = s;
            return this;
        }

        public float tChangeAngleMin = 1f;
        public float tChangeAngleMax = 1f;
        BehaviourLookAround SetTChangeAngle(float min, float max)
        {
            tChangeAngleMin = min;
            tChangeAngleMax = max;
            return this;
        }
        BehaviourLookAround SetTChangeAngle(float s)
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
            return angle + minTargetAngleDifference + Random.value*(360-2*minTargetAngleDifference);
        }

        public override void Beggin(MemoryEvent target)
        {
            base.Beggin(target);

            tChangeDesiredAngle.cd = Random.Range(tChangeAngleMin, tChangeAngleMax);

            if(target != null)
            {
                angle = Vector2.SignedAngle(target.position - (Vector2)transform.position, Vector2.up);
            }
            else
            {
                angle = Random.Range(0, 360);
            }
        }
        public override BehaviourBase Update()
        {
            for (int i = 0; i < data.keys.Length; ++i)
                data.keys[i] = false;


            if(tChangeDesiredAngle.IsReadyRestart())
            {
                tChangeDesiredAngle.cd = Random.Range(tChangeAngleMin, tChangeAngleMax);
                angle = GetNewAngle();
            }

            data.positionInput = Vector2.zero;
            data.directionInput = Vector2.zero;
            data.rotationInput = data.rotationInput * (1 - rotationInputChange) + 
                (Vector2)(Quaternion.Euler(0, 0, angle) * Vector2.up * rotationInputChange);

            return base.Update();
        }
    }
    */

}
