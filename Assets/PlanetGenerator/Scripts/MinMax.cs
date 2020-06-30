using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PlanetGenerator
{
    [System.Serializable]
    public class MinMax
    {
        [SerializeField] private float _min;
        public float Min { get { return _min; } private set { _min = value; } }

        [SerializeField] private float _max;
        public float Max { get { return _max; } private set { _max = value; } }

        public MinMax()
        {
            Min = float.MaxValue;
            Max = float.MinValue;
        }

        public void AddValue(float value)
        {
            if (value > Max)
                Max = value;
            if (value < Min)
                Min = value;
        }

        public void Clear()
        {
            Min = float.MaxValue;
            Max = float.MinValue;
        }
    }
}
