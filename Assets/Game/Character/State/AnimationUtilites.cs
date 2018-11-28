using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace ReAnim
{

    [System.Serializable]
    public struct Period
    {
        public Period(float _min = 0.0f, float _max = 1.0f)
        {
            min = _min;
            max = _max;
        }
        [Range(0.0f, 1.0f)]
        public float min;
        [Range(0.0f, 1.0f)]
        public float max;
        public bool IsIn(float currentTime) { return min <= currentTime && (max >= currentTime || min >= 1f); }
    }

}
