using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ReAi
{
    public class ActionBase
    {
        public void SetMind(UnitMind s) { mind = s; }
        protected UnitMind mind;

        #region Utility
        public float baseUtility = 1.0f;
        public ActionBase SetBaseUtility(float s) { baseUtility = s; return this; } 

        /// how likely is the action to be chosen as next
        public virtual float GetUtility() { return baseUtility; }
        // if the action ends its performance, it can filter utility of all other actions
        public virtual float FilterUtility( ActionBase action ) { return action.GetUtility(); }
        #endregion Utility


        #region Performance
        /// Called every frame after selecting this action as currently executed
        /// <returns> has the action ended it's performance? </returns>
        public virtual bool PerformAction() { return true; }

        /// Called at the beginning of action execution
        public virtual void EnterAction() { }
        /// Called at the end of action execution
        public virtual void ExitAction() { }
        #endregion Performance
    }

    public class UnitMind
    {
        public UnitMind(InputManagerExternal _input)
        {
            input = _input;
            perception = _input.GetComponentInParent<AiPerceptionBase>();
            fraction = input.GetComponentInParent<AiFraction>();
            character = input.GetComponentInParent<CharacterController>();
        }
        
        public void Update()
        {
            if (currentAction != null && currentAction.PerformAction()) {
                SetCurrentAction(SelectNewAction());
            }
        }

        #region Actions
        public float utilityThreshold = 0;

        List<ActionBase> actions = new List<ActionBase>();
        public ActionBase AddAction(ActionBase action)
        {
            action.SetMind(this);
            actions.Add(action);
            if (actions.Count > actionChance.chances.Length)
                actionChance.chances = new float[actions.Count];
            return action;
        }
        ActionBase GetAction(int id) { return actions[id]; }


        ActionBase currentAction;        
        public void SetCurrentAction(ActionBase action)
        {
            if (currentAction != null)
                currentAction.ExitAction();

            currentAction = action;

            if (currentAction != null)
                currentAction.EnterAction();
        }

        static RandomChance actionChance = new RandomChance(new float[10]);
        ActionBase SelectNewAction()
        {
            if (currentAction != null)
                for (int i = 0; i < actions.Count; ++i){
                    float utility = currentAction.FilterUtility(actions[i] );
                    actionChance.chances[i] = utility >= utilityThreshold ? utility : 0;
                }
            else
                for (int i = 0; i < actions.Count; ++i){
                    float utility = actions[i].GetUtility();
                    actionChance.chances[i] = utility >= utilityThreshold ? utility : 0;
                }
            
            var id = actionChance.GetRandedId();
            if (id != -1)
                return actions[id];
            else return null;
        }
        #endregion Actions


        #region Input
        protected InputManagerExternal input;
        public Vector2 positionInput
        {
            set { input.positionInput = value; }
            get { return input.positionInput; }
        }
        public Vector2 directionInput
        {
            set { input.directionInput = value; }
            get { return input.directionInput; }
        }
        public bool key0
        {
            set { input.input[0] = value; }
            get { return input.input[0]; }
        }
        public bool key1
        {
            set { input.input[1] = value; }
            get { return input.input[1]; }
        }
        public bool key2
        {
            set { input.input[2] = value; }
            get { return input.input[2]; }
        }
        public bool key3
        {
            set { input.input[3] = value; }
            get { return input.input[3]; }
        }
        public void SetKey(int id, bool b)
        {
            if(id != -1)
                input.input[id] = b;
        }
        #endregion Input;

        #region Perception
        public AiPerceptionBase perception;
        public AiFraction fraction;
        public CharacterController character;
        public MemoryItem SearchInMemory(AiFraction.Attitude attitude)
        {
            return perception.SearchInMemory(fraction, attitude);
        }
        public MemoryItem SearchInMemory(string fractionName)
        {
            return perception.SearchInMemory(fractionName);
        }
        #endregion
    }
}
