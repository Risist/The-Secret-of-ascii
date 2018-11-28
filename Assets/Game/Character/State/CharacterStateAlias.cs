using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ReAnim;


namespace Character
{
    public class CStateSetInt : StateComponent
    {
        public CStateSetInt(int _id, int _value)
        {
            id = _id;
            value = _value;
        }
        public int id;
        public int value;

        public override void InitPlayback(StateTransition transition)
        {
            controller.SetCommonInt(id, value);
        }
    }

    public class CStateIntEq : StateComponent
    {
        public CStateIntEq(int _id, int _value)
        {
            id = _id;
            value = _value;
        }
        public int id;
        public int value;

        public override bool CanEnter()
        {
            return controller.GetCommonInt(id) == value;
        }
    }
    /// if conditions are met runs aliased component
    public class CStateAlias: StateComponent
    {
        public List<StateComponent> conditions = new List<StateComponent>();
        public List<StateComponent> aliased = new List<StateComponent>();

        public CStateAlias AddCondition(StateComponent s)
        {
            conditions.Add(s);
            return this;
        }
        public CStateAlias AddAliased(StateComponent s)
        {
            aliased.Add(s);
            return this;
        }
        public override bool CanEnter()
        {
            if(CheckConditions())
            {
                foreach (var it in aliased)
                    if (!it.CanEnter())
                        return false;
            }
            return true;
        }
        public override void Init()
        {
            foreach (var it in conditions)
            {
                it.state = state;
                it.Init();
            }
            foreach (var it in aliased)
            {
                it.state = state;
                it.Init();
            }
        }

        public override void InitPlayback(StateTransition transition)
        {
            if (CheckConditions())
            {
                foreach (var it in aliased)
                    it.InitPlayback(transition);
            }
        }
        public override void FinishPlayback()
        {
            if (CheckConditions())
            {
                foreach (var it in aliased)
                    it.FinishPlayback();
            }
        }
        public override void Update()
        {
            if (CheckConditions())
            {
                foreach (var it in aliased)
                    it.Update();
            }
        }

        public override void OnAnimationBeggin(AnimatorStateInfo stateInfo)
        {
            if (CheckConditions())
            {
                foreach (var it in aliased)
                    it.OnAnimationBeggin(stateInfo);
            }
        }
        public override void OnAnimationEnd(AnimatorStateInfo stateInfo)
        {
            if (CheckConditions())
            {
                foreach (var it in aliased)
                    it.OnAnimationEnd(stateInfo);
            }
        }
        public override void OnAnimationUpdate(AnimatorStateInfo stateInfo)
        {
            if (CheckConditions())
            {
                foreach (var it in aliased)
                    it.OnAnimationUpdate(stateInfo);
            }
        }


        private bool CheckConditions()
        {
            foreach (var it in conditions)
                if (!it.CanEnter())
                    return false;
            return true;
        }
    }
}