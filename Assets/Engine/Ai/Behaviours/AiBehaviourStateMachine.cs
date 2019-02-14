using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace ReAi
{
    

    public class BehaviourFilterBase
    {
        #region Data
        public BehaviourHolder behaviour;
        /// machine this behaviour is assigned to
        public BehaviourStateMachine machine { get { return behaviour.machine; } }
        public MemoryEvent target { get { return behaviour.target; } set { behaviour.target = value; } }
        /// indput data the behaviour comunicates with body
        public InputManagerExternal.InputData data { get { return behaviour.data; } }
        /// perception data about environment
        public AiPerceptionHolder memory { get { return behaviour.memory; } }
        public Transform transform { get { return character.transform; } }
        public CharacterStateController character{ get { return machine.character; } }
        #endregion Data

        public virtual void Init() { }

        public virtual bool CanEnter(MemoryEvent target) { return true; }

        public virtual void Beggin()
        {
        }
        public virtual void End() { }

        public enum EBehaviourStateReturn
        {   
            /// this filter still wants to execute behaviour
            EStillExecute,
            /// this filter ended up execution and waits for other filters to finish
            ENextStateIfAll,
            /// there is need for immidiate change of state
            ENextStateImmidiate,
            EDoNotTransition
        }
        public virtual EBehaviourStateReturn Update()
        {
            return EBehaviourStateReturn.ENextStateIfAll;
        }
    }
    public class BehaviourHolder
    {
        #region Data
        /// machine this behaviour is assigned to
        public BehaviourStateMachine machine;
        /// indput data the behaviour comunicates with body
        public InputManagerExternal.InputData data;

        /// perception data about environment
        public AiPerceptionHolder memory;

        public Transform transform { get { return memory.transform; } }
        /// body data
        public CharacterStateController character { get { return machine.character; } }
        /// saved target state
        public MemoryEvent target { get { return machine.target; } set { machine.target = value;  } }
        #endregion Data

        #region Filter
        /// Behaviour filters alters input data 
        /// so that character can behave in diferrent way during execution of this behaviour
        List<BehaviourFilterBase> filters = new List<BehaviourFilterBase>();
        public BehaviourHolder AddFilter(BehaviourFilterBase s)
        {
            filters.Add(s);
            s.behaviour = this;
            return this;
        }
        #endregion Filter;

        #region Transition
        public List<BehaviourHolder> transitions = new List<BehaviourHolder>();
        public List<float> transitionChances = new List<float>();
        public BehaviourHolder AddTransition(BehaviourHolder b, float chance = 1f)
        {
            transitions.Add(b);
            transitionChances.Add(chance);

            return this;
        }

        protected BehaviourHolder GetNextBehaviour()
        {
            float sum = 0;
            for (int i = 0; i < transitionChances.Count; ++i)
                if (transitions[i].CanEnter(target))
                    sum += transitionChances[i];

            if (sum == 0)
                return this;

            float randed = UnityEngine.Random.Range(0, sum);

            float lastSum = 0;
            for (int i = 0; i < transitionChances.Count; ++i)
                if (randed >= lastSum && randed <= lastSum + transitionChances[i])
                {
                    if (transitions[i].CanEnter(target))
                        return transitions[i];
                }
                else
                {
                    if (transitions[i].CanEnter(target))
                        lastSum += transitionChances[i];
                }

            return this;
        }
        public bool CanEnter(MemoryEvent target)
        {
            foreach (var it in filters)
                if (!it.CanEnter(target))
                    return false;
            return true;
        }
        #endregion Transition

        public void Beggin()
        {
            foreach (var it in filters)
                it.Beggin();
        }
        public void End()
        {
            foreach (var it in filters)
                it.End();
        }

        public BehaviourHolder Update()
        {
            bool canChangeBehaviour = true;
            bool hasToChangeBehaviour = false;
            bool doNotTransition = false;

            foreach (var it in filters)
            {
                var res = it.Update();
                if (res == BehaviourFilterBase.EBehaviourStateReturn.EStillExecute)
                    canChangeBehaviour = false;
                else if (res == BehaviourFilterBase.EBehaviourStateReturn.ENextStateImmidiate)
                    hasToChangeBehaviour = true;
                else if (res == BehaviourFilterBase.EBehaviourStateReturn.EDoNotTransition)
                    doNotTransition = true;
            }

            if (doNotTransition)
                return null;
            
            return hasToChangeBehaviour || canChangeBehaviour ? 
                 GetNextBehaviour() : null;
        }
    }
    
    
    public class BehaviourStateMachine
    {
        public List<BehaviourHolder> behaviours = new List<BehaviourHolder>();
        public InputManagerExternal.InputData data;
        public CharacterStateController character;
        public AiPerceptionHolder memory;

        BehaviourHolder current;
        public MemoryEvent target;

        public BehaviourHolder AddNewBehaviour()
        {
            var newBehaviour = new BehaviourHolder();
            newBehaviour.memory = memory;
            newBehaviour.data = data;
            newBehaviour.machine = this;

            behaviours.Add(newBehaviour);

            return newBehaviour;
        }
        public void SetCurrentBehaviour(BehaviourHolder behaviour)
        {
            if (current != null)
                current.End();
            current = behaviour;
            current.Beggin();
        }
        public void SetCurrentBehaviour(int id)
        {
            if (current != null)
                current.End();
            current = behaviours[id];
            current.Beggin();
        }
        public BehaviourHolder GetCurrentBehaviour()
        {
            return current;
        }
        
        public void Update()
        {
            Debug.Assert(current != null);
            var next = current.Update();

            if(next != null)
            {
                SetCurrentBehaviour(next);
            }
        }
    }

}