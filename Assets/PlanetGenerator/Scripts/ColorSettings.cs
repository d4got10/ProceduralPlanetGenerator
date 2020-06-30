using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PlanetGenerator
{
    [CreateAssetMenu()]
    public class ColorSettings : ScriptableObject
    {
        public Gradient Gradient;
        public Material Material;
    }
}
