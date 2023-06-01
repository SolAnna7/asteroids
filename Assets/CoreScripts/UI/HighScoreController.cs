using Asteroid.Gameplay;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace Asteroid.UI
{
    /// <summary>
    /// Unity component controlling the high score ui logic
    /// </summary>
    public class HighScoreController : MonoBehaviour
    {
        private const int storedScoreNum = 8;

        [Header("Score list")]
        [SerializeField]
        private TMP_Text _scoreText;
        [SerializeField]
        private GameObject _scoreList;
        [SerializeField]
        private GameObject _scoreLinePrototype;
        [SerializeField]
        private GameObject _scoreInputPrototype;
        [Header("Buttons")]
        [SerializeField]
        private Button _menuButton;
        [SerializeField]
        private Button _backToGameButton;

        private TMP_InputField _nameInputField;
        private List<HighScoreEntry> _currentScores;

        private void Start()
        {

            switch (HighScoreSetupHelper.OpenMode)
            {
                case HighScoreSetupHelper.HighScoreOpenMode.FromMenu:
                    _menuButton.onClick.AddListener(() => { SceneManager.LoadScene("MainMenuScene"); });
                    _backToGameButton.gameObject.SetActive(false);
                    _scoreText.gameObject.SetActive(false);
                    break;
                case HighScoreSetupHelper.HighScoreOpenMode.FromGame:
                    _backToGameButton.onClick.AddListener(() =>
                    {
                        if (HighScoreSetupHelper.NewScore != 0)
                        {
                            AddScoreAndSave();
                        }
                        GameplaySetupHelper.LoadGameplayScene();
                    });
                    _menuButton.onClick.AddListener(() =>
                    {
                        if (HighScoreSetupHelper.NewScore != 0)
                        {
                            AddScoreAndSave();
                        }
                        SceneManager.LoadScene("MainMenuScene");
                    });

                    _backToGameButton.interactable = HighScoreSetupHelper.NewScore == 0;
                    _menuButton.interactable = HighScoreSetupHelper.NewScore == 0;
                    _scoreText.text = $"New score: {HighScoreSetupHelper.NewScore}";
                    break;
            }

            bool newScoreLineAdded = false;
            _currentScores = LoadHighScores().OrderByDescending(s => s.score).ToList();
            int existingScoreCntr = 0;
            for (int i = 0; i < storedScoreNum; i++)
            {
                HighScoreEntry item = _currentScores[existingScoreCntr];
                if (HighScoreSetupHelper.OpenMode == HighScoreSetupHelper.HighScoreOpenMode.FromGame
                    && !newScoreLineAdded
                    && HighScoreSetupHelper.NewScore > item.score)
                {
                    var inputLine = Instantiate(_scoreInputPrototype, _scoreList.transform);
                    _nameInputField = inputLine.GetComponentInChildren<TMP_InputField>();
                    inputLine.transform.Find("NumberText").GetComponent<TMP_Text>().text = (i + 1).ToString();
                    inputLine.transform.Find("ScoreText").GetComponent<TMP_Text>().text = HighScoreSetupHelper.NewScore.ToString();
                    _nameInputField.Select();
                    newScoreLineAdded = true;
                }
                else
                {
                    var inputLine = Instantiate(_scoreLinePrototype, _scoreList.transform);
                    inputLine.transform.Find("NumberText").GetComponent<TMP_Text>().text = i.ToString();
                    inputLine.transform.Find("NameText").GetComponent<TMP_Text>().text = item.name;
                    inputLine.transform.Find("ScoreText").GetComponent<TMP_Text>().text = item.score == 0 ? "" : item.score.ToString();

                    existingScoreCntr++;
                }
            }

            _scoreLinePrototype.gameObject.SetActive(false);
            _scoreInputPrototype.gameObject.SetActive(false);
        }


        private void Update()
        {
            if (_nameInputField != null)
            {
                bool isnameFilled = !string.IsNullOrWhiteSpace(_nameInputField.text);
                _menuButton.interactable = isnameFilled;
                _backToGameButton.interactable = isnameFilled;
            }
        }

        private List<HighScoreEntry> LoadHighScores()
        {
            List<HighScoreEntry> res = new List<HighScoreEntry>();

            for (int i = 0; i < storedScoreNum; i++)
            {
                string name = PlayerPrefs.GetString($"highscore.{i}.name", null);
                int score = PlayerPrefs.GetInt($"highscore.{i}.score", -1);

                if (name == null || score < 0)
                {
                    res.Add(new HighScoreEntry("--EMPTY--", 0));
                }
                else
                {
                    res.Add(new HighScoreEntry(name, score));
                }
            }

            return res;
        }

        private void AddScoreAndSave()
        {
            var scoresToSave = _currentScores.ToList();
            scoresToSave.Add(new HighScoreEntry(_nameInputField.text.Trim(), HighScoreSetupHelper.NewScore));
            scoresToSave = scoresToSave.OrderByDescending(s => s.score).ToList();
            SaveHighScores(scoresToSave);
        }

        private void SaveHighScores(List<HighScoreEntry> scores)
        {

            for (int i = 0; i < storedScoreNum; i++)
            {
                HighScoreEntry item = scores[i];

                PlayerPrefs.SetString($"highscore.{i}.name", item.name);
                PlayerPrefs.SetInt($"highscore.{i}.score", item.score);
            }

            PlayerPrefs.Save();
        }

        private class HighScoreEntry
        {
            public HighScoreEntry(string name, int score)
            {
                this.name = name;
                this.score = score;
            }

            public string name;
            public int score;
        }
    }
}
