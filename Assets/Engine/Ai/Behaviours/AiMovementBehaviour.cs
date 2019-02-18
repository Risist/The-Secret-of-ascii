using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ReAi
{
    public class BFilterStayInRange : BehaviourFilterBase
    {
        public BFilterStayInRange(float minDist, float maxDist, float positionInputChange = 0.1f)
        {
            minDistance = minDist;
            maxDistance = maxDist;
            this.positionInputChange = positionInputChange;
        }
        public float positionInputChange = 0.1f;

        public float minDistance;
        public float maxDistance;

        public override bool CanEnter(MemoryEvent target)
        {
            if (target == null)
                return false;

            var distSq = ((Vector2)transform.position - target.position).sqrMagnitude;

            return distSq > maxDistance*maxDistance || distSq < minDistance*minDistance;
        }

        public override EBehaviourStateReturn Update()
        {
            if (target == null)
                return EBehaviourStateReturn.ENextStateImmidiate;

            Vector2 toTarget = (Vector2)transform.position - target.position;
            var distSq = toTarget.sqrMagnitude;

            if (distSq > maxDistance * maxDistance)
            {
                /*if (AiNavmesh.instance)
                {
                    Vector2 desired = AiNavmesh.instance.EvaluateAttractionDir(transform.position,
                       (Vector2)transform.position - toTarget.normalized * 10,
                       AnimationCurve.Linear(0, 1.0f, 1.0f, 0.0f));

                    data.positionInput = Vector2.Lerp(data.positionInput, desired, positionInputChange);
                }
                else*/
                    data.positionInput = Vector2.Lerp(data.positionInput, -toTarget.normalized * 10, positionInputChange);

            }
            else if (distSq < minDistance * minDistance)
            {
                /*if (AiNavmesh.instance)
                {
                    Vector2 desired = AiNavmesh.instance.EvaluateAttractionDir(transform.position,
                       (Vector2)transform.position + toTarget.normalized * 10,
                       AnimationCurve.Linear(0, 1.0f, 1.0f, 0.0f));

                    data.positionInput = Vector2.Lerp(data.positionInput, desired, positionInputChange);
                }
                else*/
                    data.positionInput = Vector2.Lerp(data.positionInput, toTarget.normalized * 10, positionInputChange);
            }
            else
                return EBehaviourStateReturn.ENextStateImmidiate;


            return EBehaviourStateReturn.ENextStateIfAll;
        }

    }

    

}
