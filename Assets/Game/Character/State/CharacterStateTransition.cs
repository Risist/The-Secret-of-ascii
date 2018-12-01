using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ReAnim;
using System;

namespace Character
{
    /// Allows to go into state if a given input is pressed
    public class CStateInput : StateComponent
    {
        public CStateInput(int _inputId) { inputId = _inputId; }
        public int inputId;
        public override bool CanEnter()
        {
            return controller.GetInput().IsInputPressed(inputId);
        }
    }
    /// allows to go into state if given cd is ready
    /// then resets it
    /// possible options to use only part of above functionalities
    public class CStateCd : StateComponent
    {
        public CStateCd(int _cdId, EMode _mode = EMode.EAll) { cdId = _cdId; mode = _mode; }
        public CStateCd(int _cdId, float _restartOffset, EMode _mode = EMode.EAll) { cdId = _cdId; mode = _mode; restartOffset = _restartOffset; }
        public int cdId;
        public float restartOffset = 0;
        public bool restartAfter = true;
        public enum EMode
        {
            EAll,           /// checks for cd readiness and resets it
            ERestartOnly,   /// restarts cd at animation init only
            EConditionOnly, /// only checks cd
        }
        EMode mode;
        public override bool CanEnter()
        {
            if (mode != EMode.ERestartOnly)
                return controller.IsCdReady(cdId);
            return true;
        }

        public CStateCd SetRestartAfter(bool b = true) { restartAfter = b; return this; }

        public override void InitPlayback(StateTransition transition)
        {
            if (mode != EMode.EConditionOnly)
                controller.RestartCd(cdId, restartOffset);
        }
        public override void OnAnimationEnd(AnimatorStateInfo stateInfo)
        {
            if (mode != EMode.EConditionOnly && restartAfter)
                controller.RestartCd(cdId, restartOffset);
        }
    }

    /// Dissalow to run the state if another is ready
    /// (not sure if usefull at all)
    public class CStateExclusiveReady : StateComponent
    {
        public CStateExclusiveReady(State _target) { target = _target; }
        public State target = null;

        public override bool CanEnter()
        {
            return !target.CanEnter();
        }
    }

    /// allows to run at most 2 animations at the time 
    /// used to ensure that animation transitions aren't that messy
    public class CStateMaxStateInstances : StateComponent
    {
        public CStateMaxStateInstances(State _exception = null) { exception = _exception; }
        public State exception;

        public override bool CanEnter()
        {
            return controller.GetCurrentState() == exception || controller.appliedStatesCount <= 1;
        }
        public override void InitPlayback(StateTransition transition)
        {
            controller.appliedStatesCount++;
        }
        public override void OnAnimationEnd(AnimatorStateInfo stateInfo)
        {
            controller.appliedStatesCount--;
        }
    }

    /// After succesful finish of animation playback state machine will auto transition to @target state
    public class CStateAutoTransition : StateComponent
    {
        public CStateAutoTransition(State _target) { target = _target; }
        public State target;
        public bool beggin = false;
        public bool end = true;

        public CStateAutoTransition ApplyOnBeggin(bool b = true) { beggin = b; return this; }
        public CStateAutoTransition ApplyOnEnd(bool b = true) { beggin = b; return this; }
        public override void OnAnimationUpdate(AnimatorStateInfo stateInfo)
        {
            if (beggin)
                controller.SetCurrentState(target);
        }
        public override void OnAnimationEnd(AnimatorStateInfo stateInfo)
        {
            if (end && state == controller.GetCurrentState())
                controller.SetCurrentState(target);
        }
        /*public override void FinishPlayback()
        {
            //if (state == controller.GetCurrentState())
            controller.SetCurrentState(target);
        }*/
    }

    /// allows to manual transition towards given target
    public class CStateTransition : StateComponent
    {
        public CStateTransition(State _target, Period _period) { target = _target; period = _period; }
        public CStateTransition(State _target) { target = _target; period = new Period(0.0f, 1.0f); }
        public State target;
        public Period period;
        public override void OnAnimationUpdate(AnimatorStateInfo stateInfo)
        {
            if (state == controller.GetCurrentState() 
                && period.min <= stateInfo.normalizedTime && (stateInfo.normalizedTime <= period.max || period.max >= 1f) )
            {
                if (target.CanEnter() == false)
                    return;
                
                target.InitPlayback(null);
                controller.SetCurrentState(target);
            }
        }
    }

    /// Input buffer is a thing which remembers that the state could use given transition, but the time is before transition allowed period
    /// So Input buffer remembers that transition should occur and even if conditions are not met fully transition goes on when in the right period
    /// this component resets input buffer on @target state
    public class CStateResetBuffer : StateComponent
    {
        public CStateResetBuffer(State _target = null) { target = _target; }
        public State target;
        public override void OnAnimationUpdate(AnimatorStateInfo stateInfo)
        {
            if (target != null)
                target.bufferedInput = false;
            else
                controller.ResetInputBuffer();
        }

    }

    /// restarts all cds on given state 
    /// TODO : kind of wtf it actually stays there? Probably legacy code
    class CStateResetCds : StateComponent
    {
        public CStateResetCds(float _cdState = 0)
        {
            cdState = _cdState;
        }
        public float cdState = 0;
        public override void InitPlayback(StateTransition transition)
        {
            controller.RestartCdAll();
        }
    }

    /// allows to transition when character is/is not at move
    class CStateConditionAtMove : StateComponent
    {
        public CStateConditionAtMove(bool _negation = false)
        {
            negation = _negation; 
        }
        public bool negation;
        public override bool CanEnter()
        {
            return negation ? !controller.GetInput().IsAtMove() : controller.GetInput().IsAtMove();
        }
    }
}