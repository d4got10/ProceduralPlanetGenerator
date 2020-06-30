using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PlanetGenerator
{
    public class PlanetPainter
    {
        private ColorSettings _colorSettings;
        private Texture2D _texture;
        private const int _textureResolution = 50;

        public void UpdateSettings(ColorSettings colorSettings)
        {
            _colorSettings = colorSettings;
            if (_texture == null)
                _texture = new Texture2D(_textureResolution, 1);
        }

        public void UpdateElevation(MinMax elevationMinMax)
        {
            _colorSettings.Material.SetVector("_elevationMinMax", new Vector4(elevationMinMax.Min, elevationMinMax.Max));
        }

        public void UpdateColors()
        {
            Color[] colors = new Color[_textureResolution];
            for (int i = 0; i < _textureResolution; i++)
            {
                colors[i] = _colorSettings.Gradient.Evaluate(i / (_textureResolution - 1f));
            }
            _texture.SetPixels(colors);
            _texture.Apply();
            _colorSettings.Material.SetTexture("_planetTexture", _texture);
        }
    }
}