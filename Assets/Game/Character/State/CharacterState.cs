using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using ReAnim;


namespace Character
{
    [Serializable]
    public class StateTransition
    {
        public Period period;
        public State target;
        public bool bufferInput = true;
        public Timer timer = new Timer(0);
    }

    enum EStateType
    {
        ENeutral,
        EAction,
        EDisability,
    }

    public class StateComponent
    {
        public State state;

        protected CharacterController controller {
            get { return state.controller; }
        }

        public virtual void Init() { }

        public virtual bool CanEnter() { return true; }

        public virtual void InitPlayback(StateTransition transition) {}
        public virtual void FinishPlayback() { }
        public virtual void Update() { }

        public virtual void OnAnimationBeggin(AnimatorStateInfo stateInfo){}
        public virtual void OnAnimationEnd(AnimatorStateInfo stateInfo) {}
        public virtual void OnAnimationUpdate(AnimatorStateInfo stateInfo) {}
    }

    public class State 
    {
        public int stateId;

        /// When currently executing state has transition to this one and this State requirements are meet
        /// then this flag remembers that at the beggining of transition available perion the animation should init playback
        [NonSerialized] public bool bufferedInput = false;
        public CharacterController controller;

        public void Update(float time)
        {
            foreach (var it in components)
                it.Update();

            foreach (var it in transitions)
                if( it.timer.isReady() && (it.target.bufferedInput || it.target.CanEnter() ) )
                {
                    var p = it.period;
                    //Debug.Log(time + ", " + p.min + ", " + p.max);

                    if ( p.min <= time  && ( time <= p.max || p.max >= 1f) )
                    {
                        it.target.bufferedInput = false;
                        it.target.InitPlayback(it);
                        it.timer.restart();
                        break;
                    }
                    else if (time < p.min && it.bufferInput)
                        it.target.bufferedInput = true;
                    // else ignore input
                }//*/
        }

        #region Transitions
        List<StateTransition> transitions = new List<StateTransition>();

        public StateTransition GetTransition(State state)
        {
            foreach (var it in transitions)
                if (it.target == state)
                    return it;
            return null;
        }
        public State AddTransition(State target, Period period, bool bufferInput = true)
        {
            var tr = new StateTransition();
            tr.target = target;
            tr.period = period;
            tr.bufferInput = bufferInput;
            transitions.Add(tr);
            return this;
        }
        public State AddTransition(State target, bool bufferInput = true)
        {
            var tr = new StateTransition();
            tr.target = target;
            tr.period = new Period(0f,1f);
            tr.bufferInput = bufferInput;
            transitions.Add(tr);
            return this;
        }
        public State AddTransition(State target, Period period,Timer timer, bool bufferInput = true)
        {
            var tr = new StateTransition();
            tr.target = target;
            tr.period = period;
            tr.bufferInput = bufferInput;
            tr.timer = timer;
            transitions.Add(tr);
            return this;
        }
        public State AddTransition(State target, Timer timer, bool bufferInput = true)
        {
            var tr = new StateTransition();
            tr.target = target;
            tr.period = new Period(0f,1f);
            tr.bufferInput = bufferInput;
            tr.timer = timer;
            transitions.Add(tr);
            return this;
        }
        public State AddTransition(State target, Period period, int cdId, bool bufferInput = true)
        {
            var tr = new StateTransition();
            tr.target = target;
            tr.period = period;
            tr.bufferInput = bufferInput;
            tr.timer = controller.GetCdTimer(cdId);
            transitions.Add(tr);
            return this;
        }
        public State AddTransition(State target, int cdId, bool bufferInput = true)
        {
            var tr = new StateTransition();
            tr.target = target;
            tr.period = new Period(0f,1f);
            tr.bufferInput = bufferInput;
            tr.timer = controller.GetCdTimer(cdId);
            transitions.Add(tr);
            return this;
        }
        public State AddTransition(StateTransition transition) { transitions.Add(transition); return this; }
        #endregion Transitions

        #region Virtual 
        /// does the state meet it's requirements to play?
        public bool CanEnter()
        {
            foreach (var it in components)
                if (!it.CanEnter())
                    return false;
            return true;
        }

        /// event initializing playback. 
        /// Its not the exact moment when animation starts to play but rather when animation is sheduled
        public void InitPlayback(StateTransition transition)
        {
            controller.SetCurrentState(this);

            foreach (var it in components)
                it.InitPlayback(transition);
        }
        public void FinishPlayback()
        {
            controller.ResetInputBuffer();
            foreach (var it in components)
                it.FinishPlayback();
        }

        /// animation events -> used to make sure the events will be applied in correct time (can varry because of animation blending )
        public void OnAnimationBeggin(AnimatorStateInfo stateInfo)
        {
            if(controller.GetPreviousState() != null)
                controller.GetPreviousState().FinishPlayback();
            controller.ResetInputBuffer();
            foreach (var it in components)
                it.OnAnimationBeggin(stateInfo);
        }
        public void OnAnimationUpdate(AnimatorStateInfo stateInfo)
        {
            foreach (var it in components)
                it.OnAnimationUpdate(stateInfo);
        }
        public void OnAnimationEnd(AnimatorStateInfo stateInfo)
        {
            foreach (var it in components)
                it.OnAnimationEnd(stateInfo);
        }
        #endregion Virtual


        #region Component
        List<StateComponent> components = new List<StateComponent>();
        public State AddComponent(StateComponent component)
        {
            component.state = this;
            component.Init();
            components.Add(component);
            return this;
        }
        #endregion Component
    }

    public class CStateDebug : StateComponent
    {
        public CStateDebug(string _messageInit, string _messageBeggin, string _messageUpdate, string _messageEnd)
        {
            messageInit = _messageInit;
            messageBeggin = _messageBeggin;
            messageUpdate = _messageUpdate;
            messageEnd = _messageEnd;
        }
        public string messageInit;
        public string messageBeggin;
        public string messageUpdate;
        public string messageEnd;

        public override void InitPlayback(StateTransition transition)
        {
            if(messageInit != "")
                Debug.Log(messageInit);
        }
        public override void OnAnimationBeggin(AnimatorStateInfo stateInfo)
        {
            if (messageBeggin != "")
                Debug.Log(messageBeggin);
        }
        public override void OnAnimationUpdate(AnimatorStateInfo stateInfo)
        {
            if (messageUpdate != "")
                Debug.Log(messageUpdate);
        }
        public override void OnAnimationEnd(AnimatorStateInfo stateInfo)
        {
            if (messageEnd != "")
                Debug.Log(messageEnd);
        }
    }
}