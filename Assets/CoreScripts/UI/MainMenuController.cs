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

        void Awake()
        {
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
        _exitButton.onClick.AddListener(() => { Application.Quit; });
#endif
        }

        // Update is called once per frame
        void Update()
        {

        }
    }

}