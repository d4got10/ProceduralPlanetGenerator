using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PlanetGenerator
{
    public class Planet : MonoBehaviour
    {
        [SerializeField] public MinMax MinMax;

        private int _resolution;
        public int Resolution
        {
            get { return _resolution; }
            set
            {
                if (value > 254) _resolution = 254;
                else if (value < 2) _resolution = 2;
                else _resolution = value;
            }
        }

        private int _faceCount = 2;
        public int FaceCount
        {
            get { return _faceCount; }
            set
            {
                if (value > 5) _faceCount = 5;
                else if (value < 1) _faceCount = 1;
                else _faceCount = value;
            }
        }

        public ColorSettings ColorSettings { get; set; }
        public float SeaLevel;

        public NoiseSettings[] PlanetNoiseSettings = new NoiseSettings[1];

        private bool _useFirstLayerAsMask = true;
        private PlanetPainter _planetPainter;

        [SerializeField, HideInInspector] private MeshFilter[] _meshFilters;
        private TerrainFace[] _terrainFaces;

        public void UpdatePlanetMesh()
        {
            DestroyFaces();
            Initialize();
            GenerateMesh();
        }

        private void DestroyFaces()
        {
            foreach (Transform child in transform)
            {
                if (child.name == "Face")
                {
                    Destroy(child);
                }
            }
        }

        private void Initialize()
        {
            Vector3[] directions = new Vector3[] { transform.up, -transform.up, transform.right, -transform.right, transform.forward, -transform.forward };
            MinMax = new MinMax();
            _useFirstLayerAsMask = true;

            if (_planetPainter == null)
                _planetPainter = new PlanetPainter();
            _planetPainter.UpdateSettings(ColorSettings);

            if (_meshFilters == null || _meshFilters.Length == 0 || _meshFilters[0] == null || _meshFilters.Length != 6 * _faceCount * _faceCount)
            {
                _meshFilters = new MeshFilter[6 * _faceCount * _faceCount];
            }

            for (int j = 0; j < 6; j++)
            {
                for (int i = 0; i < _faceCount * _faceCount; i++)
                {
                    if (_meshFilters[j * _faceCount * _faceCount + i] == null)
                    {
                        var obj = new GameObject("Face");
                        obj.transform.parent = transform;
                        obj.transform.localPosition = directions[j] * (_faceCount - 1);
                        var localX = directions[j + 2 > 5 ? j + 2 - 6 : j + 2];
                        var localY = directions[j + 4 > 5 ? j + 4 - 6 : j + 4];
                        obj.transform.localPosition += (localX) * _faceCount * ((i % _faceCount) - 0.5f * (_faceCount - 1))
                                                     + (localY) * _faceCount * ((i / _faceCount) - 0.5f * (_faceCount - 1));

                        obj.AddComponent<MeshRenderer>();

                        _meshFilters[j * _faceCount * _faceCount + i] = obj.AddComponent<MeshFilter>();
                        _meshFilters[j * _faceCount * _faceCount + i].sharedMesh = new Mesh();
                    }
                    _meshFilters[j * _faceCount * _faceCount + i].GetComponent<MeshRenderer>().sharedMaterial = ColorSettings.Material;
                }
            }
            if (PlanetNoiseSettings[0] == null)
                PlanetNoiseSettings[0] = new NoiseSettings(2, Mathf.Pow(2, 5), Mathf.Pow(2, 0), transform.position, NoiseSettings.NoiseTypes.Soft);
            for (int i = 1; i < PlanetNoiseSettings.Length; i++)
            {
                if (PlanetNoiseSettings[i] == null)
                    PlanetNoiseSettings[i] = new NoiseSettings(2, Mathf.Pow(2, i + (_faceCount / 3)), Mathf.Pow(2, i), transform.position, NoiseSettings.NoiseTypes.Rigid);
            }

            _terrainFaces = new TerrainFace[6 * _faceCount * _faceCount];
            for (int j = 0; j < 6; j++)
            {
                for (int i = 0; i < _faceCount * _faceCount; i++)
                {
                    _terrainFaces[j * _faceCount * _faceCount + i] = new TerrainFace(_meshFilters[j * _faceCount * _faceCount + i],
                                                                                        _resolution, directions[j] * (0.5f * _faceCount), PlanetNoiseSettings,
                                                                                        _useFirstLayerAsMask, SeaLevel, ref MinMax);
                }
            }
        }

        private void GenerateMesh()
        {
            MinMax.Clear();
            foreach (var face in _terrainFaces)
            {
                face.CreateMesh();
            }
            foreach (var filter in _meshFilters)
            {
                filter.transform.localPosition = Vector3.zero;
            }
            _planetPainter.UpdateElevation(MinMax);
            _planetPainter.UpdateColors();
            foreach (var filter in _meshFilters)
            {
                filter.gameObject.AddComponent<MeshCollider>();
            }
        }
    }
}