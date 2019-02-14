using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ReAi
{
    /*
    public class BehaviourRandomPosition : BehaviourTimedChange
    {
        public BehaviourRandomPosition(float _stopDistance, float _angleOffset, float _minDistance, float _maxDistance)
        {
            stopDistance = _stopDistance;
            angleOffset = _angleOffset;
            minDistance = _minDistance;
            maxDistance = _maxDistance;
        }

        /// distance from the center at which desired point will be choosen
        public float minDistance;
        public float maxDistance;

        /// possible angle offsets counted from center
        public float angleOffset;

        /// Distance at which execution of the behaviour ends up
        public float stopDistance;
        Vector2 aim;

        public override void Beggin(MemoryEvent target)
        {
            base.Beggin(target);
            aim = Quaternion.Euler(0, 0, Random.Range(-angleOffset, angleOffset)) * memory.transform.up * Random.Range(minDistance, maxDistance);
        }

        public override BehaviourBase Update()
        {
            data.directionInput = Vector2.zero;
            data.positionInput = aim;
            data.rotationInput = Vector2.zero;

            for (int i = 0; i < data.keys.Length; ++i)
                data.keys[i] = false;
            
            if (aim.sqrMagnitude < stopDistance || tChangeBehaviour.IsReady())
                return GetNextBehaviour();

            return base.Update();
        }
    }*/
}
