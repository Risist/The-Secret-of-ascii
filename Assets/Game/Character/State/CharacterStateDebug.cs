using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ReAnim;
using System;

namespace Character
{

    public class CStateDebugValidAnimationName : StateComponent
    {
        public CStateDebugValidAnimationName(string[] _animationNames) { animationNames = _animationNames; }
        public CStateDebugValidAnimationName(string _animationName) { animationNames = new string[1] { _animationName }; }
        public string[] animationNames;

        bool b;
        public override void OnAnimationBeggin(AnimatorStateInfo stateInfo)
        {
            b = true;
        }
        public override void Update()
        {
            if (!b || controller.GetCurrentState() != state)
                return;

            var stateInfo = controller.GetAnimationStateInfo();

            foreach (var animName in animationNames)
                if (stateInfo.IsName(animName))
                    return;
            string s = "";
            foreach (var animName in animationNames)
                s += animName + ", ";
            Debug.Log("None of <" + s + "> are being played; state id  = " + state.stateId);
        }
        public override void OnAnimationEnd(AnimatorStateInfo stateInfo)
        {
            b = false;
        }
        public override void FinishPlayback()
        {
            b = false;
        }
    }

    public class CStateDebugKey : StateComponent
    {
        public CStateDebugKey(KeyCode _keyCode)
        {
            keyCode = _keyCode;
        }
        public KeyCode keyCode;
        public override bool CanEnter()
        {
            return Input.GetKey(keyCode);
        }
    }

}
