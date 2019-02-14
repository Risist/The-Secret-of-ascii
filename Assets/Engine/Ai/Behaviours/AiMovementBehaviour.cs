using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ReAi
{
    /*
     * the behaviour will automatically mark dash ablility 
     * when there is oportunity to jump over a wall
     *
    public class BehaviourJumpMovement : BehaviourTimedChange
    {
        const int jumpIndicatorId = 1;
        const int jumpKeyId = 3;
        CharacterUiIndicator indicators;
        public bool enableJump = true;
        public float jumpDistance = 0.85f;

        public BehaviourJumpMovement EnableJump(bool s)
        {
            enableJump = s;
            return this;
        }
        public BehaviourJumpMovement SetJumpDistance(float s)
        {
            jumpDistance = s;
            return this;
        }
        bool CanJump()
        {
            return indicators.environmentIndicators[jumpIndicatorId].use &&
                indicators.environmentIndicators[jumpIndicatorId].rayDistance < jumpDistance;
        }

        public override void Init()
        {
            base.Init();
            indicators = memory.GetComponentInParent<CharacterStateController>().GetIndicators();
        }

        public override BehaviourBase Update()
        {
            if(enableJump && CanJump())
            {
                data.keys[jumpKeyId] = true;
            }
            return base.Update();
        }


    }

    /*
     * 
     *
    public class BehaviourSkilledMovement : BehaviourJumpMovement
    {

    }

    
    public class BehaviourStayInRange : BehaviourJumpMovement
    {
        public BehaviourStayInRange(float _positionInputScale, float _directionInputScale)
        {
            positionInputScaleAlly = positionInputScale = _positionInputScale;
            directionInputScale = _directionInputScale;
        }
        public BehaviourStayInRange(float _positionInputScale, float _directionInputScale, float _positionInputScaleAlly)
        {
            positionInputScale = _positionInputScale;
            positionInputScaleAlly = _positionInputScaleAlly;
            directionInputScale = _directionInputScale;
        }

        public float positionInputScale;
        public float positionInputScaleAlly;
        public float directionInputScale;
        public float rotationInputChange = 0.01f;

        public int nAllyConsidered = 3;

        public BehaviourStayInRange SetAlly(int n, float minD, float maxD)
        {
            nAllyConsidered = n;
            minDistanceAlly = minD;
            maxDistanceAlly = maxD;
            return this;
        }
        public BehaviourStayInRange SetDistanceTarget(float min, float max)
        {
            minDistance = min;
            maxDistance = max;
            return this;
        }
        public BehaviourStayInRange SetRotationInputChange(float s)
        {
            rotationInputChange = s;
            return this;
        }

        /// agent will try to stay in given distance to target
        public float minDistance = 4f;
        public float maxDistance = 7f;

        /// agent will try to stay in given distance to allies
        public float minDistanceAlly = 4f;
        public float maxDistanceAlly = 40f;


        int StayInRange(MemoryEvent target, float min, float max,
            float positionInputScale,
            float directionInputScale)
        {
            Vector2 toTarget = (target.position - (Vector2)transform.position);
            float lengthToTarget = toTarget.sqrMagnitude;

            if (lengthToTarget < min * min)
            {
                data.positionInput = data.positionInput * (1 - positionInputScale) - toTarget * positionInputScale;
                data.directionInput = data.directionInput * (1 - directionInputScale) - toTarget * directionInputScale;

                return -1;
            }
            else if (lengthToTarget > max * max)
            {
                data.positionInput = data.positionInput * (1 - positionInputScale) + toTarget * positionInputScale;
                data.directionInput = data.directionInput * (1 - directionInputScale) + toTarget * directionInputScale;

                return 1;
            }
            else
            {
                data.positionInput *= (1 - positionInputScale);
                data.directionInput = data.directionInput * (1 - directionInputScale) + toTarget * directionInputScale;
                return 0;
            }

        }

        int lastPhase = 0;
        int prePhase = 0;

        public override void Beggin(MemoryEvent target)
        {
            base.Beggin(target);

            for (int _i = 0; _i < data.keys.Length; ++_i)
                data.keys[_i] = false;
            data.positionInput = Vector2.zero;
            data.directionInput = Vector2.zero;
            data.rotationInput = Vector2.zero;
        }
        public override BehaviourBase Update()
        {
            for (int _i = 0; _i < data.keys.Length; ++_i)
                data.keys[_i] = false;
            data.positionInput = Vector2.zero;
            data.directionInput = Vector2.zero;
            data.rotationInput = Vector2.zero;

            int phase = StayInRange(target, minDistance, maxDistance, positionInputScale,  directionInputScale);
            

            if( (lastPhase == -1 || prePhase == -1) && phase != -1)
            {
                data.rotationInput = data.directionInput;
            }

            prePhase = lastPhase;
            lastPhase = phase;
            /// todo - dashing in and out

            /*var allyIt = memory.SearchInMemory(EMemoryEvent.EAlly);

            int i = 0;
            while (allyIt != null && i < nAllyConsidered)
            {
                StayInRange(allyIt, minDistanceAlly, maxDistanceAlly, positionInputScaleAlly, 0f);
                ++i;
                allyIt = memory.SearchInMemory(EMemoryEvent.EAlly, i);
            }*/


            /*if (phase0)
            {
                data.rotationInput = data.rotationInput*(1-rotationInputChange) +  data.directionInput*rotationInputChange;
            }else
            {
                data.rotationInput *= (1-rotationInputChange);
            }*
            return base.Update();
        }
    }*/

    

}
