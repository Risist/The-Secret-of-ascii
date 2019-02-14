using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ReAi
{
    /*
     * Allows to maintain preconditions
     *
    public class BehaviourSkillBase : BehaviourTimedChange
    {
        protected CharacterStateController character;
        public override void Init()
        {
            base.Init();
            character = memory.GetComponentInParent<CharacterStateController>();
            Debug.Assert(character);
        }

        public override bool CanEnter(MemoryEvent target)
        {
            return true;
        }
    }



    public class BehaviourUseSkill : BehaviourTimedChange
    {
        public BehaviourUseSkill(float[] _probabilities)
        {
            probabilities = _probabilities;
        }
        public float[] probabilities;


        public BehaviourUseSkill SetPositionInputParam( float change, float scale = 1f)
        {
            positionInputChange = change;
            positionInputScale = scale;
            return this;
        }
        public float positionInputChange = 0.1f;
        public float positionInputScale = 1f;

        public BehaviourUseSkill SetDirectionInputParam(float change, float scale = 1f)
        {
            directionInputChange = change;
            directionInputScale = scale;
            return this;
        }
        public float directionInputChange = 0.1f;
        public float directionInputScale = 1f;


        public override BehaviourBase Update()
        {
            target = memory.SearchInMemory(EMemoryEvent.EEnemy);
            if (target == null || target.remainedTime.IsReady(target.knowledgeTime) )
                return GetNextBehaviour();

            for (int i = 0; i < data.keys.Length && i < probabilities.Length; ++i)
                data.keys[i] = Random.value < probabilities[i];

            data.directionInput = (Vector2)transform.position - target.position;

            data.positionInput = data.positionInput * (1 - positionInputChange) +
                (target.position - (Vector2)transform.position) * positionInputChange * positionInputScale;
            data.directionInput = data.directionInput * (1 - directionInputChange) +
                (target.position - (Vector2)transform.position) * directionInputChange * directionInputScale;
            data.rotationInput = Vector2.zero;

            return base.Update();
        }
    }*/
}