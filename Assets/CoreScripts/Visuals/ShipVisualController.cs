using Asteroid.Gameplay;
using Asteroid.Input;
using Asteroid.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Rendering.Universal;

namespace Asteroid.Visuals
{
    public class ShipVisualController : MonoBehaviour, ServiceProvider.IInitialisableService
    {
        [SerializeField]
        private Light2D _backLight;
        [SerializeField]
        private ParticleSystem _backEmission;
        [SerializeField]
        private ParticleSystem _backSideEmissionLeft;
        [SerializeField]
        private ParticleSystem _backSideEmissionRight;
        [SerializeField]
        private ParticleSystem _frontSideEmissionLeft;
        [SerializeField]
        private ParticleSystem _frontSideEmissionRight;
        [SerializeField]
        private ParticleSystem _frontEmissionLeft;
        [SerializeField]
        private ParticleSystem _frontEmissionRight;
        [SerializeField]
        private GameObject _shield;
        [SerializeField]
        private GameObject _shipExplosion;


        private IShipController _shipController;
        private IInputHandler _inputHandler;
        public void Initialise(ServiceProvider serviceProvider)
        {
            _shipController = serviceProvider.GetService<IShipController>();
            _inputHandler = serviceProvider.GetService<IInputHandler>();

            _shipController.OnDeath += OnDeath;
        }

        private void OnDeath()
        {
            _shipExplosion.SetActive(true);
        }

        private void Update()
        {
            if (_inputHandler.Forward)
            {
                _backEmission.Play();
            }
            else
            {
                _backEmission.Stop();
            }

            if (_inputHandler.Backward)
            {
                _frontEmissionLeft.Play();
                _frontEmissionRight.Play();
            }
            else
            {
                _frontEmissionLeft.Stop();
                _frontEmissionRight.Stop();
            }

            if (_inputHandler.RotateLeft)
            {
                _frontSideEmissionRight.Play();
                _backSideEmissionLeft.Play();
            }
            else
            {
                _frontSideEmissionRight.Stop();
                _backSideEmissionLeft.Stop();
            }

            if (_inputHandler.RotateRight)
            {
                _frontSideEmissionLeft.Play();
                _backSideEmissionRight.Play();
            }
            else
            {
                _frontSideEmissionLeft.Stop();
                _backSideEmissionRight.Stop();
            }

            _shield.SetActive(_shipController.RemainingInvulnerability > 0);
        }
    }
}
