using Asteroid.Gameplay;
using Asteroid.Services;
using Asteroid.Time;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;

namespace Asteroid.UI
{
    public class UIController : MonoBehaviour
    {
        [SerializeField]
        private TMP_Text _lifeText;
        [SerializeField]
        private TMP_Text _scoreText;

        private ShipController _shipController;
        private ITimeService _timeService;
        public void Initialise(ServiceProvider serviceProvider)
        {
            _shipController = serviceProvider.GetService<ShipController>();
            _timeService = serviceProvider.GetService<ITimeService>();
        }

        public void Tick()
        {
            string healtStr = _shipController.CurrentHealth.ToString();
            if(_shipController.RemainingInvulnerability > 0)
            {
                healtStr = $"({healtStr})";
            }

            _lifeText.text = healtStr;
            _scoreText.text = _timeService.Time.ToString();
        }

    }
}
