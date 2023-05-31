using Asteroid.Config;
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
    public interface IBulletController
    {
        void FireBullet(Vector2 position, Vector2 forward);
    }

    public class BulletController : IBulletController, ServiceProvider.IInitialisableService
    {
        private ServiceProvider _serviceProvider;
        private IConfigStore _configStore;
        private ITimeService _timeService;
        private IBulletBodyFactory _factory;
        private float _bulletSpeed;
        private float _bulletLifetime;
        private float _bulletFireInterval;
        private float _lastBulletFired;

        private readonly IDictionary<IMapBody, BulletInstanceData> _bulletInstances = new Dictionary<IMapBody, BulletInstanceData>();
        private readonly ISet<IMapBody> _instancesToRemove = new HashSet<IMapBody>();
        private readonly Stack<IMapBody> _bodyPool = new();

        public void FireBullet(Vector2 position, Vector2 forward)
        {
            if (_timeService.Time - _lastBulletFired < _bulletFireInterval)
            {
                return;
            }

            IMapBody body;
            if (_bodyPool.Count == 0)
            {
                body = _factory.CreateBody(position);
                body.Initialise(_serviceProvider);
                body.OnCollision += Bullet_OnCollision;
            }
            else
            {
                body = _bodyPool.Pop();
                body.Enabled = true;
            }

            body.Forward = forward;
            body.Position = position;

            _bulletInstances.Add(body, new BulletInstanceData(body, forward * _bulletSpeed, _timeService.Time));
            _lastBulletFired = _timeService.Time;
        }

        private void Bullet_OnCollision(IMapBody body, Vector2 position, IMapBody.MapBodyType type)
        {
            if (type == IMapBody.MapBodyType.Asteroid || type == IMapBody.MapBodyType.UFO)
            {
                _instancesToRemove.Add(body);
            }
        }

        public void FixedTick()
        {
            foreach (var toRemove in _instancesToRemove)
            {
                _bulletInstances.Remove(toRemove);
                toRemove.Enabled = false;
                _bodyPool.Push(toRemove);
            }
            _instancesToRemove.Clear();

            foreach (var instance in _bulletInstances.Values)
            {
                instance.body.Move(instance.speed * _timeService.FixedDeltaTime);

                if (_timeService.Time - instance.creationTime > _bulletLifetime)
                {
                    _instancesToRemove.Add(instance.body);
                }
            }

        }

        public void Initialise(ServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
            _configStore = serviceProvider.GetService<IConfigStore>();
            _timeService = serviceProvider.GetService<ITimeService>();
            _factory = serviceProvider.GetService<IBulletBodyFactory>();

            _bulletSpeed = _configStore.BulletSpeed;
            _bulletLifetime = _configStore.BulletLifetime;
            _bulletFireInterval = _configStore.BulletFireInterval;
            _lastBulletFired = -_bulletFireInterval;
        }


        private class BulletInstanceData
        {
            public BulletInstanceData(IMapBody body, Vector2 speed, float creationTime)
            {
                this.body = body;
                this.speed = speed;
                this.creationTime = creationTime;
            }

            public readonly IMapBody body;
            public readonly Vector2 speed;
            public readonly float creationTime;
        }
    }
}
