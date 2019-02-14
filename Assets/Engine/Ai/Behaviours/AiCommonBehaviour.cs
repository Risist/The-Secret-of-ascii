using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/*
namespace ReAi
{
    public class BehaviourTimedChange : BehaviourBase
    {
        public float tChangeBehaviourMin;
        public float tChangeBehaviourMax;
        protected Timer tChangeBehaviour = new Timer();
        public BehaviourTimedChange SetTimeChangeBehaviour(float cd)
        {
            tChangeBehaviourMin = tChangeBehaviourMax = cd;
            return this;
        }
        public BehaviourTimedChange SetTimeChangeBehaviour(float cdMin, float cdMax)
        {
            tChangeBehaviourMin = cdMin;
            tChangeBehaviourMax = cdMax;
            return this;
        }


        public override void Beggin(MemoryEvent target)
        {
            base.Beggin(target);
            tChangeBehaviour.cd = Random.Range(tChangeBehaviourMin, tChangeBehaviourMax);
            tChangeBehaviour.Restart();
        }
        public override BehaviourBase Update()
        {
            return tChangeBehaviour.IsReadyRestart() ? GetNextBehaviour() : null;
        }
    }

    public class BehaviourIdle : BehaviourTimedChange
    {
        public override BehaviourBase Update()
        {
            data.directionInput = Vector2.zero;
            data.positionInput = Vector2.zero;
            data.rotationInput = Vector2.zero;

            for (int i = 0; i < data.keys.Length; ++i)
                data.keys[i] = false;

            return base.Update();
        }
    }

    public class BehaviourDebug : BehaviourBase
    {
        public BehaviourDebug(string s)
        {
            txt = s;
        }
        public string txt;
        public override BehaviourBase Update()
        {
            Debug.Log(txt);
            return GetNextBehaviour();
        }
    }
}
*/