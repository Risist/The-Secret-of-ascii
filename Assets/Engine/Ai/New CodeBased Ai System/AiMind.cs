using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ReAi
{
    class ActionBase
    {
        /// for identification
        public string name = "";
        public void SetMind(UnitMind s) { mind = s; }
        protected UnitMind mind;

        #region Utility
        public float baseUtility = 1.0f;

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

    class UnitMind
    {
        public void Update()
        {
            if (currentAction != null && currentAction.PerformAction()) {
                SetCurrentAction(SelectNewAction());
            }
        }

        #region Actions
        public float utilityThreshold = 0;

        List<ActionBase> actions = new List<ActionBase>();
        public void AddAction(ActionBase action)
        {
            action.SetMind(this);
            actions.Add(action);
            if (actions.Count > actionChance.chances.Length)
                actionChance.chances = new float[actions.Count];
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
    }
}
