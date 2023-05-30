using Asteroid.Config;
using Asteroid.Random;
using Asteroid.Services;
using Asteroid.Time;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Asteroid.Gameplay
{
    public interface IAsteroidController
    {
        public event Action<int> OnAsteroidHitByBullet;
        public event Action OnAllAsteroidsDestroyed;
    }

    public class AsteroidController : ServiceProvider.IInitialisableService, IAsteroidController
    {
        private IConfigStore _configStore;
        private ITimeService _timeService;
        private IRandomService _random;
        private IAsteroidBodyFactory _factory;
        private ServiceProvider _serviceProvider;

        //todo map config
        private int _asteroidNum = 3;
        private IDictionary<IMapBody, AsteroidInstanceData> _asteroidInstances = new Dictionary<IMapBody, AsteroidInstanceData>();
        private ISet<IMapBody> _instancesToRemove = new HashSet<IMapBody>();
        private ISet<AsteroidInstanceData> _instancesToAdd = new HashSet<AsteroidInstanceData>();

        public event Action<int> OnAsteroidHitByBullet;
        public event Action OnAllAsteroidsDestroyed;

        public void Initialise(ServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
            _configStore = serviceProvider.GetService<IConfigStore>();
            _random = serviceProvider.GetService<IRandomService>();
            _factory = serviceProvider.GetService<IAsteroidBodyFactory>();
            _timeService = serviceProvider.GetService<ITimeService>();

            var mapExtents = _configStore.MapExtents;
            float asteroidFreeZoneAtStartupRadius = _configStore.AsteroidFreeZoneAtStartupRadius;

            for (int i = 0; i < _asteroidNum; i++)
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

                Vector2 direction = (Vector2)(Quaternion.Euler(0, 0, i * 360 / _asteroidNum) * Vector2.right);
                _asteroidInstances.Add(asteroidBody, new AsteroidInstanceData(asteroidBody,
                                                                              direction.normalized * _configStore.AsteroidSpeed,
                                                                              _random.NextFloat(-180, 180),
                                                                              _timeService.Time,
                                                                              0));
            }
        }

        private void AsteroidBody_OnCollision(IMapBody body, Vector2 position, IMapBody.MapBodyType type)
        {
            if (type == IMapBody.MapBodyType.Ship || type == IMapBody.MapBodyType.Bullet)
            {
                var asteroidData = _asteroidInstances[body];

                if (_timeService.Time - asteroidData.creationTime < _configStore.AsteroidInvulnerablilitySec)
                {
                    return;
                }

                if (asteroidData.generation < _configStore.AsteroidMaxGeneration)
                {
                    IMapBody child1 = _factory.CreateBody(body.Position, asteroidData.generation + 1);
                    IMapBody child2 = _factory.CreateBody(body.Position, asteroidData.generation + 1);

                    child1.Initialise(_serviceProvider);
                    child2.Initialise(_serviceProvider);

                    child1.OnCollision += AsteroidBody_OnCollision;
                    child2.OnCollision += AsteroidBody_OnCollision;

                    Vector2 diff = body.Position - position;
                    diff = Quaternion.Euler(0, 0, 90) * diff;
                    var speed1 = diff.normalized * _configStore.AsteroidSpeed;
                    var speed2 = -speed1;

                    _instancesToAdd.Add(new AsteroidInstanceData(child1,
                                                                 speed1 * _configStore.AsteroidSpeed,
                                                                 _random.NextFloat(-180, 180),
                                                                 _timeService.Time,
                                                                 asteroidData.generation + 1));
                    _instancesToAdd.Add(new AsteroidInstanceData(child2,
                                                                 speed2 * _configStore.AsteroidSpeed,
                                                                 _random.NextFloat(-180, 180),
                                                                 _timeService.Time,
                                                                 asteroidData.generation + 1));
                }
                else
                {
                    if (_asteroidInstances.Count == 1)
                    {
                        OnAllAsteroidsDestroyed?.Invoke();
                    }
                }

                body.OnCollision -= AsteroidBody_OnCollision;
                _instancesToRemove.Add(body);

                if(type == IMapBody.MapBodyType.Bullet)
                {
                    OnAsteroidHitByBullet?.Invoke(asteroidData.generation);
                }
            }
        }

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
                if (_instancesToRemove.Contains(instance.body))
                {
                    continue;
                }
                instance.body.Rotate(instance.rotation * _timeService.FixedDeltaTime);
                instance.body.Move(instance.speed * _timeService.FixedDeltaTime);
            }
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
