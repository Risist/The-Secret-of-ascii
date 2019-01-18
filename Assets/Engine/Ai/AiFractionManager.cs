using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ReAi
{
    class AiFractionManager : MonoBehaviour
    {           
        [System.Serializable]
        public struct FractionData
        {
            public string name;
            public int[] friendlyFractions;
            public int[] enemyFractions;
        }
        public FractionData[] fractions;

        public enum Attitude
        {
            friendly,
            neutral,
            enemy
        }


        public Attitude GetAttitude(int from, int to)
        {
            if (from == to)
                return Attitude.friendly;
            
            return GetAttitude(ref fractions[from], to);
        }
        Attitude GetAttitude(ref FractionData from, int to)
        {
            foreach (var it in from.enemyFractions)
                if (it == to)
                    return Attitude.enemy;

            foreach (var it in from.friendlyFractions)
                if (it == to)
                    return Attitude.friendly;

            return Attitude.neutral;
        }

        public int GetFractionId(string name)
        {
            int i = 0;
            foreach (var it in fractions)
            {
                if (it.name == name)
                    return i;
                ++i;
            }

            return -1;
        }

    }

}
