using Asteroid.Config;
using Asteroid.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Asteroid.Gameplay
{
    public interface IScoreCounterService
    {
        public int CurrentScore { get; }
    }

    public class ScoreCounterService : ServiceProvider.IInitialisableService, IScoreCounterService
    {
        private IAsteroidController _asteroidController;
        private IConfigStore _configStore;
        private int[] _asteroidScoresByGen;

        public int CurrentScore { get; private set; }

        public void Initialise(ServiceProvider serviceProvider)
        {
            _asteroidController = serviceProvider.GetService<IAsteroidController>();
            _configStore = serviceProvider.GetService<IConfigStore>();

            _asteroidController.OnAsteroidHitByBullet += OnAsteroidHit;
            _asteroidScoresByGen = _configStore.AsteroidScoresByGeneration;

            if (GameplaySetupHelper.CurrentScore > 0)
            {
                CurrentScore = GameplaySetupHelper.CurrentScore;
            }

        }

        private void OnAsteroidHit(int gen)
        {
            if (gen > _asteroidScoresByGen.Length - 1)
            {
                throw new Exception($"No score configured for generation {gen} asteroid");
            }

            CurrentScore += _asteroidScoresByGen[gen] * GameplaySetupHelper.CurrentLevel;

        }
    }
}
