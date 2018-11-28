using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ReAnim;
using System;

namespace Character
{
    public class CStateInput : StateComponent
    {
        public CStateInput(int _inputId) { inputId = _inputId; }
        public int inputId;
        public override bool CanEnter()
        {
            return controller.GetInput().IsInputPressed(inputId);
        }
    }
    public class CStateCd : StateComponent
    {
        public CStateCd(int _cdId, EMode _mode = EMode.EAll) { cdId = _cdId; mode = _mode; }
        public CStateCd(int _cdId, float _restartOffset, EMode _mode = EMode.EAll) { cdId = _cdId; mode = _mode; restartOffset = _restartOffset; }
        public int cdId;
        public float restartOffset = 0;
        public bool restartAfter = true;
        public enum EMode
        {
            EAll,
            ERestartOnly,
            EConditionOnly,
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
    public class CStateExclusiveReady : StateComponent
    {
        public CStateExclusiveReady(State _target) { target = _target; }
        public State target = null;

        public override bool CanEnter()
        {
            return !target.CanEnter();
        }
    }

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
    public class CStateTransition : StateComponent
    {
        public CStateTransition(State _target) { target = _target; }
        public State target;
        public override void OnAnimationUpdate(AnimatorStateInfo stateInfo)
        {

            Debug.Log("In function");
            if (state == controller.GetCurrentState())
            {
                Debug.Log("this state");
                if (target.CanEnter() == false)
                    return;

                Debug.Log("CanEnter");

                target.InitPlayback(null);
                controller.SetCurrentState(target);

                Debug.Log("Entered");
            }
        }
    }

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