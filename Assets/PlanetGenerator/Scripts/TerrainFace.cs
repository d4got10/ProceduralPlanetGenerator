using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PlanetGenerator
{
    public class TerrainFace
    {
        private MeshFilter _meshFilter;
        private int _resolution;

        private Vector3 _localXaxis;
        private Vector3 _localYaxis;
        private Vector3 _localZaxis;

        private float _seaLevel;

        private Vector3 _positionInNoise;

        private Noise _noise;
        private NoiseSettings[] _noiseSettings;
        private bool _useFirstLayerAsMask;

        public MinMax _minMax;

        public TerrainFace(MeshFilter meshFilter, int resolution, Vector3 normal, NoiseSettings[] noiseSettings, bool useFirstLayerAsMask, float seaLevel, ref MinMax minMax)
        {
            _meshFilter = meshFilter;
            _resolution = resolution;
            _localYaxis = normal;

            _localXaxis = new Vector3(_localYaxis.y, _localYaxis.z, _localYaxis.x);
            _localZaxis = Vector3.Cross(_localYaxis, _localXaxis);
            _localZaxis.Normalize();
            _localZaxis *= _localXaxis.magnitude;

            _seaLevel = seaLevel;

            _noiseSettings = noiseSettings;
            _noise = new Noise();
            _useFirstLayerAsMask = useFirstLayerAsMask;

            _minMax = minMax;
        }

        public void CreateMesh()
        {
            Mesh mesh = new Mesh();
            var vertices = new Vector3[(_resolution + 1) * (_resolution + 1)];
            var normals = new Vector3[(_resolution + 1) * (_resolution + 1)];

            for (int x = 0; x < _resolution + 1; x++)
            {
                for (int z = 0; z < _resolution + 1; z++)
                {
                    float xCoordinate = (x - _resolution / 2f) / _resolution;
                    float zCoordinate = (z - _resolution / 2f) / _resolution;
                    Vector3 pointOnUnitCube = _localYaxis + 2 * xCoordinate * _localXaxis + 2 * zCoordinate * _localZaxis;
                    Vector3 pointOnUnitSphere = (pointOnUnitCube + _meshFilter.transform.localPosition).normalized * _localYaxis.magnitude;
                    vertices[x * (_resolution + 1) + z] = pointOnUnitSphere;

                    float noiseValue = GetNoiseValue(vertices[x * (_resolution + 1) + z]);
                    vertices[x * (_resolution + 1) + z] *= noiseValue;

                    _minMax.AddValue(vertices[x * (_resolution + 1) + z].magnitude);
                }
            }

            var triangles = new int[_resolution * _resolution * 6];
            for (int i = 0; i < _resolution; i++)
            {
                for (int j = 0; j < _resolution; j++)
                {
                    int index = 6 * (i * _resolution + j);

                    triangles[index] = i * (_resolution + 1) + j;
                    triangles[index + 1] = (i + 1) * (_resolution + 1) + j;
                    triangles[index + 2] = (i + 1) * (_resolution + 1) + j + 1;


                    triangles[index + 3] = i * (_resolution + 1) + j;
                    triangles[index + 4] = (i + 1) * (_resolution + 1) + j + 1;
                    triangles[index + 5] = i * (_resolution + 1) + j + 1;
                }
            }

            mesh.vertices = vertices;
            mesh.triangles = triangles;
            mesh.normals = normals;
            mesh.RecalculateNormals();

            _meshFilter.sharedMesh = mesh;
        }

        private float GetNoiseValue(Vector3 point)
        {
            float value = 1;
            var settings = _noiseSettings[0];
            float firstLayerValue = settings.Evalute(point);
            float mask = _useFirstLayerAsMask ? (firstLayerValue > _seaLevel ? firstLayerValue - _seaLevel : 0) : 1;
            value += firstLayerValue;

            for (int i = 1; i < _noiseSettings.Length; i++)
            {
                settings = _noiseSettings[i];
                value += mask * settings.Evalute(point);
            }

            return Mathf.Max(1 + _seaLevel, value);
        }
    }
}
