using Asteroid.Config;
using Asteroid.Random;
using Asteroid.Services;
using Asteroid.Time;
using Asteroid.UI;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace Asteroid.Gameplay
{
    public class GameplayManager : MonoBehaviour
    {
        [SerializeField]
        private MapBody _shipBody;
        [SerializeField]
        private UIController _uiController;

        private SimpleInputHandler _inputHandlet;
        private ServiceProvider _serviceProvider;
        private ShipController _shipController;
        private AsteroidController _asteroidController;
        private BulletController _bulletController;

        private void Awake()
        {
            _inputHandlet = new SimpleInputHandler();
            _shipController = new ShipController(_shipBody);
            _asteroidController = new AsteroidController();
            _bulletController = new BulletController();

            _serviceProvider = new ServiceProvider.ServiceProviderBuilder()
                .RegisterService<IAsteroidBodyFactory>(new ResourceAsteroidBodyFactory())
                .RegisterService<IBulletBodyFactory>(new ResourceBulletBodyFactory())
                .RegisterService<IRandomService>(new RandomService(0))
                .RegisterService<ITimeService>(new DefaultTimeService())
                .RegisterService<IConfigStore>(new StreamingAssetsConfigStore())
                .RegisterService<IMapConfinementHelper>(new MapConfinementHelper())
                .RegisterService<IInputHandler>(_inputHandlet)
                .RegisterService(_asteroidController)
                .RegisterService<IBulletController>(_bulletController)
                .RegisterService(_shipController)
                .Initialise();

            _uiController.Initialise(_serviceProvider);
            _shipBody.Initialise(_serviceProvider);
        }

        // Update is called once per frame
        void Update()
        {
            _inputHandlet.Tick();
            _uiController.Tick();
        }

        void FixedUpdate()
        {
            _shipController.FixedTick();
            _asteroidController.FixedTick();
            _bulletController.FixedTick();
        }
    }
}

