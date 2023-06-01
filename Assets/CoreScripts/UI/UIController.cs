using Asteroid.Gameplay;
using Asteroid.Input;
using Asteroid.Services;
using Asteroid.Time;
using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace Asteroid.UI
{
    /// <summary>
    /// Unity component controlling the gameplay ui logic
    /// </summary>
    public class UIController : MonoBehaviour
    {
        [Header("Game UI")]
        [SerializeField]
        private Transform _lifeIconParent;
        [SerializeField]
        private GameObject _prototypeIcon;
        [SerializeField]
        private TMP_Text _scoreText;

        [Header("Pause Menu")]
        [SerializeField]
        private GameObject _pauseMenu;
        [SerializeField]
        private Button _resumeButton;
        [SerializeField]
        private Button _settingsButton;
        [SerializeField]
        private Button _exitButton;

        [Header("Pause Menu")]
        [SerializeField]
        private GameObject _infoMenu;
        [SerializeField]
        private TMP_Text _infoText;
        [SerializeField]
        private TMP_Text _infoConfirmButtonText;
        [SerializeField]
        private Button _infoConfirmButton;


        private IShipController _shipController;
        private IAsteroidController _asteroidController;
        private IInputHandler _inputHandler;
        private ITimeService _timeService;
        private IScoreCounterService _scoreService;

        private Stack<GameObject> _lifeIconStack = new Stack<GameObject>();
        public void Initialise(ServiceProvider serviceProvider)
        {
            _shipController = serviceProvider.GetService<IShipController>();
            _asteroidController = serviceProvider.GetService<IAsteroidController>();
            _inputHandler = serviceProvider.GetService<IInputHandler>();
            _timeService = serviceProvider.GetService<ITimeService>();
            _scoreService = serviceProvider.GetService<IScoreCounterService>();

            _inputHandler.PauseToggle += PauseToggle;
            _asteroidController.OnAllAsteroidsDestroyed += OnAllAsteroidsDestroyed;
            _shipController.OnDeath += ShipController_OnDeath;
            _shipController.OnDamage += ShipController_OnDamage;

            for (int i = 0; i < _shipController.CurrentHealth; i++)
            {
                var icon = Instantiate(_prototypeIcon, _lifeIconParent);
                _lifeIconStack.Push(icon);
            }
            _prototypeIcon.SetActive(false);
        }

        private void ShipController_OnDamage()
        {
            _lifeIconStack.Pop().SetActive(false);
        }

        private void OnAllAsteroidsDestroyed()
        {
            _timeService.TimeRunning = false;
            _infoMenu.SetActive(true);
            _infoText.text = $"Congratulations, you survived the level!\nYour score is {_scoreService.CurrentScore}";
            _infoConfirmButtonText.text = "To the next level!";
            _infoConfirmButton.onClick.RemoveAllListeners();
            _infoConfirmButton.onClick.AddListener(() =>
            {
                _timeService.TimeRunning = true;
                GameplaySetupHelper.LoadGameplayScene(_shipController.CurrentHealth, _scoreService.CurrentScore, GameplaySetupHelper.CurrentLevel + 1);
            });
        }

        private void ShipController_OnDeath()
        {
            _timeService.TimeRunning = false;
            _infoMenu.SetActive(true);
            _infoText.text = $"Oh no, you died!\nYour score is {_scoreService.CurrentScore}";
            _infoConfirmButtonText.text = "Ok :(";
            _infoConfirmButton.onClick.RemoveAllListeners();
            _infoConfirmButton.onClick.AddListener(() =>
            {
                _timeService.TimeRunning = true;
                HighScoreSetupHelper.OpenFromGameplay(_scoreService.CurrentScore);
            });
        }

        private void Awake()
        {
            _resumeButton.onClick.AddListener(PauseToggle);
            _exitButton.onClick.AddListener(() => { SceneManager.LoadScene("MainMenuScene"); });
        }

        private void PauseToggle()
        {
            bool currentMenuState = !_pauseMenu.activeSelf;
            _timeService.TimeRunning = !currentMenuState;
            _pauseMenu.SetActive(currentMenuState);
        }

        public void Tick()
        {
            _scoreText.text = _scoreService.CurrentScore.ToString();
        }

    }
}
