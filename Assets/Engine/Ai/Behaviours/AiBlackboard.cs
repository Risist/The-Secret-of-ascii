using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ReAi
{
    class Blackboard
    {
        public Vector2 aim;
        public float aimAngle;

        public float distanceToTargetSq;


        enum ECombatIntention
        {
            /// Combat behaviour
            damageEnemy,
            strife,
            flee,
            charge
        }
    }
}
