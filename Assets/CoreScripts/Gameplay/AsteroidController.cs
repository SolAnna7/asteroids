using Asteroid.Config;
using Asteroid.Random;
using Asteroid.Services;
using Asteroid.Time;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Asteroid.Gameplay
{
    /// <summary>
    /// The public api of the AsteroidController used by other services 
    /// </summary>
    public interface IAsteroidController
    {
        /// <summary>
        /// Invoked when an asteroid is hit by a bullet. The first parameter is the generation of the hit asteroid
        /// </summary>
        public event Action<int> OnAsteroidHitByBullet;
        /// <summary>
        /// Invoked when the last asteroid is destroyed
        /// </summary>
        public event Action OnAllAsteroidsDestroyed;
    }

    /// <summary>
    /// The main game logic class for handling asteroids
    /// </summary>
    public class AsteroidController : ServiceProvider.IInitialisableService, IAsteroidController
    {
        public event Action<int> OnAsteroidHitByBullet;
        public event Action OnAllAsteroidsDestroyed;

        private IConfigStore _configStore;
        private ITimeService _timeService;
        private IRandomService _random;
        private IAsteroidBodyFactory _factory;
        private ServiceProvider _serviceProvider;

        private IDictionary<IMapBody, AsteroidInstanceData> _asteroidInstances = new Dictionary<IMapBody, AsteroidInstanceData>();
        private ISet<IMapBody> _instancesToRemove = new HashSet<IMapBody>();
        private ISet<AsteroidInstanceData> _instancesToAdd = new HashSet<AsteroidInstanceData>();

        public void Initialise(ServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
            _configStore = serviceProvider.GetService<IConfigStore>();
            _random = serviceProvider.GetService<IRandomService>();
            _factory = serviceProvider.GetService<IAsteroidBodyFactory>();
            _timeService = serviceProvider.GetService<ITimeService>();

            int asteroidNum = _configStore.AsteroidStartNumber;
            Vector2 mapExtents = _configStore.MapExtents;
            float asteroidFreeZoneAtStartupRadius = _configStore.AsteroidFreeZoneAtStartupRadius;

            for (int i = 0; i < asteroidNum; i++)
            {
                int cntr = 0;
                Vector2 position = Vector2.zero;
                while (position.magnitude < asteroidFreeZoneAtStartupRadius || cntr > 30)
                {
                    position = _random.NextVec2(-mapExtents, mapExtents);
                    cntr++;
                }

                var asteroidBody = _factory.CreateBody(position, 0);
                asteroidBody.Initialise(serviceProvider);
                asteroidBody.OnCollision += AsteroidBody_OnCollision;

                Vector2 direction = (Vector2)(Quaternion.Euler(0, 0, i * 360 / asteroidNum) * Vector2.right);
                _asteroidInstances.Add(asteroidBody, new AsteroidInstanceData(asteroidBody,
                                                                              direction.normalized * _configStore.AsteroidSpeed,
                                                                              _random.NextFloat(-180, 180),
                                                                              _timeService.Time,
                                                                              0));
            }
        }

        /// <summary>
        /// Should be called every frame from unity FixedUpdate
        /// </summary>
        public void FixedTick()
        {
            foreach (var toRemove in _instancesToRemove)
            {
                _asteroidInstances.Remove(toRemove);
                toRemove.Destroy();
            }
            _instancesToRemove.Clear();
            foreach (var toAdd in _instancesToAdd)
            {
                _asteroidInstances.Add(toAdd.body, toAdd);
            }
            _instancesToAdd.Clear();

            foreach (var instance in _asteroidInstances.Values)
            {
                instance.body.Rotate(instance.rotation * _timeService.FixedDeltaTime);
                instance.body.MoveToPosition(instance.speed * _timeService.FixedDeltaTime);
            }
        }

        private void AsteroidBody_OnCollision(IMapBody parentBody, Vector2 position, IMapBody.MapBodyType type)
        {
            if (type == IMapBody.MapBodyType.Ship || type == IMapBody.MapBodyType.Bullet)
            {
                var asteroidData = _asteroidInstances[parentBody];

                if (_timeService.Time - asteroidData.creationTime < _configStore.AsteroidInvulnerablilitySec)
                {
                    return;
                }

                if (asteroidData.generation < _configStore.AsteroidMaxGeneration)
                {
                    Vector2 diff = parentBody.Position - position;
                    diff = Quaternion.Euler(0, 0, 90) * diff;
                    var speed = diff.normalized * _configStore.AsteroidSpeed;

                    //todo enable more than 2 child
                    CreateAsteroidChild(parentBody.Position, asteroidData.generation + 1, speed);
                    CreateAsteroidChild(parentBody.Position, asteroidData.generation + 1, -speed);
                }
                else
                {
                    if (_asteroidInstances.Count == 1)
                    {
                        OnAllAsteroidsDestroyed?.Invoke();
                    }
                }

                parentBody.OnCollision -= AsteroidBody_OnCollision;
                _instancesToRemove.Add(parentBody);

                if(type == IMapBody.MapBodyType.Bullet)
                {
                    OnAsteroidHitByBullet?.Invoke(asteroidData.generation);
                }
            }
        }

        private void CreateAsteroidChild(Vector2 position, int newGeneration, Vector2 speed)
        {
            IMapBody child1 = _factory.CreateBody(position, newGeneration);
            child1.Initialise(_serviceProvider);
            child1.OnCollision += AsteroidBody_OnCollision;
            _instancesToAdd.Add(new AsteroidInstanceData(child1,
                                     speed * _configStore.AsteroidSpeed,
                                     _random.NextFloat(-180, 180),
                                     _timeService.Time,
                                     newGeneration));
        }

        private class AsteroidInstanceData
        {
            public AsteroidInstanceData(IMapBody body, Vector2 speed, float rotation, float creationTime, int generation)
            {
                this.body = body;
                this.speed = speed;
                this.rotation = rotation;
                this.creationTime = creationTime;
                this.generation = generation;
            }

            public readonly IMapBody body;
            public readonly Vector2 speed;
            public readonly float rotation;
            public readonly float creationTime;
            public readonly int generation;
        }
    }
}
