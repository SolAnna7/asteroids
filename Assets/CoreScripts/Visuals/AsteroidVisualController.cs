using Asteroid.Gameplay;
using Asteroid.Services;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Asteroid.Visuals
{
    public class AsteroidVisualController : MonoBehaviour, ServiceProvider.IInitialisableService
    {
        [SerializeField]
        private GameObject _explosionPrefab;
        [SerializeField]
        private float _explosionLifetimeSec;
        [SerializeField]
        private float _scaleMultiplier;

        private IAsteroidController _asteroidController;

        public void Initialise(ServiceProvider serviceProvider)
        {
            _asteroidController = serviceProvider.GetService<IAsteroidController>();

            _asteroidController.OnAsteroidCollison += OnAsteroidCollison; ;
        }

        private void OnAsteroidCollison(IMapBody obj)
        {
            var explosionInstance = Instantiate(_explosionPrefab);
            explosionInstance.transform.position = obj.Position;
            explosionInstance.transform.localScale = _scaleMultiplier * obj.Radius * Vector3.one;

            Destroy(explosionInstance, _explosionLifetimeSec);
        }
    }

}
