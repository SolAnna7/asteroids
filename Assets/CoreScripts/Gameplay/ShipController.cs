using Asteroid.Config;
using Asteroid.Services;
using Asteroid.Time;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace Asteroid.Gameplay
{
    public interface IShipController
    {
        public int CurrentHealth { get; }
        float RemainingInvulnerability { get; }

        public event Action OnDeath;
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
        private IMapBody _shipBody;

        public event Action OnDeath;

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
            _currentHealt = GameplaySetupHelper.CurrentHealth > 0 ? GameplaySetupHelper.CurrentHealth : _configStore.ShipMaxHealth;
            _invulnerablityLengthSec = _configStore.ShipInvulnerablilitySec;
            _inputHandler.Fire += OnFire;

            _invulnerablityLastTriggered = _timeService.Time;
            _shipBody.OnCollision += OnShipBodyCollision;
        }

        private void OnFire() => _bulletController.FireBullet(_shipBody.Position, _shipBody.Forward);

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
                OnDeath?.Invoke();
            }
        }

        public void FixedTick()
        {
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

            _shipBody.Move(_movementSpeedVector);

            float rotationVal =
                (_inputHandler.RotateLeft ? _rotationMultiplierDegreePerSec : 0) +
                (_inputHandler.RotateRight ? -_rotationMultiplierDegreePerSec : 0);
            if (rotationVal != 0)
            {
                _shipBody.Rotate(rotationVal * _timeService.FixedDeltaTime);
            }

        }

    }
}