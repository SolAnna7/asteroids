using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine.SceneManagement;

namespace Asteroid.Gameplay
{
    public static class GameplaySetupHelper
    {
        public static int CurrentScore { get; private set; } = -1;
        public static int CurrentHealth { get; private set; } = -1;
        public static int CurrentLevel { get; private set; } = 1;

        public static void LoadGameplayScene(int health = -1, int score = -1, int level = 1)
        {
            CurrentHealth = health;
            CurrentScore = score;
            CurrentLevel = level;
            SceneManager.LoadScene("GameplayScene");
        }
    }
}
