using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PlanetGenerator
{
    public class PlanetGenerator : MonoBehaviour
    {
        [SerializeField] private ColorSettings _hot;
        [SerializeField] private ColorSettings _snowy;
        [SerializeField] private ColorSettings _dusty;
        [SerializeField] private ColorSettings _earthLike;

        [Range(2, 128)] [SerializeField] private int _resolution = 64;
        [Range(1, 3)] [SerializeField] private int _faceCount = 2;

        public GameObject CurrentPlanet { get; private set; }

        private void Awake()
        {
            CreatePlanet(new Vector3(2, 2, 2), _hot);
            CreatePlanet(new Vector3(2, -2, 2), _snowy);
            CreatePlanet(new Vector3(-2, 2, 2), _dusty);
            CreatePlanet(new Vector3(-2, -2, 2), _earthLike);
        }

        public GameObject CreatePlanet(Vector3 position, ColorSettings settings)
        {
            GameObject planet = new GameObject("Planet");

            planet.transform.position = position;

            var planetComponent = planet.AddComponent<Planet>();

            planetComponent.ColorSettings = settings;
            planetComponent.Resolution = _resolution;
            planetComponent.FaceCount = _faceCount;
            planetComponent.PlanetNoiseSettings = new NoiseSettings[6];
            if (settings == _hot)
            {
                planetComponent.PlanetNoiseSettings[0] = new NoiseSettings(1, 8, 1, planet.transform.position, NoiseSettings.NoiseTypes.Soft);
                planetComponent.PlanetNoiseSettings[1] = new NoiseSettings(1, 16, 2, planet.transform.position, NoiseSettings.NoiseTypes.Soft);
            }
            planetComponent.UpdatePlanetMesh();

            CurrentPlanet = planet;

            return planet;
        }

        public GameObject CreatePlanet(Vector3 position)
        {
            float val = Random.value;
            ColorSettings color;

            if (val < 0.25f) color = _hot;
            else if (val < 0.5f) color = _snowy;
            else if (val < 0.75f) color = _dusty;
            else color = _earthLike;

            return CreatePlanet(position, color);
        }

        //DEBUG PURPOSE
        public void OnClick_SpawnPlanet()
        {
            if(CurrentPlanet.transform.position.x == 0 && CurrentPlanet.transform.position.y == 0)
                Destroy(CurrentPlanet);
            CreatePlanet(new Vector3(0,0, Random.Range(-2,4)));
        }
    }
}