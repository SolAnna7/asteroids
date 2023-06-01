using Asteroid.Gameplay;
using UnityEngine;
using UnityEngine.UI;

namespace Asteroid.UI
{
    /// <summary>
    /// Unity component controlling the main menu ui logic
    /// </summary>
    public class MainMenuController : MonoBehaviour
    {

        [Header("Menu Buttons")]
        [SerializeField]
        private Button _startButton;
        [SerializeField]
        private Button _highScoreButton;
        [SerializeField]
        private Button _settingsButton;
        [SerializeField]
        private Button _exitButton;

        [Header("Ship")]
        [SerializeField]
        private GameObject _ship;
        [SerializeField]
        private float _shipMovementMultiplier;
        [SerializeField]
        private GameObject _shipLight;

        private Vector2 _originalShipPos;

        void Awake()
        {
            _originalShipPos = _ship.transform.position;

            _startButton.onClick.AddListener(() =>
            {
                GameplaySetupHelper.LoadGameplayScene();
            });
            _highScoreButton.onClick.AddListener(() =>
            {
                HighScoreSetupHelper.OpenFromMenu();
            });
#if UNITY_EDITOR
            _exitButton.onClick.AddListener(() => { UnityEditor.EditorApplication.isPlaying = false; });
#else
            _exitButton.onClick.AddListener(() => { Application.Quit(); });
#endif
        }

        // Update is called once per frame
        void Update()
        {
            float time = UnityEngine.Time.time;
            _ship.transform.position = _originalShipPos + _shipMovementMultiplier * new Vector2(
                Mathf.Sin(time/4) + Mathf.Cos(time /2 + 10) * -2,
                Mathf.Cos(time/4 + 7) * 2 + Mathf.Sin(time /2) * 4);
        }
    }

}