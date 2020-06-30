using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PlanetGenerator
{
    [System.Serializable]
    public class NoiseSettings
    {
        public enum NoiseTypes { Soft, Rigid };
        public NoiseTypes NoiseType;
        public float Strength = 1;
        [Range(1, 100000)] public float StrengthDecrement = 1;
        public float Roughness = 1;
        public Vector3 Offset = new Vector3();

        private Noise _noise;

        public NoiseSettings(float strength, float strengthDecrement, float roughness, Vector3 offset, NoiseTypes noiseType)
        {
            Strength = strength;
            StrengthDecrement = strengthDecrement;
            Roughness = roughness;
            Offset = offset;
            NoiseType = noiseType;

            _noise = new Noise();
        }

        public float Evalute(Vector3 point)
        {
            switch (NoiseType)
            {
                case NoiseTypes.Soft:
                    return _noise.Evaluate(point * Roughness + Offset) * Strength / StrengthDecrement;
                case NoiseTypes.Rigid:
                    var value = _noise.Evaluate(point * Roughness + Offset);
                    return ((1 - Mathf.Abs(value)) * (1 - Mathf.Abs(value)) * 2 - 1) * Strength / StrengthDecrement;
                default:
                    return 0;
            }
        }
    }
}
