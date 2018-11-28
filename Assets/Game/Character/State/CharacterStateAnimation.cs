using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ReAnim;


namespace Character
{
    
    /// Plays animation one after another
    public class CStateSequentionalAnimation : StateComponent
    {
        public CStateSequentionalAnimation(string[] _animcodes, int _stateId = -1, int _step = 1) { animCodes = _animcodes; stateId = _stateId; step = _step; }
        public string[] animCodes;
        int current
        {
            get
            {
                return controller.GetCommonInt(stateId);
            }
            set
            {
                controller.SetCommonInt(stateId, value);
            }
        }
        public int stateId;
        public int step;

        public override void Init()
        {
            if (stateId == -1)
                stateId = controller.AddCommonInt();
        }
        public override void InitPlayback(StateTransition transition)
        {
            controller.PlayAnimation(animCodes[current]);
        }
        public override void OnAnimationBeggin(AnimatorStateInfo stateInfo)
        {
            foreach (var it in animCodes)
                controller.ResetAnimation(it);
            //controller.ResetAnimation(animCodes[(current + stateOffset) % animCodes.Length]);
            current = (current + step) % animCodes.Length;
        }
    }
    /// plays random animation
    public class CStateRandomAnimation : StateComponent
    {
        public CStateRandomAnimation(string[] _animcodes) { animCodes = _animcodes; }
        public string[] animCodes;
        int randed;

        public override void InitPlayback(StateTransition transition)
        {
            randed = Random.Range(0, animCodes.Length);
            controller.PlayAnimation(animCodes[randed]);
        }
        public override void OnAnimationBeggin(AnimatorStateInfo stateInfo)
        {
            foreach(var it in animCodes)
                controller.ResetAnimation(it);
        }
    }
    public class CStateAnimation : StateComponent
    {
        public CStateAnimation(string _animcode) { animCode = _animcode; }
        public string animCode;

        public override void InitPlayback(StateTransition transition)
        {
            controller.PlayAnimation(animCode);
        }
        public override void OnAnimationBeggin(AnimatorStateInfo stateInfo)
        {
            controller.ResetAnimation(animCode);
        }
    }
    public class CStateAnimationTrigger : StateComponent
    {
        public CStateAnimationTrigger(string _animcode) { animCode = _animcode; }
        public string animCode;

        public override void InitPlayback(StateTransition transition)
        {
            controller.PlayAnimationTrigger(animCode);
        }
    }

    public class CStateResetAnimation :StateComponent
    {
        public CStateResetAnimation(string _name) { names = new string[] { _name}; }
        public CStateResetAnimation(string[] _names) { names = _names; }
        public string[] names;
        public override void OnAnimationUpdate(AnimatorStateInfo stateInfo)
        {
            foreach (var it in names)
                controller.ResetAnimation(it);
        }
    }
}