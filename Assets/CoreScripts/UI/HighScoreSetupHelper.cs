using UnityEngine.SceneManagement;

namespace Asteroid.UI
{
    /// <summary>
    /// Static class that helps with loading the high score scene with parameters
    /// </summary>
    public static class HighScoreSetupHelper
    {
        public static HighScoreOpenMode OpenMode { get; private set; }
        public static int NewScore { get; private set; }

        public static void OpenFromMenu()
        {
            NewScore = -1;
            OpenMode = HighScoreOpenMode.FromMenu;
            SceneManager.LoadScene("HighScoreScene");
        }
        public static void OpenFromGameplay(int score)
        {
            NewScore = score;
            OpenMode = HighScoreOpenMode.FromGame;
            SceneManager.LoadScene("HighScoreScene");
        }

        public enum HighScoreOpenMode
        {
            FromMenu,
            FromGame
        }
    }
}
