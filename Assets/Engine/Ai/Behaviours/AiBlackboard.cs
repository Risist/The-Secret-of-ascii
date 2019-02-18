using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ReAi
{
    public class Blackboard
    {
        #region Distance to aim
        public float distanceToTarget;
        public float distanceAttractionAccumulator;

        #endregion Distance to aim
    }

    public class BFilterBlackboardResetDistanceAccumulator : BehaviourFilterBase
    {
        public override void Beggin()
        {
            blackboard.distanceAttractionAccumulator = 0;
            if (target != null)
            {
                blackboard.distanceToTarget = ((Vector2)transform.position - target.position).magnitude;
            }
            else blackboard.distanceToTarget = 5;
        }

    }
    public class BFilterBlackboardUpdateDistanceAccumulator : BehaviourFilterBase
    {
        public BFilterBlackboardUpdateDistanceAccumulator(float distanceScale = 10f, float decreaseValue = 0.9f)
        {
            this.decreaseValue = decreaseValue;
            this.distanceScale = distanceScale;
        }

        public float decreaseValue = 0.9f;
        public float distanceScale = 10f;

        public override EBehaviourStateReturn Update()
        {
            float dist = ((Vector2)transform.position - target.position).magnitude;
            float difference = (blackboard.distanceToTarget - dist);

            blackboard.distanceAttractionAccumulator *= decreaseValue;
            blackboard.distanceAttractionAccumulator += (difference * difference) * distanceScale * Time.deltaTime;
            

            blackboard.distanceToTarget = dist;

            Debug.Log(blackboard.distanceAttractionAccumulator);

            return EBehaviourStateReturn.ENextStateIfAll;
        }
    }

    public class BFilterBlackboardRequireDistance : BehaviourFilterBase
    {
        public BFilterBlackboardRequireDistance(float min)
        {
            this.min = min;
        }

        public float min;
        public override bool CanEnter(MemoryEvent target)
        {
            return blackboard.distanceAttractionAccumulator > min; 
        }
    }
}
