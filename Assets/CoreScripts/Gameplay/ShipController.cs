using Asteroid.Config;
using Asteroid.Services;
using Asteroid.Time;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace Asteroid.Gameplay
{
    public interface IShipController
    {
        public int CurrentHealth { get; }
    }

    public class ShipController : IShipController, ServiceProvider.IInitialisableService
    {
        private IInputHandler _inputHandler;
        private IConfigStore _configStore;
        private ITimeService _timeService;
        private IBulletController _bulletController;

        private Vector2 _movementSpeedVector;
        private int _currentHealt;
        private float _rotationMultiplierDegreePerSec = 90;
        private float _shipSpeedupPerSec = 0.5f;
        private float _shipSlowdownPerSec = 0.05f;
        private float _maxShipSpeedPerSec = 0.02f;
        private float _invulnerablityLengthSec = 0;
        private float _invulnerablityLastTriggered = 0;
        private float _bulletFireInterval;
        private float _lastBulletFired;
        private IMapBody _shipBody;

        public ShipController(IMapBody shipBody)
        {
            this._shipBody = shipBody;
        }

        public int CurrentHealth => _currentHealt;
        public float RemainingInvulnerability => _invulnerablityLastTriggered - _timeService.Time + _invulnerablityLengthSec;

        public void Initialise(ServiceProvider serviceProvider)
        {
            _inputHandler = serviceProvider.GetService<IInputHandler>();
            _configStore = serviceProvider.GetService<IConfigStore>();
            _timeService = serviceProvider.GetService<ITimeService>();
            _bulletController = serviceProvider.GetService<IBulletController>();

            _rotationMultiplierDegreePerSec = _configStore.ShipRotationSpeed;
            _shipSpeedupPerSec = _configStore.ShipAccelearation;
            _shipSlowdownPerSec = _configStore.ShipDecelearation;
            _maxShipSpeedPerSec = _configStore.ShipMaxSpeed;
            _currentHealt = _configStore.ShipMaxHealth;
            _invulnerablityLengthSec = _configStore.ShipInvulnerablilitySec;
            _bulletFireInterval = _configStore.BulletFireInterval;
            _inputHandler.Fire += OnFire;

            _invulnerablityLastTriggered = _timeService.Time;
            _shipBody.OnCollision += OnShipBodyCollision;
        }

        private void OnFire()
        {
            if (_timeService.Time - _lastBulletFired > _bulletFireInterval)
            {
                _bulletController.FireBullet(_shipBody.Position, _shipBody.Forward);
                _lastBulletFired = _timeService.Time;
            }
        }

        private void OnShipBodyCollision(IMapBody body, Vector2 arg1, IMapBody.MapBodyType targetType)
        {
            if (RemainingInvulnerability > 0 || targetType != IMapBody.MapBodyType.Asteroid)
            {
                return;
            }

            _currentHealt -= 1;
            _invulnerablityLastTriggered = _timeService.Time;

            if (_currentHealt <= 0)
            {
                //todo
                Debug.Log("DEATH");
                UnityEditor.EditorApplication.isPlaying = false;
            }
        }

        public void FixedTick()
        {
            _shipBody.Move(_movementSpeedVector);

            float rotationVal =
                (_inputHandler.RotateLeft ? _rotationMultiplierDegreePerSec : 0) +
                (_inputHandler.RotateRight ? -_rotationMultiplierDegreePerSec : 0);
            if (rotationVal != 0)
            {
                _shipBody.Rotate(rotationVal * _timeService.FixedDeltaTime);
            }

            if (_inputHandler.Forward)
            {
                _movementSpeedVector += _shipSpeedupPerSec * _timeService.FixedDeltaTime * _shipBody.Forward;
            }
            if (_inputHandler.Backward)
            {
                _movementSpeedVector += _shipSlowdownPerSec * _timeService.FixedDeltaTime * -_shipBody.Forward;
            }

            if (_movementSpeedVector.magnitude > _maxShipSpeedPerSec)
            {
                _movementSpeedVector *= _maxShipSpeedPerSec / _movementSpeedVector.magnitude;
            }
        }

    }
}