using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ReAi
{
    public enum EAwareness
    {
        ENotAware,
        EAllerted,
        EAgressive
    }

    public class MindRef
    {
        public UnitMind mind;
    }

    /// distance from enemy- modes:
    /// close
    /// moderate
    /// far
    /// escape

    /// describes space conditions for ai behaviours
    public class Location : MindRef
    {
        public virtual Vector2 getVector()
        {
            return Vector2.zero;
        }
    }

    /// describes conditions put onto behaviour execution
    public class Condition : MindRef
    {
        public virtual bool CanEnter()
        {
            return true;
        }
    }

    class ConditionStateReady : Condition
    {
        public int stateId;
        public override bool CanEnter()
        {
            return mind.stateController.GetState(stateId).CanEnter();
        }
    }

    class ConditionCdReady : Condition
    {
        public int cdId;
        public override bool CanEnter()
        {
            return mind.stateController.IsCdReady(cdId);
        }
    }

    /// real action
    public class Behaviour
    {

    }





    public class UnitMind
    {
        public CharacterStateController stateController;
    }
}
